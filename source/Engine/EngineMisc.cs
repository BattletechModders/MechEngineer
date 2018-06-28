using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;
using UnityEngine;

namespace MechEngineer
{
    internal static class EngineMisc
    {
        internal static void InitEffectstats(Mech mech)
        {
            var engine = mech.MechDef.Inventory.GetEngineCoreRef();

            if (engine == null)
            {
                return;
            }

            var tonnage = mech.tonnage;

            float walkSpeed, runSpeed;
            Control.calc.CalcSpeeds(engine.CoreDef, tonnage, out walkSpeed, out runSpeed);

            mech.StatCollection.GetStatistic("WalkSpeed").SetValue(walkSpeed);
            mech.StatCollection.GetStatistic("RunSpeed").SetValue(runSpeed);
        }

        internal static bool CalculateMovementStat(MechDef mechDef, ref float walkSpeed, ref float runSpeed, ref float TTWalkSpeed)
        {
            var engine = mechDef.Inventory.GetEngineCoreRef();
            if (engine == null)
            {
                return false;
            }

            var maxTonnage = mechDef.Chassis.Tonnage;
            Control.calc.CalcSpeeds(engine.CoreDef, maxTonnage, out walkSpeed, out runSpeed);
            TTWalkSpeed = Control.calc.CalcMovementPoints(engine.CoreDef, maxTonnage);

            return true;
        }

        internal static void ChangeInstallationCosts(MechComponentRef mechComponent, WorkOrderEntry_InstallComponent workOrderEntry)
        {
            if (mechComponent == null)
            {
                return;
            }

            // removing is always fast and cheap
            if (workOrderEntry.DesiredLocation == ChassisLocations.None)
            {
                return;
            }

            var engine = mechComponent.GetEngineCoreRef(null);
            if (engine == null)
            {
                return;
            }

            var cbillCost = workOrderEntry.GetCBillCost();
            var techCost = workOrderEntry.GetCost();

            Control.calc.CalcInstallCosts(engine.CoreDef, ref cbillCost, ref techCost);

            var traverse = Traverse.Create(workOrderEntry);
            traverse.Field("CBillCost").SetValue(cbillCost);
            traverse.Field("Cost").SetValue(techCost);
        }

