using BattleTech.UI;
using Harmony;

namespace MechEngineer.Test
{
    [HarmonyPatch(typeof(MechLabPanel), "ShowDropErrorMessage")]
    public static  class test_patch
    {
        public static void Postfix(string msg)
        {
            Control.mod.Logger.LogDebug($"MSG: {msg}");
        }
    }
}