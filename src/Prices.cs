using System.Collections.Generic;

namespace PeakShop;

internal static class Prices
{
    // Базовая ценовая лесенка:
    //   5 — мусор/расходники мелкие
    //  10-25 — еда, простые лечилки, инструменты
    //  30-50 — продвинутые лечилки, инструменты средней редкости
    //  60-100 — редкие предметы
    // 125-250 — special/legendary предметы
    private static readonly Dictionary<string, int> _exact = new(System.StringComparer.OrdinalIgnoreCase)
    {
        // Food
        { "Marshmallow", 16 },
        { "Lollipop", 20 },
        { "Airplane Food", 11 },
        { "Granola Bar", 7 },
        { "ScoutCookies", 12 },
        { "TrailMix", 5 },

        // Meds
        { "Bandages", 8 },
        { "Heat Pack", 11 },
        { "Antidote", 23 },
        { "FirstAidKit", 30 },
        { "Cure-All", 65 },
        { "ScoutEffigy", 75 },
        { "Cure-Some", 25 },
        { "EnergyElixir", 32 },
        { "Energy Drink", 15 },
        { "Sports Drink", 10 },
        { "Napberry", 9 },

        // Tools
        { "Flare", 6 },
        { "Lantern", 25 },
        { "Compass", 20 },
        { "Bugle", 18 },
        { "PortableStovetopItem", 45 },
        { "RopeShooter", 38 },
        { "RopeSpool", 22 },
        { "Anti-Rope Spool", 22 },
        { "RopeShooterAnti", 42 },
        { "ChainShooter", 40 },
        { "Piton", 20 },
        { "ShelfShroom", 5 },
        { "Backpack", 60 },
        { "Binoculars", 15 },
        { "Parasol", 18 },
        { "Megaphone", 12 },

        // Special
        { "Bugle_Magic", 88 },
        { "Lantern_Faerie", 70 },
        { "PandorasBox", 125 },
        { "HealingDart Variant", 35 },
        { "Cursed Skull", 100 },
        { "Pirate Compass", 80 },
        { "Bugle_Scoutmaster Variant", 45 },
        { "MagicBean", 40 },
        { "Strange Gem", 50 },
        { "BounceShroom", 8 },
        { "Warp Compass", 75 },
        { "Frisbee", 3 },
        { "BingBong", 250 },
    };

    public const int FallbackPrice = 35;

    public static int GetPrice(string prefabName, string displayName)
    {
        if (_exact.TryGetValue(prefabName, out var p)) return p;
        if (_exact.TryGetValue(displayName, out p)) return p;
        return GuessPrice(prefabName);
    }

    private static int GuessPrice(string prefabName)
    {
        var n = prefabName.ToLowerInvariant();

        // Ягоды/еда — дёшево
        if (n.Contains("berry") || n.Contains("nana") || n.Contains("shroom") || n.Contains("apple") || n.Contains("kingberry"))
            return 6;
        // Мобы/идолы/special
        if (n.Contains("idol") || n.Contains("totem") || n.Contains("relic") || n.Contains("ancient"))
            return 90;
        // Лечащее
        if (n.Contains("dart") || n.Contains("aid") || n.Contains("bandage") || n.Contains("pack"))
            return 25;
        // Инструменты
        if (n.Contains("rope") || n.Contains("hook") || n.Contains("piton") || n.Contains("chain"))
            return 30;
        // Освещение
        if (n.Contains("lantern") || n.Contains("flare") || n.Contains("torch"))
            return 18;
        // Magic / faerie / cursed
        if (n.Contains("magic") || n.Contains("faerie") || n.Contains("cursed") || n.Contains("warp"))
            return 70;
        // Bishop / King / piece — likely chess set / collectibles
        if (n.Contains("bishop") || n.Contains("king") || n.Contains("queen") || n.Contains("pawn"))
            return 40;

        return FallbackPrice;
    }
}