        // only allow one engine slot per specific location
        internal static void EngineSlotsValidateAdd(
            MechComponentDef newComponentDef,
            List<MechLabItemSlotElement> localInventory,
            ref string dropErrorMessage,
            ref bool result)
        {
            try
            {
                if (!newComponentDef.IsEngineSlots())
                {
                    return;
                }

                var existingEngine = localInventory
                    .Where(x => x != null)
                    .Select(x => x.ComponentRef)
                    .FirstOrDefault(x => x != null && x.Def != null && x.Def.IsEngineSlots());

                if (existingEngine == null)
                {
                    return;
                }

                dropErrorMessage = string.Format("Cannot add {0}: Engine shielding is already installed", newComponentDef.Description.Name);
                result = false;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        // only allow one engine core
        internal static void EngineCoreValidateAdd(
            MechComponentDef newComponentDef,
            List<MechLabItemSlotElement> localInventory,
            ref string dropErrorMessage,
            ref bool result)
        {
            try
            {
                if (!newComponentDef.IsEngineCore())
                {
                    return;
                }

                var existingEngine = localInventory.Select(c => c.ComponentRef).GetEngineCoreRef();
                if (existingEngine == null)
                {
                    return;
                }

                dropErrorMessage = string.Format("Cannot add {0}: Engine core is already installed", newComponentDef.Description.Name);
                result = false;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }

        internal static float TonnageChanges(MechDef mechDef)
        {
            var engineCoreRef = mechDef.GetEngineCoreRef();
            if (engineCoreRef == null)
            {
                return 0;
            }

            return engineCoreRef.TonnageChanges;
        }

        internal static void ValidationRulesCheck(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            // engine
            var engineRefs = mechDef.Inventory.Where(x => x.Def.IsEnginePart()).ToList();
            var mainEngine = engineRefs.Where(x => x.DamageLevel == ComponentDamageLevel.Functional).GetEngineCoreRef();

            if (mainEngine == null || mainEngine.Type == null)
            {
                return;
            }

            var requirements = mainEngine.Type.Requirements;

            if (engineRefs.Where(x => x.DamageLevel == ComponentDamageLevel.Functional).Select(c => c.ComponentDefID).Intersect(requirements).Count() != requirements.Length)
            {
                var engineName = mainEngine.CoreDef.Def.Description.UIName.ToUpper();
                errorMessages[MechValidationType.InvalidInventorySlots].Add(engineName + ": Requires left and right torso slots");
            }

            // jump jets
            {
                var currentCount = mechDef.Inventory.Count(c => c.ComponentDefType == ComponentType.JumpJet);
                var maxCount = Control.calc.CalcJumpJetCount(mainEngine.CoreDef, mechDef.Chassis.Tonnage);
                if (currentCount > maxCount)
                {
                    errorMessages[MechValidationType.InvalidJumpjets].Add(string.Format("JUMPJETS: This Mech mounts too many jumpjets ({0} out of {1})", currentCount, maxCount));
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

            var engine = mechLab.activeMechInventory.GetEngineCoreRef();

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
                widget.totalJumpjets = Control.calc.CalcJumpJetCount(engine.CoreDef, mechLab.activeMechDef.Chassis.Tonnage);
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

                var engine = componentDef.GetEngineCoreDef();
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


        internal static void AddEngineIfPossible(MechDef mechDef, float originalTotalTonnage)
        {
            if (!Control.settings.AutoFixMechDefEngine)
            {
                return;
            }

            if (mechDef.Inventory.GetEngineCoreRef() != null)
            {
                return;
            }

            float freeTonnage;
            {
                float currentTotalTonnage = 0, maxValue = 0;
                MechStatisticsRules.CalculateTonnage(mechDef, ref currentTotalTonnage, ref maxValue);
                
                var originalInitialTonnage = Chassis.GetOriginalTonnage(mechDef.Chassis);
                if (originalInitialTonnage.HasValue) // either use the freed up tonnage from the initial tonnage fix
                {
                    freeTonnage = originalInitialTonnage.Value - mechDef.Chassis.InitialTonnage;
                    freeTonnage -= currentTotalTonnage - originalTotalTonnage;
                }
                
                else // or use up available total tonnage
                {
                    freeTonnage = mechDef.Chassis.Tonnage - currentTotalTonnage;
                }
            }

            var maxEngine = (EngineCoreDef) null;

            //Control.mod.Logger.LogDebug("C maxEngineTonnage=" + maxEngineTonnage);

            foreach (var keyvalue in mechDef.DataManager.HeatSinkDefs)
            {
                var heatSinkDef = keyvalue.Value;

                if (heatSinkDef.Tonnage > freeTonnage)
                {
                    continue;
                }

                var engineDef = heatSinkDef.GetEngineCoreDef();
                if (engineDef == null)
                {
                    continue;
                }

                if (maxEngine != null && maxEngine.Rating >= engineDef.Rating)
                {
                    continue;
                }

                maxEngine = engineDef;
            }

            //Control.mod.Logger.LogDebug("D maxEngine=" + maxEngine);

            if (maxEngine == null)
            {
                return;
            }

            var componentRefs = new List<MechComponentRef>(mechDef.Inventory);

            { // remove superfluous jump jets
                var maxJetCount = Control.calc.CalcJumpJetCount(maxEngine, mechDef.Chassis.Tonnage);
                var jumpJetList = componentRefs.Where(x => x.ComponentDefType == ComponentType.JumpJet).ToList();
                for (var i = 0; i < jumpJetList.Count - maxJetCount; i++)
                {
                    componentRefs.Remove(jumpJetList[i]);
                }
            }

            { // add engine core
                var isDHS = componentRefs.Select(c => c.Def as HeatSinkDef).Where(c => c != null).Any(c => c.IsDouble());

                var simGameUID = isDHS ? "/ihstype=dhs" : null;

                var componentRef = new MechComponentRef(maxEngine.Def.Description.Id, simGameUID, maxEngine.Def.ComponentType, ChassisLocations.CenterTorso);
                componentRefs.Add(componentRef);
            }

            { // add standard shielding
                var engineType = Control.settings.EngineTypes.First();
                var componentRef = new MechComponentRef(engineType.ComponentTypeID, null, ComponentType.HeatSink, ChassisLocations.CenterTorso);
                componentRefs.Add(componentRef);
            }

            mechDef.SetInventory(componentRefs.ToArray());
        }
    }
}