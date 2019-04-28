using System.Collections.Generic;
using BattleTech.Save;
using Harmony;

namespace MechEngineer.Features.NewSaveFolder.Patches
{
    [HarmonyPatch(typeof(CachedSettings), nameof(CachedSettings.Settings), MethodType.Getter)]
    public static class CachedSettings_SettingsGetter_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return NewSaveFolderFeature.Transpiler(instructions);
        }
    }
}
