using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BattleTech.UI;
using fastJSON;
using Harmony;

namespace MechEngineer.Features.SaveMechToFile
{
    [HarmonyPatch(typeof(MechLabPanel), "DoConfirmRefit")]
    public static class MechLabPanel_DoConfirmRefit_Patch
    {
        public static void Prefix(MechLabPanel __instance)
        {
            try
            {
                if (!Control.settings.SaveMechDefOnMechLabConfirm)
                {
                    return;
                }

                var mechDef = __instance.activeMechDef;

                var id = $"{mechDef.Description.Name}_{mechDef.Description.Id}";
                var path = Path.Combine(Path.Combine(Control.mod.Directory, "Saves"), $"{id}.json");
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
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}
