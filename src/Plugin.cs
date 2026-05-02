using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace HookGun;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = "com.altumus.hookgun";
    public const string PluginName = "HookGun";
    public const string PluginVersion = "0.3.0";

    public static ManualLogSource Log = null!;

    private void Awake()
    {
        Log = Logger;
        new Harmony(PluginGuid).PatchAll();
        Log.LogInfo($"{PluginName} v{PluginVersion} loaded. F9 toggles the item shop.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            ShopWindow.Toggle();
        }

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
