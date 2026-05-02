using UnityEngine;

namespace HookGun;

internal static class ShopStyle
{
    public static readonly Color Backdrop = new(0f, 0f, 0f, 0.55f);
    public static readonly Color Blue = new(0.118f, 0.384f, 0.847f, 1f);
    public static readonly Color BlueDark = new(0.078f, 0.275f, 0.659f, 1f);
    public static readonly Color BlueDeep = new(0.063f, 0.196f, 0.471f, 1f);
    public static readonly Color Paper = new(0.910f, 0.949f, 0.886f, 1f);
    public static readonly Color PaperDim = new(0.835f, 0.890f, 0.808f, 1f);
    public static readonly Color White = Color.white;
    public static readonly Color Yellow = new(0.961f, 0.847f, 0.357f, 1f);
    public static readonly Color TextOnBlue = Color.white;
    public static readonly Color TextOnPaper = new(0.078f, 0.275f, 0.659f, 1f);

    private static Texture2D? _backdropTex;
    private static Texture2D? _blueTex;
    private static Texture2D? _blueDarkTex;
    private static Texture2D? _blueDeepTex;
    private static Texture2D? _paperTex;
    private static Texture2D? _paperDimTex;
    private static Texture2D? _whiteTex;
    private static Texture2D? _yellowTex;

    public static GUIStyle? Window;
    public static GUIStyle? TitleHuge;
    public static GUIStyle? TitleSmall;
    public static GUIStyle? Label;
    public static GUIStyle? LabelDim;
    public static GUIStyle? Button;
    public static GUIStyle? TextField;
    public static GUIStyle? Tile;
    public static GUIStyle? TileLabel;
    public static GUIStyle? PriceLabel;
    public static GUIStyle? PriceLabelMissing;
    public static GUIStyle? CoinLabel;
    public static GUIStyle? Invisible;

    private static bool _initialized;
    private static Font? _gameFont;

    public static Font? GameFont => _gameFont;
    public static Texture2D? PaperTex => _paperTex;
    public static Texture2D? PaperDimTex => _paperDimTex;
    public static Texture2D? BlueTex => _blueTex;
    public static Texture2D? BlueDarkTex => _blueDarkTex;
    public static Texture2D? WhiteTex => _whiteTex;
    public static Texture2D? YellowTex => _yellowTex;

    public static void EnsureInitialized()
    {
        if (_initialized) return;

        _backdropTex = MakeTex(Backdrop);
        _blueTex = MakeTex(Blue);
        _blueDarkTex = MakeTex(BlueDark);
        _blueDeepTex = MakeTex(BlueDeep);
        _paperTex = MakeTex(Paper);
        _paperDimTex = MakeTex(PaperDim);
        _whiteTex = MakeTex(White);
        _yellowTex = MakeTex(Yellow);

        _gameFont = TryResolveGameFont();

        Window = new GUIStyle
        {
            padding = new RectOffset(24, 24, 22, 22),
            margin = new RectOffset(0, 0, 0, 0),
            richText = true,
        };
        Window.normal.background = _blueTex;

        TitleHuge = new GUIStyle
        {
            fontSize = 30,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            richText = true,
            padding = new RectOffset(0, 0, 4, 4),
        };
        TitleHuge.normal.textColor = TextOnBlue;

        TitleSmall = new GUIStyle
        {
            fontSize = 12,
            alignment = TextAnchor.MiddleCenter,
            richText = true,
            padding = new RectOffset(0, 0, 0, 6),
        };
        TitleSmall.normal.textColor = new Color(1f, 1f, 1f, 0.7f);

        Label = new GUIStyle
        {
            fontSize = 13,
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(8, 8, 4, 4),
            richText = true,
        };
        Label.normal.textColor = TextOnBlue;

        LabelDim = new GUIStyle(Label)
        {
            fontSize = 11,
        };
        LabelDim.normal.textColor = new Color(1f, 1f, 1f, 0.7f);

        Button = new GUIStyle
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(14, 14, 8, 8),
            border = new RectOffset(2, 2, 2, 2),
            richText = true,
        };
        Button.normal.background = _blueDarkTex;
        Button.hover.background = _whiteTex;
        Button.active.background = _yellowTex;
        Button.focused.background = _blueDarkTex;
        Button.normal.textColor = TextOnBlue;
        Button.hover.textColor = TextOnPaper;
        Button.active.textColor = TextOnPaper;
        Button.focused.textColor = TextOnBlue;

