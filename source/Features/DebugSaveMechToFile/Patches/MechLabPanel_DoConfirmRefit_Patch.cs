using System.IO;
using BattleTech.UI;
using fastJSON;

namespace MechEngineer.Features.DebugSaveMechToFile.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.DoConfirmRefit))]
public static class MechLabPanel_DoConfirmRefit_Patch
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechLabPanel __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        var mechDef = __instance.CreateMechDef();

        var id = $"{mechDef.Description.Name}_{mechDef.Description.Id}";
        var path = Path.Combine(Path.Combine(Control.Mod.Directory, "Saves"), $"{id}.json");
        Directory.CreateDirectory(Directory.GetParent(path).FullName);

        using (var writer = new StreamWriter(path))
        {
            var p = new JSONParameters
            {
                EnableAnonymousTypes = true,
                SerializeToLowerCaseNames = false,
                UseFastGuid = false,
                KVStyleStringDictionary = false,
                SerializeNullValues = false
            };

            var json = JSON.ToNiceJSON(mechDef, p);
            writer.Write(json);
        }
    }
}
