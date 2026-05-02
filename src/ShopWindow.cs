using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

namespace HookGun;

internal static class ShopWindow
{
    public static bool IsOpen;

    private const int WindowId = 0x484F4F4B;
    private const int WindowWidth = 920;
    private const int WindowHeight = 720;
    private const int Columns = 5;
    private const int TileSize = 150;
    private const int TileGap = 10;

    private static Rect _windowRect;
    private static Vector2 _scroll;
    private static string _filter = string.Empty;
    private static List<ItemEntry>? _cachedItems;

    private readonly struct ItemEntry
    {
        public readonly Item Item;
        public readonly string PrefabName;
        public readonly string DisplayName;
        public readonly Texture2D? Icon;
        public readonly int Price;

        public ItemEntry(Item item, string prefabName, string displayName, Texture2D? icon, int price)
        {
            Item = item;
            PrefabName = prefabName;
            DisplayName = displayName;
            Icon = icon;
            Price = price;
        }
    }

    public static void Toggle()
    {
        IsOpen = !IsOpen;
        if (IsOpen)
        {
            _windowRect = new Rect(
                (Screen.width - WindowWidth) * 0.5f,
                (Screen.height - WindowHeight) * 0.5f,
                WindowWidth,
                WindowHeight);
            RefreshCatalog();
        }
    }

    public static void Draw()
    {
        if (!IsOpen) return;
        ShopStyle.EnsureInitialized();

        ShopStyle.DrawBackdrop();

        if (Event.current.type == EventType.MouseDown ||
            Event.current.type == EventType.MouseUp ||
            Event.current.type == EventType.ScrollWheel)
        {
            if (!_windowRect.Contains(Event.current.mousePosition))
            {
                Event.current.Use();
            }
        }

        _windowRect = GUI.Window(WindowId, _windowRect, DrawWindowContents, GUIContent.none, ShopStyle.Window);
        ShopStyle.DrawBorder(_windowRect, 3f, ShopStyle.White);
    }

    private static void DrawWindowContents(int id)
    {
        DrawHeader();
        GUILayout.Space(8);
        DrawSearchBar();
        GUILayout.Space(8);
        DrawSeparator();
        GUILayout.Space(8);

        DrawItemGrid();

        GUILayout.Space(8);
        DrawSeparator();
        GUILayout.Space(8);
        DrawFooter();

        GUI.DragWindow(new Rect(0, 0, _windowRect.width, 70));
    }

    private static void DrawHeader()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("ITEM SHOP", ShopStyle.TitleHuge);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        var character = Character.localCharacter;
        var canSpawn = character != null && PhotonNetwork.IsConnected;
        var total = _cachedItems?.Count ?? 0;