        TextField = new GUIStyle
        {
            fontSize = 13,
            padding = new RectOffset(10, 10, 6, 6),
            border = new RectOffset(1, 1, 1, 1),
            alignment = TextAnchor.MiddleLeft,
        };
        TextField.normal.background = _whiteTex;
        TextField.hover.background = _whiteTex;
        TextField.focused.background = _whiteTex;
        TextField.normal.textColor = TextOnPaper;
        TextField.hover.textColor = TextOnPaper;
        TextField.focused.textColor = TextOnPaper;

        Tile = new GUIStyle
        {
            border = new RectOffset(2, 2, 2, 2),
            padding = new RectOffset(8, 8, 8, 8),
            margin = new RectOffset(0, 0, 0, 0),
        };
        Tile.normal.background = _paperTex;

        TileLabel = new GUIStyle
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(2, 2, 2, 2),
            wordWrap = true,
            richText = true,
        };
        TileLabel.normal.textColor = TextOnPaper;

        PriceLabel = new GUIStyle
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(2, 2, 2, 2),
            richText = true,
        };
        PriceLabel.normal.textColor = new Color(0.078f, 0.275f, 0.659f, 1f);

        PriceLabelMissing = new GUIStyle(PriceLabel);
        PriceLabelMissing.normal.textColor = new Color(0.65f, 0.18f, 0.18f, 1f);

        CoinLabel = new GUIStyle
        {
            fontSize = 18,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            richText = true,
        };
        CoinLabel.normal.textColor = Yellow;

        Invisible = new GUIStyle
        {
            border = new RectOffset(0, 0, 0, 0),
            padding = new RectOffset(0, 0, 0, 0),
            margin = new RectOffset(0, 0, 0, 0),
        };
        Invisible.normal.background = null;

        ApplyGameFont();
        _initialized = true;
    }

    private static Font? TryResolveGameFont()
    {
        try
        {
            var tmpAssets = Resources.FindObjectsOfTypeAll<TMPro.TMP_FontAsset>();
            Font? best = null;
            int bestScore = -1;
            string bestName = "";

            foreach (var tmp in tmpAssets)
            {
                if (tmp == null || tmp.sourceFontFile == null) continue;

                var name = tmp.name ?? string.Empty;
                int score = tmp.atlasWidth;
                if (name.IndexOf("Daggersquare", System.StringComparison.OrdinalIgnoreCase) >= 0) score += 10000;
                if (name.IndexOf("Bone", System.StringComparison.OrdinalIgnoreCase) >= 0) score += 8000;
                if (name.IndexOf("Trajan", System.StringComparison.OrdinalIgnoreCase) >= 0) score += 6000;
                if (name.IndexOf("Title", System.StringComparison.OrdinalIgnoreCase) >= 0) score += 2000;

                if (score > bestScore)
                {
                    bestScore = score;
                    best = tmp.sourceFontFile;
                    bestName = name;
                }
            }

            if (best != null)
            {
                Plugin.Log.LogInfo($"Using game font: TMP='{bestName}' → Font='{best.name}'");
            }
            else
            {
                Plugin.Log.LogWarning("No usable TMP_FontAsset.sourceFontFile found; falling back to default IMGUI font.");
            }
            return best;
        }
        catch (System.Exception e)
        {
            Plugin.Log.LogError($"TryResolveGameFont failed: {e}");
            return null;
        }
    }

    private static void ApplyGameFont()
    {
        if (_gameFont == null) return;
        foreach (var style in new[] { Window, TitleHuge, TitleSmall, Label, LabelDim, Button, TextField, Tile, TileLabel, PriceLabel, PriceLabelMissing, CoinLabel, Invisible })
        {
            if (style != null) style.font = _gameFont;
        }
    }

    public static void DrawBackdrop()
    {
        if (_backdropTex == null) return;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _backdropTex);
    }

    public static void DrawSolid(Rect rect, Color color)
    {
        var tex = MakeTex(color);
        GUI.DrawTexture(rect, tex);
    }

    public static void DrawBorder(Rect rect, float thickness, Color color)
    {
        var tex = MakeTex(color);
        GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, thickness), tex);
        GUI.DrawTexture(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), tex);
        GUI.DrawTexture(new Rect(rect.x, rect.y, thickness, rect.height), tex);
        GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), tex);
    }

    public static void DrawHorizontalLine(float x, float y, float width, Color color)
    {
        var tex = MakeTex(color);
        GUI.DrawTexture(new Rect(x, y, width, 1f), tex);
    }

    private static Texture2D MakeTex(Color color)
    {
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, mipChain: false)
        {
            hideFlags = HideFlags.HideAndDontSave,
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
        };
        var pixels = new[] { color, color, color, color };
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }
}
