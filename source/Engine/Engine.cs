using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using UnityEngine;

namespace MechEngineMod
{
    internal static class Engine
    {
        internal static void InitEffectstats(Mech mech)
        {
            var engine = mech.MechDef.Inventory
                .Select(x => x.GetEngineRef())
                .FirstOrDefault(x => x != null);

            if (engine == null)
            {
                return;
            }

            var tonnage = mech.tonnage;

            float walkSpeed, runSpeed, TTWalkSpeed;
            Control.calc.CalcSpeeds(engine.engineDef, tonnage, out walkSpeed, out runSpeed, out TTWalkSpeed);

            mech.StatCollection.GetStatistic("WalkSpeed").SetValue(walkSpeed);
            mech.StatCollection.GetStatistic("RunSpeed").SetValue(runSpeed);
        }

        internal static bool CalculateMovementStat(MechDef mechDef, ref float currentValue, ref float maxValue)
        {
            var engine = mechDef.Inventory.Select(x => x.GetEngineRef()).FirstOrDefault(x => x != null);
            if (engine == null)
            {
                return true;
            }

            var maxTonnage = mechDef.Chassis.Tonnage;
            //actualy tonnage is never used for anything in combat, hence it will warn you if you take less than the max
            //since we don't want to fiddle around too much, ignore current tonnage
            //var currentTonnage = 0f;
            //MechStatisticsRules.CalculateTonnage(mechDef, ref currentTonnage, ref maxTonnage);
            float walkSpeed, runSpeed, TTwalkSpeed;
            Control.calc.CalcSpeeds(engine.engineDef, maxTonnage, out walkSpeed, out runSpeed, out TTwalkSpeed);

            //used much more familiar TT walk speed values
            //also, user doesn't have to change min/max sprint values depending on if they are using curved movement or not
            //divided by 9 instead of 10 to make scale more reactive at the bottom.
            currentValue = Mathf.Floor(
                (TTwalkSpeed - Control.settings.const_MinTTWalk)
                / (Control.settings.const_MaxTTWalk - Control.settings.const_MinTTWalk)
                * 9f + 1
            );

            currentValue = Mathf.Max(currentValue, 1f);
            maxValue = 10f;
            return false;
        }

        internal static void ChangeInstallationCosts(MechComponentRef mechComponent, WorkOrderEntry_InstallComponent workOrderEntry)
        {
            if (mechComponent == null)
            {
                return;
            }

            var engine = mechComponent.GetEngineRef();
            if (engine == null)
            {
                return;
            }

            workOrderEntry.SetCost(Control.calc.CalcInstallTechCost(engine.engineDef));
        }

        // only allow one engine part per specific location
        internal static void ValidateAdd(
            MechComponentDef newComponentDef,
            List<MechLabItemSlotElement> localInventory,
            ref string dropErrorMessage,
            ref bool result)
        {
            try
            {
                if (!newComponentDef.IsEnginePart())
                {
                    return;
                }

                var existingEngine = localInventory
                    .Where(x => x != null)
                    .Select(x => x.ComponentRef)
                    .FirstOrDefault(x => x != null && x.Def != null && x.Def.IsEnginePart());

                if (existingEngine == null)
                {
                    return;
                }

                dropErrorMessage = String.Format("Cannot add {0}: An engine part is already installed", newComponentDef.Description.Name);
                result = false;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        internal static float AdditionalHeatSinkTonnage(MechDef mechDef)
        {
            var engineRef = mechDef.Inventory.Where(x => x.Def.IsMainEngine()).Select(x => x.GetEngineRef()).FirstOrDefault();
            if (engineRef == null)
            {
                return 0;
            }

            return engineRef.AdditionalHeatSinkCount * 1; // we assume one ton per heat sink
        }

        internal static void ValidationRulesCheck(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            // engine
            var engineRefs = mechDef.Inventory.Where(x => x.Def.IsEnginePart()).ToList();
            var mainEngine = engineRefs
                .Where(x => x.DamageLevel == ComponentDamageLevel.Functional)
                .Select(x => x.GetEngineRef())
                .FirstOrDefault(x => x != null);
            if (mainEngine == null)
            {
                errorMessages[MechValidationType.InvalidInventorySlots].Add("MISSING ENGINE: This Mech must mount a functional Fusion Engine");
                return;
            }

            if (mainEngine.engineDef.Type == EngineDef.EngineType.XL && engineRefs.Count(x => x.DamageLevel == ComponentDamageLevel.Functional) != 3)
            {
                errorMessages[MechValidationType.InvalidInventorySlots].Add("XL ENGINE: Requires XL left and right slots");
            }

            // jump jets
            {
                var currentCount = mechDef.Inventory.Count(c => c.ComponentDefType == ComponentType.JumpJet);
                var maxCount = Control.calc.CalcJumpJetCount(mainEngine.engineDef, mechDef.Chassis.Tonnage);
                if (currentCount > maxCount)
                {
                    errorMessages[MechValidationType.InvalidJumpjets].Add(String.Format("JUMPJETS: This Mech mounts too many jumpjets ({0} out of {1})", currentCount, maxCount));
                }
            }
        }

        internal static void SetJumpJetHardpointCount(MechLabMechInfoWidget widget, MechLabPanel mechLab, MechLabHardpointElement[] hardpoints)
        {
            if (mechLab == null || mechLab.activeMechDef == null || mechLab.activeMechDef.Inventory == null)
            {
                return;
            }

            if (mechLab.activeMechDef.Chassis == null)
            {
                return;
            }

            var engine = mechLab.activeMechInventory
                .Select(x => x.GetEngineRef())
                .FirstOrDefault(x => x != null);

            var current = mechLab.headWidget.currentJumpjetCount
                          + mechLab.centerTorsoWidget.currentJumpjetCount
                          + mechLab.leftTorsoWidget.currentJumpjetCount
                          + mechLab.rightTorsoWidget.currentJumpjetCount
                          + mechLab.leftArmWidget.currentJumpjetCount
                          + mechLab.rightArmWidget.currentJumpjetCount
                          + mechLab.leftLegWidget.currentJumpjetCount
                          + mechLab.rightLegWidget.currentJumpjetCount;

            if (engine == null)
            {
                widget.totalJumpjets = 0;
            }
            else
            {
                widget.totalJumpjets = Control.calc.CalcJumpJetCount(engine.engineDef, mechLab.activeMechDef.Chassis.Tonnage);
            }

            if (hardpoints == null || hardpoints[4] == null)
            {
                return;
            }

            hardpoints[4].SetData(WeaponCategory.AMS, String.Format("{0} / {1}", current, widget.totalJumpjets));
        }

        internal static void RefreshAvailability(MechLabInventoryWidget widget, float tonnage)
        {
            if (tonnage <= 0)
            {
                return;
            }

            foreach (var element in widget.localInventory)
            {
                MechComponentDef componentDef;
                if (element.controller != null)
                {
                    componentDef = element.controller.componentDef;
                }
                else if (element.ComponentRef != null)
                {
                    componentDef = element.ComponentRef.Def;
                }
                else
                {
                    continue;
                }

                var engine = componentDef.GetEngineDef();
                if (engine == null)
                {
                    continue;
                }

                if (Control.calc.CalcAvailability(engine, tonnage))
                {
                    continue;
                }

                element.gameObject.SetActive(false);
            }
        }
    }
}