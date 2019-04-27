using System.Collections.Generic;
using BattleTech.Save;
using Harmony;

namespace MechEngineer
{
    //[HarmonyPatch(typeof(CachedSettings), nameof(CachedSettings.Settings), MethodType.Setter)]
    public static class CachedSettings_SettingsSetter_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return NewSaveFolderHandlers.Transpiler(instructions);
        }
    }
}