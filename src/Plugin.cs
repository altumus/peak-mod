using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace PeakShop;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = "com.altumus.peakshop";
    public const string PluginName = "peak-shop";
    public const string PluginVersion = "1.0.0";

    public static ManualLogSource Log = null!;

    private void Awake()
    {
        Log = Logger;
        new Harmony(PluginGuid).PatchAll();
        Log.LogInfo($"{PluginName} v{PluginVersion} loaded. Interact with the shop kiosk next to a campfire.");
    }

    private void Update()
    {
        if (ShopWindow.IsOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            ShopWindow.IsOpen = false;
        }

        if (ShopWindow.IsOpen)
        {
            if (Cursor.lockState != CursorLockMode.None) Cursor.lockState = CursorLockMode.None;
            if (!Cursor.visible) Cursor.visible = true;
        }
    }

    private void OnGUI()
    {
        if (ShopWindow.IsOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        CoinHUD.Draw();
        ShopWindow.Draw();
    }
}
