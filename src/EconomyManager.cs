using UnityEngine;

namespace HookGun;

internal static class EconomyManager
{
    private static int _coins;

    public static int Coins => _coins;

    public static void Reset()
    {
        _coins = 0;
    }

    public static void AddCoins(int amount, string? source = null)
    {
        if (amount <= 0) return;
        _coins += amount;
        if (source != null)
            Plugin.Log.LogInfo($"+{amount} coins ({source}) → {_coins} total");
        else
            Plugin.Log.LogInfo($"+{amount} coins → {_coins} total");
    }

    public static bool TrySpend(int amount)
    {
        if (amount <= 0) return true;
        if (_coins < amount) return false;
        _coins -= amount;
        Plugin.Log.LogInfo($"-{amount} coins → {_coins} total");
        return true;
    }

    public static int LuggageReward(Luggage luggage)
    {
        // Базовая награда 8-18 монет. Можно расширить если у Luggage есть тип/displayName.
        return Random.Range(8, 19);
    }
}
