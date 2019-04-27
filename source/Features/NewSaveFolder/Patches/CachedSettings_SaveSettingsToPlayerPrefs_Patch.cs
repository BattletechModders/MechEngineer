using System.Collections.Generic;
using BattleTech.Save;
using Harmony;

namespace MechEngineer
{
    //[HarmonyPatch(typeof(CachedSettings), "SaveSettingsToPlayerPrefs")]
    public static class CachedSettings_SaveSettingsToPlayerPrefs_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return NewSaveFolderHandlers.Transpiler(instructions);
        }
    }
}