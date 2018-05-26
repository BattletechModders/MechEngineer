using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using Harmony;

namespace MechEngineMod
{
    internal static class EnginePersistence
    {
        internal static void OnCreateInventoryItem(MechLabInventoryWidget widget, MechLabPanel panel, DataManager dataManager, MechComponentRef componentRef)
        {
            var engineRef = componentRef.GetEngineRef();
            if (engineRef == null)
            {
                return;
            }

            if (engineRef.AdditionalSHSCount > 0)
            {
                // add shs back
                var def = dataManager.GetObjectOfType<HeatSinkDef>(Control.GearHeatSinkGenericStandard, BattleTechResourceType.HeatSinkDef);
                widget.CreateInventoryItem(def.CreateRef(panel.sim), false, engineRef.AdditionalSHSCount);
                engineRef.AdditionalSHSCount = 0;
            }

            if (engineRef.AdditionalDHSCount > 0)
            {
                // add dhs back
                var def = dataManager.GetObjectOfType<HeatSinkDef>(Control.GearHeatSinkGenericDouble, BattleTechResourceType.HeatSinkDef);
                widget.CreateInventoryItem(def.CreateRef(panel.sim), false, engineRef.AdditionalDHSCount);
                engineRef.AdditionalDHSCount = 0;
            }

            if (engineRef.IsDHS)
            {
                // add dhs engine kit back
                var def = dataManager.GetObjectOfType<HeatSinkDef>(Control.EngineKitDHS, BattleTechResourceType.HeatSinkDef);
                widget.CreateInventoryItem(def.CreateRef(panel.sim), false, 1);
                engineRef.IsDHS = false;
            }

            SaveEngineState(engineRef, panel);
        }

        internal static void SaveEngineState(EngineRef engineRef, MechLabPanel panel)
        {
            var oldSimGameUID = engineRef.mechComponentRef.SimGameUID;
            var newSimGameUID = engineRef.GetNewSimGameUID();
            if (oldSimGameUID != newSimGameUID)
            {
                engineRef.mechComponentRef.SetSimGameUID(newSimGameUID);
                // the only ones actually relying on SimGameUID are these work orders, so we have to change them
                // hopefully no previous or other working order lists are affected
                ChangeWorkOrderSimGameUID(panel, oldSimGameUID, newSimGameUID);
            }

            //Control.mod.Logger.LogDebug("mechComponentRef SimGameUID=" + mechComponentRef.SimGameUID);
        }

        private static void ChangeWorkOrderSimGameUID(MechLabPanel panel, string oldSimGameUID, string newSimGameUID)
        {
            if (!panel.IsSimGame || panel.baseWorkOrder == null)
            {
                return;
            }

            foreach (var entry in panel.baseWorkOrder.SubEntries)
            {
                var install = entry as WorkOrderEntry_InstallComponent;
                var repair = entry as WorkOrderEntry_RepairComponent;

                Traverse traverse;
                if (install != null)
                {
                    traverse = Traverse.Create(install);
                }
                else if (repair != null)
                {
                    traverse = Traverse.Create(repair);
                }
                else
                {
                    continue;
                }

                traverse = traverse.Property("ComponentSimGameUID");
                var componentSimGameUID = traverse.GetValue<string>();

                if (componentSimGameUID != oldSimGameUID)
                {
                    continue;
                }

                traverse.SetValue(newSimGameUID);
            }
        }
    }
}