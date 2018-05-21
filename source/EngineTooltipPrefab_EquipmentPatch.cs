using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using Harmony;

namespace MechEngineMod
{
    [HarmonyPatch(typeof(TooltipPrefab_Equipment), "SetData")]
    public static class EngineTooltipPrefab_EquipmentPatch
    {
        public static List<MechComponentRef> activeMechInventory = null;
        public static MechDef activeMechDef = null;

        // show extended engine information (as its now shown anywhere else)
        public static void Postfix(TooltipPrefab_Equipment __instance, object data)
        {
            try
            {
                if (activeMechDef == null || activeMechInventory == null || data == null)
                {
                    return;
                }

                var mechComponentDef = (MechComponentDef)data;
                var engine = Engine.MainEngineFromDef(mechComponentDef);
                if (engine == null)
                {
                    return;
                }

                var heatDissipation = EngineHeatMechStatisticsRulesPatch.GetEngineHeatDissipation(activeMechInventory.ToArray());

                float walkSpeed, runSpeed, TTwalkSpeed;
                Control.calc.CalcSpeeds(engine, activeMechDef.Chassis.Tonnage, out walkSpeed, out runSpeed, out TTwalkSpeed);

                __instance.bonusesText.text = string.Format(
                    "- {0} Heat / Turn, {1} Top Speed",
                    heatDissipation,
                    runSpeed);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }

    [HarmonyPatch(typeof(MechLabPanel), "LoadMech")]
    public static class EngineMechLabPanelLoadMechPatch
    {
        public static void Postfix(MechLabPanel __instance)
        {
            try
            {
                EngineTooltipPrefab_EquipmentPatch.activeMechDef = __instance.activeMechDef;
                EngineTooltipPrefab_EquipmentPatch.activeMechInventory = __instance.activeMechInventory;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }

    [HarmonyPatch(typeof(MechLabPanel), "Cleanup")]
    public static class EngineMechLabPanelCleanupPatch
    {
        public static void Postfix()
        {
            try
            {
                EngineTooltipPrefab_EquipmentPatch.activeMechDef = null;
                EngineTooltipPrefab_EquipmentPatch.activeMechInventory = null;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }

    [HarmonyPatch(typeof(MechLabPanel), "ClearData")]
    public static class EngineMechLabPanelClearDataPatch
    {
        public static void Postfix()
        {
            try
            {
                EngineTooltipPrefab_EquipmentPatch.activeMechDef = null;
                EngineTooltipPrefab_EquipmentPatch.activeMechInventory = null;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}
