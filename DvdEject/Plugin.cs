using BepInEx;
using Enemy;
using HarmonyLib;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DvdEject
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string pluginGuid = "patman.iamyourbeast.dvdeject";
        public const string pluginName = "-eject";
        public const string pluginVersion = "1.0.0";

        public void Awake()
        {
            Logger.LogInfo("your dvd drive is: damn ejected");
            new Harmony(pluginGuid).PatchAll();
        }

        [DllImport("winmm.dll")]
        protected static extern int mciSendString(string command, string buffer, int bufferSize, IntPtr hwndCallback);

        public static void OpenDrive()
        {
            Task.Run(() => mciSendString("set CDAudio door open", "", 127, IntPtr.Zero));
        }

        public static void CloseDrive()
        {
            Task.Run(() => mciSendString("set CDAudio door closed", "", 127, IntPtr.Zero));
        }

        [HarmonyPatch(typeof(EnemyHuman))]
        public class KillEnemy
        {
            [HarmonyPatch(nameof(EnemyHuman.Kill))]
            [HarmonyPostfix]
            public static void Postfix()
            {
                CloseDrive();
            }
        }

        [HarmonyPatch(typeof(PlayerHealthManager))]
        public class PlayerDamage
        {
            [HarmonyPatch(nameof(PlayerHealthManager.Damage))]
            [HarmonyPostfix]
            public static void Postfix()
            {
                OpenDrive();
            }
        }
    }
}
