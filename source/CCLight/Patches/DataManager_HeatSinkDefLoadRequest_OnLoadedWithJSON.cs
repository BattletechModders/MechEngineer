using System.Collections.Generic;
using BattleTech;
using BattleTech.Data;
using Harmony;

namespace CustomComponents
{
    [HarmonyNested(typeof(DataManager), "HeatSinkDefLoadRequest", "OnLoadedWithJSON")]
    internal static class DataManager_HeatSinkDefLoadRequest_OnLoadedWithJSON_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return DataManagerPatchHelper.ReplaceDefaultConstructor<HeatSinkDef>(instructions);
        }

        public static void Prefix(string json)
        {
            DataManagerPatchHelper.ComponentTypeDescriptor = Registry.GetDescriptorFromJSON(json);
        }

        public static void Postfix(string json)
        {
            DataManagerPatchHelper.ComponentTypeDescriptor = null;
        }
    }
}
