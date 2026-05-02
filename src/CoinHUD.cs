using UnityEngine;

namespace PeakShop;

internal static class CoinHUD
{
    private const int Width = 180;
    private const int Height = 56;
    private const int MarginX = 24;
    private const int MarginY = 24;

    private static GUIStyle? _coinStyle;
    private static GUIStyle? _hintStyle;

    public static void Draw()
    {
        if (ShopWindow.IsOpen) return;
        if (Character.localCharacter == null) return;

        ShopStyle.EnsureInitialized();
        EnsureLocalStyles();

        var rect = new Rect(
            Screen.width - Width - MarginX,
            MarginY,
            Width,
            Height);

        if (ShopStyle.BlueTex != null) GUI.DrawTexture(rect, ShopStyle.BlueTex);
        ShopStyle.DrawBorder(rect, 2f, ShopStyle.White);

        var coinsRect = new Rect(rect.x, rect.y + 4, rect.width, 28);
        GUI.Label(coinsRect, $"{EconomyManager.Coins} COINS", _coinStyle);

        var hintRect = new Rect(rect.x, rect.y + 32, rect.width, 18);
        GUI.Label(hintRect, "find a shop kiosk", _hintStyle);
    }

    private static void EnsureLocalStyles()
    {
        if (_coinStyle != null && _hintStyle != null) return;

        _coinStyle = new GUIStyle
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            richText = true,
        };
        _coinStyle.normal.textColor = ShopStyle.Yellow;
        if (ShopStyle.GameFont != null) _coinStyle.font = ShopStyle.GameFont;

        _hintStyle = new GUIStyle
        {
            fontSize = 11,
            alignment = TextAnchor.MiddleCenter,
            richText = true,
        };
        _hintStyle.normal.textColor = new Color(1f, 1f, 1f, 0.78f);
        if (ShopStyle.GameFont != null) _hintStyle.font = ShopStyle.GameFont;
    }
}
