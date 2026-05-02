using HarmonyLib;
using UnityEngine;

namespace HookGun;

[HarmonyPatch(typeof(CharacterInput), "Sample")]
internal static class CharacterInput_Sample_Patch
{
    private static void Prefix(ref bool playerMovementActive)
    {
        if (ShopWindow.IsOpen)
        {
            playerMovementActive = false;
        }
    }
}

[HarmonyPatch(typeof(Luggage), "OpenLuggageRPC")]
internal static class Luggage_OpenLuggageRPC_Patch
{
    private static readonly System.Collections.Generic.HashSet<int> _rewarded = new();

    private static void Postfix(Luggage __instance, bool spawnItems)
    {
        if (!spawnItems) return;
        if (__instance == null) return;

        var id = __instance.GetInstanceID();
        if (!_rewarded.Add(id)) return;

        var reward = EconomyManager.LuggageReward(__instance);
        EconomyManager.AddCoins(reward, source: "luggage");
    }
}

[HarmonyPatch(typeof(Campfire), "Awake")]
internal static class Campfire_Awake_Patch
{
    private static readonly System.Collections.Generic.HashSet<int> _spawnedFor = new();

    private static void Postfix(Campfire __instance)
    {
        if (__instance == null) return;
        var id = __instance.GetInstanceID();
        if (!_spawnedFor.Add(id)) return;

        try
        {
            var t = __instance.transform;
            var offset = t.right * 2.5f + Vector3.up * 0.0f;
            var pos = t.position + offset;

            // Притянуть к земле, если возможно
            if (UnityEngine.Physics.Raycast(pos + Vector3.up * 4f, Vector3.down, out var hit, 12f))
            {
                pos.y = hit.point.y;
            }

            var rotation = Quaternion.LookRotation(t.position - pos, Vector3.up);
            ShopKiosk.Build(pos, rotation);
            Plugin.Log.LogDebug($"ShopKiosk spawned next to campfire at {pos}.");
        }
        catch (System.Exception e)
        {
            Plugin.Log.LogError($"Failed to spawn ShopKiosk for campfire: {e}");
        }
    }
}