        GUILayout.BeginHorizontal();
        GUILayout.Label($"{total} items", ShopStyle.LabelDim, GUILayout.Width(120));
        GUILayout.FlexibleSpace();
        GUILayout.Label($"<b>{EconomyManager.Coins}</b> coins", ShopStyle.CoinLabel ?? ShopStyle.Label, GUILayout.Width(180));
        GUILayout.FlexibleSpace();
        GUILayout.Label("F9 / ESC to close", ShopStyle.LabelDim, GUILayout.Width(160));
        GUILayout.EndHorizontal();
    }

    private static void DrawSearchBar()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("SEARCH", ShopStyle.LabelDim, GUILayout.Width(70));
        _filter = GUILayout.TextField(_filter ?? string.Empty, ShopStyle.TextField, GUILayout.ExpandWidth(true), GUILayout.Height(30));
        if (GUILayout.Button("CLEAR", ShopStyle.Button, GUILayout.Width(100), GUILayout.Height(30)))
        {
            _filter = string.Empty;
        }
        GUILayout.EndHorizontal();
    }

    private static void DrawSeparator()
    {
        var lastRect = GUILayoutUtility.GetRect(1f, 1f, GUILayout.ExpandWidth(true));
        ShopStyle.DrawHorizontalLine(lastRect.x, lastRect.y, lastRect.width, new Color(1f, 1f, 1f, 0.4f));
    }

    private static void DrawItemGrid()
    {
        var character = Character.localCharacter;
        var canSpawn = character != null && PhotonNetwork.IsConnected;

        _scroll = GUILayout.BeginScrollView(_scroll);

        if (_cachedItems == null || _cachedItems.Count == 0)
        {
            GUILayout.Space(12);
            GUILayout.Label("No items loaded yet — try Refresh after entering a run.", ShopStyle.LabelDim);
            GUILayout.EndScrollView();
            return;
        }

        var query = _filter?.Trim() ?? string.Empty;
        var visible = new List<ItemEntry>();
        foreach (var entry in _cachedItems)
        {
            if (query.Length > 0 &&
                entry.DisplayName.IndexOf(query, System.StringComparison.OrdinalIgnoreCase) < 0 &&
                entry.PrefabName.IndexOf(query, System.StringComparison.OrdinalIgnoreCase) < 0)
            {
                continue;
            }
            visible.Add(entry);
        }

        if (visible.Count == 0)
        {
            GUILayout.Space(8);
            GUILayout.Label($"No items match \"{query}\".", ShopStyle.LabelDim);
            GUILayout.EndScrollView();
            return;
        }

        for (int row = 0; row < (visible.Count + Columns - 1) / Columns; row++)
        {
            GUILayout.BeginHorizontal();
            for (int col = 0; col < Columns; col++)
            {
                int index = row * Columns + col;
                if (index >= visible.Count)
                {
                    GUILayout.Space(TileSize + TileGap);
                    continue;
                }
                DrawTile(visible[index], character, canSpawn);
                if (col < Columns - 1) GUILayout.Space(TileGap);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(TileGap);
        }

        GUILayout.EndScrollView();
    }

    private static void DrawTile(ItemEntry entry, Character? character, bool canSpawn)
    {
        var rect = GUILayoutUtility.GetRect(TileSize, TileSize, GUILayout.Width(TileSize), GUILayout.Height(TileSize));
        bool hover = rect.Contains(Event.current.mousePosition);
        bool affordable = EconomyManager.Coins >= entry.Price;
        bool clickable = canSpawn && affordable;

        // Background panel — серая если не хватает монет
        if (!affordable && ShopStyle.PaperDimTex != null)
            GUI.DrawTexture(rect, ShopStyle.PaperDimTex);
        else if (ShopStyle.PaperTex != null)
            GUI.DrawTexture(rect, ShopStyle.PaperTex);

        // Icon area at top — slightly inset
        const int pad = 8;
        const int nameH = 28;
        const int priceH = 22;
        var iconArea = new Rect(rect.x + pad, rect.y + pad, rect.width - pad * 2, rect.height - pad * 2 - nameH - priceH);
        if (entry.Icon != null)
        {
            var prevColor = GUI.color;
            if (!affordable) GUI.color = new Color(1f, 1f, 1f, 0.45f);
            GUI.DrawTexture(iconArea, entry.Icon, ScaleMode.ScaleToFit, alphaBlend: true);
            GUI.color = prevColor;
        }
        else
        {
            ShopStyle.DrawSolid(iconArea, new Color(0.78f, 0.85f, 0.76f, 1f));
            var qStyle = new GUIStyle(ShopStyle.TileLabel)
            {
                fontSize = 32,
                fontStyle = FontStyle.Bold,
            };
            qStyle.normal.textColor = new Color(0.5f, 0.6f, 0.5f, 1f);
            GUI.Label(iconArea, "?", qStyle);
        }

        // Name strip
        var nameRect = new Rect(rect.x + 4, rect.yMax - nameH - priceH - 2, rect.width - 8, nameH);
        GUI.Label(nameRect, entry.DisplayName.ToUpperInvariant(), ShopStyle.TileLabel);

        // Price strip
        var priceRect = new Rect(rect.x + 4, rect.yMax - priceH - 2, rect.width - 8, priceH);
        var priceStyle = affordable ? (ShopStyle.PriceLabel ?? ShopStyle.TileLabel) : (ShopStyle.PriceLabelMissing ?? ShopStyle.TileLabel);
        GUI.Label(priceRect, $"{entry.Price} coins", priceStyle);

        // Border
        Color borderColor;
        float borderThickness;
        if (!affordable)
        {
            borderColor = new Color(0.55f, 0.6f, 0.55f, 1f);
            borderThickness = 2f;
        }
        else if (hover && clickable)
        {
            borderColor = ShopStyle.Yellow;
            borderThickness = 3f;
        }
        else
        {
            borderColor = ShopStyle.Blue;
            borderThickness = 2f;
        }
        ShopStyle.DrawBorder(rect, borderThickness, borderColor);

        // Click area
        GUI.enabled = clickable;
        if (GUI.Button(rect, GUIContent.none, ShopStyle.Invisible))
        {
            TryPurchase(entry, character!);
        }
        GUI.enabled = true;
    }

    private static void DrawFooter()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("REFRESH", ShopStyle.Button, GUILayout.Width(140), GUILayout.Height(34)))
        {
            RefreshCatalog();
        }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("CLOSE", ShopStyle.Button, GUILayout.Width(140), GUILayout.Height(34)))
        {
            IsOpen = false;
        }
        GUILayout.EndHorizontal();
    }

    private static void RefreshCatalog()
    {
        try
        {
            var db = SingletonAsset<ItemDatabase>.Instance;
            if (db == null)
            {
                Plugin.Log.LogWarning("ItemDatabase singleton is null.");
                _cachedItems = new List<ItemEntry>();
                return;
            }

            var seen = new HashSet<string>();
            var list = new List<ItemEntry>();

            void Consider(Item? item)
            {
                if (item == null || item.gameObject == null) return;
                var prefabName = item.gameObject.name;
                if (!seen.Add(prefabName)) return;

                var displayName = TryGetLocalizedName(item, prefabName);
                var icon = TryGetIcon(item);
                var price = Prices.GetPrice(prefabName, displayName);
                list.Add(new ItemEntry(item, prefabName, displayName, icon, price));
            }

            if (db.Objects != null)
            {
                foreach (var item in db.Objects) Consider(item);
            }

            if (db.itemLookup != null)
            {
                foreach (var kv in db.itemLookup) Consider(kv.Value);
            }

            _cachedItems = list.OrderBy(e => e.DisplayName, System.StringComparer.CurrentCultureIgnoreCase).ToList();
            Plugin.Log.LogInfo($"Shop catalog refreshed: {_cachedItems.Count} items.");
        }
        catch (System.Exception e)
        {
            Plugin.Log.LogError($"RefreshCatalog failed: {e}");
            _cachedItems = new List<ItemEntry>();
        }
    }

    private static string TryGetLocalizedName(Item item, string fallback)
    {
        try
        {
            var name = item.GetName();
            if (!string.IsNullOrWhiteSpace(name)) return name;
        }
        catch { }
        return fallback;
    }

    private static Texture2D? TryGetIcon(Item item)
    {
        try
        {
            if (item.UIData == null) return null;
            var tex = item.UIData.GetIcon();
            if (tex == null) tex = item.UIData.icon;
            return tex;
        }
        catch
        {
            return null;
        }
    }

    private static void TryPurchase(ItemEntry entry, Character character)
    {
        if (!EconomyManager.TrySpend(entry.Price))
        {
            Plugin.Log.LogInfo($"Cannot afford '{entry.DisplayName}' (need {entry.Price}, have {EconomyManager.Coins}).");
            return;
        }

        try
        {
            var t = character.transform;
            var pos = t.position + t.forward * 1.0f + Vector3.up * 0.8f;

            var go = PhotonNetwork.Instantiate(
                "0_Items/" + entry.PrefabName,
                pos,
                Quaternion.identity,
                0,
                null);

            if (go == null)
            {
                Plugin.Log.LogError($"Failed to spawn '{entry.PrefabName}' — refunding.");
                EconomyManager.AddCoins(entry.Price, source: "refund");
                return;
            }

            var item = go.GetComponent<Item>();
            if (item != null)
            {
                item.RequestPickup(character.GetComponent<PhotonView>());
            }

            Plugin.Log.LogInfo($"Bought '{entry.DisplayName}' ({entry.PrefabName}) for {entry.Price}.");
        }
        catch (System.Exception e)
        {
            Plugin.Log.LogError($"Purchase '{entry.PrefabName}' failed: {e} — refunding.");
            EconomyManager.AddCoins(entry.Price, source: "refund");
        }
    }
}
