using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BattleTech;
using CustomComponents;
using MechEngineer.Features.ArmorMaximizer;
using MechEngineer.Features.ArmorMaximizer.Maximizer;
using MechEngineer.Features.ArmorStructureRatio;
using MechEngineer.Features.DynamicSlots;
using MechEngineer.Features.Engines;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Features.OverrideTonnage;
using MechEngineer.Helper;
using MechEngineer.Misc;

namespace MechEngineer.Features.AutoFix;

internal class AutoFixer : IAutoFixMechDef
{
    internal static readonly AutoFixer Shared = new();

    public void AutoFix(List<MechDef> mechDefs, SimGameState? simGameState)
    {
        // we dont fix save games anymore, have to have money and time to fix an ongoing campaign
        if (simGameState != null)
        {
            return;
        }

        foreach (var mechDef in mechDefs)
        {
            try
            {
                AutoFixMechDef(mechDef);
            }
            catch (Exception e)
            {
                Log.Main.Error?.Log(e);
            }
        }
    }

    public void AutoFixMechDef(MechDef mechDef)
    {
        if (!AutoFixerFeature.settings.MechDefEngine)
        {
            return;
        }

        if (mechDef.IgnoreAutofix())
        {
            return;
        }

        if (!AutoFixerFeature.settings.MechTagsAutoFixEnabled.Any(mechDef.MechTags.Contains))
        {
            return;
        }

        Log.Main.Info?.Log($"Auto fixing mechDef={mechDef.Description.Id} chassisDef={mechDef.Chassis.Description.Id}");

        MechDefBuilder builder;
        {
            var inventory = mechDef.Inventory.ToList();
            if (Log.Main.Debug != null)
            {
                foreach (var cref in inventory)
                {
                    var def = cref.Def;
                    Log.Main.Debug.Log($" {cref.ComponentDefID}{(cref.IsFixed ? " (fixed)" : "")} at {cref.MountedLocation} tonnage={def.Tonnage}");
                }
            }

            builder = new MechDefBuilder(mechDef.Chassis, inventory);
        }

        var dataManager = mechDef.DataManager;

        if (AutoFixerFeature.settings.ArmActuatorAdder.Enabled)
        {
            bool CheckAndAdd(ChassisLocations location, AutoFixerSettings.ArmActuatorAdderSettings.ActuatorSettings settings)
            {
                var tagLimit = location == ChassisLocations.LeftArm ? settings.TagLimitLeft : settings.TagLimitRight;
                if (mechDef.Chassis.ChassisTags.Contains(tagLimit))
                {
                    return false;
                }
                if (builder.Inventory.Any(x => x.MountedLocation == location && x.GetCategory(settings.CategoryId) != null))
                {
                    return true;
                }
                var def = dataManager.UpgradeDefs.Get(settings.DefId);
                return builder.Add(def, location) != null;
            }

            void CheckArm(ChassisLocations location)
            {
                if (CheckAndAdd(location, AutoFixerFeature.settings.ArmActuatorAdder.Lower))
                {
                    CheckAndAdd(location, AutoFixerFeature.settings.ArmActuatorAdder.Hand);
                }
            }

            CheckArm(ChassisLocations.LeftArm);
            CheckArm(ChassisLocations.RightArm);

            mechDef.SetInventory(builder.Inventory.ToArray());
        }

        ArmorStructureRatioFeature.Shared.AutoFixMechDef(mechDef);

        var res = EngineSearcher.SearchInventory(builder.Inventory);
        if (res.CoolingDef == null)
        {
            throw new NullReferenceException("No CoolingDef found");
        }
        if (res.HeatBlockDef == null)
        {
            throw new NullReferenceException("No HeatBlockDef found");
        }

        var engineHeatSinkDef = dataManager.HeatSinkDefs.Get(res.CoolingDef.HeatSinkDefId).GetComponent<EngineHeatSinkDef>();

        if (!EngineFeature.settings.KeepIncompatibleHeatSinks)
        {
            // remove incompatible heat sinks
            var incompatibleHeatSinks = builder.Inventory
                .Where(r => r.Def.Is<EngineHeatSinkDef>(out var hs) && hs != engineHeatSinkDef)
                .ToList();

            foreach (var incompatibleHeatSink in incompatibleHeatSinks)
            {
                builder.Remove(incompatibleHeatSink);
                builder.Add(engineHeatSinkDef.Def, ChassisLocations.Head, true);
                Log.Main.Debug?.Log($" Converted incompatible heat sinks to compatible ones");
            }
        }

        Engine? engine = null;
        if (res.CoreDef != null)
        {
            Log.Main.Debug?.Log($" Found an existing engine");
            engine = new Engine(res.CoolingDef, res.HeatBlockDef, res.CoreDef, res.WeightFactors, new List<MechComponentRef>());

            // convert external heat sinks into internal ones
            // TODO only to make space if needed, drop the rest of the heat sinks

            if (AutoFixerFeature.settings.InternalizeHeatSinksOnValidEngines)
            {
                var max = engine.HeatSinkInternalAdditionalMaxCount;
                var oldCurrent = engine.HeatBlockDef.HeatSinkCount;
                var current = oldCurrent;

                var heatSinks = builder.Inventory
                    .Where(r => r.Def.Is<EngineHeatSinkDef>())
                    .ToList();

                while (current < max && heatSinks.Count > 0)
                {
                    var component = heatSinks[0];
                    heatSinks.RemoveAt(0);
                    builder.Remove(component);
                    current++;
                }

                if (current > 0)
                {
                    var heatBlock = builder.Inventory.FirstOrDefault(r => r.Def.Is<EngineHeatBlockDef>());
                    if (heatBlock != null)
                    {
                        builder.Remove(heatBlock);
                    }

                    var heatBlockDefId = $"{AutoFixerFeature.settings.MechDefHeatBlockDef}_{current}";
                    var def = dataManager.HeatSinkDefs.Get(heatBlockDefId);
                    builder.Add(def, ChassisLocations.CenterTorso, true);

                    Log.Main.Debug?.Log($" Converted external heat sinks ({current - oldCurrent}) to internal ones (to make space)");
                }
            }
        }
        else
        {
            Log.Main.Debug?.Log(" Finding engine");
            var freeTonnage = CalculateFreeTonnage(mechDef);

            var jumpJets = builder.Inventory.Where(x => x.ComponentDefType == ComponentType.JumpJet).ToList();
            var jumpJetTonnage = jumpJets.Select(x => x.Def.Tonnage).FirstOrDefault(); //0 if no jjs

            var externalHeatSinks = builder.Inventory
                .Where(r => r.Def.Is<EngineHeatSinkDef>())
                .ToList();
            var internalHeatSinksCount = res.HeatBlockDef.HeatSinkCount;

            var engineCandidates = new List<Engine>();

            var engineCoreDefs = dataManager.HeatSinkDefs
                .Select(hs => hs.Value)
                .Select(hs => hs.GetComponent<EngineCoreDef>())
                .Where(c => c != null)
                .OrderByDescending(x => x.Rating);

            var removedExternalHeatSinksOverUse = false;

            foreach (var coreDef in engineCoreDefs)
            {
                {
                    // remove superfluous jump jets
                    var maxJetCount = coreDef.GetMovement(mechDef.Chassis.Tonnage).JumpJetCount;
                    while (jumpJets.Count > maxJetCount)
                    {
                        var lastIndex = jumpJets.Count - 1;
                        var jumpJet = jumpJets[lastIndex];
                        freeTonnage += jumpJet.Def.Tonnage;
                        builder.Remove(jumpJet);
                        jumpJets.Remove(jumpJet);

                        Log.Main.Trace?.Log("  Removed JumpJet");
                    }
                }

                {
                    var candidate = new Engine(res.CoolingDef, res.HeatBlockDef, coreDef, res.WeightFactors, externalHeatSinks, false);

                    Log.Main.Trace?.Log($"  candidate id={coreDef.Def.Description.Id} TotalTonnage={candidate.TotalTonnage}");

                    engineCandidates.Add(candidate);

                    var internalHeatSinksMax = candidate.HeatSinkInternalAdditionalMaxCount;

                    // convert external ones to internal ones
                    while (internalHeatSinksCount < internalHeatSinksMax && externalHeatSinks.Count > 0)
                    {
                        var component = externalHeatSinks[0];
                        externalHeatSinks.RemoveAt(0);
                        builder.Remove(component);
                        internalHeatSinksCount++;

                        Log.Main.Trace?.Log("  ~Converted external heat sinks to internal ones");
                    }

                    // this only runs on the engine that takes the most heat sinks (since this is in a for loop with rating descending order)
                    // that way we only remove external heat sinks that couldn't be moved internally
                    while (!removedExternalHeatSinksOverUse && externalHeatSinks.Count > 0)
                    {
                        var component = externalHeatSinks[0];
                        externalHeatSinks.RemoveAt(0);
                        builder.Remove(component);
                        var newComponent = builder.Add(component.Def);
                        if (newComponent == null)
                        {
                            Log.Main.Trace?.Log("  Removed external heat sink that doesn't fit");
                            // might still need to remove some
                            continue;
                        }
                        // addition worked
                        externalHeatSinks.Add(newComponent);
                        break;
                    }
                    removedExternalHeatSinksOverUse = true;

                    // convert internal ones to external ones
                    while (internalHeatSinksCount > internalHeatSinksMax)
                    {
                        var externalHeatSink = builder.Add(engineHeatSinkDef.Def);
                        if (externalHeatSink == null)
                        {
                            Log.Main.Trace?.Log("  ~Dropped external when converting from internal");
                            freeTonnage++;
                        }
                        else
                        {
                            externalHeatSinks.Add(externalHeatSink);
                            Log.Main.Trace?.Log("  ~Converted internal heat sink to external one");
                        }
                        internalHeatSinksCount--;
                    }

                    candidate.HeatSinksExternal = new List<MechComponentRef>(externalHeatSinks);
                    candidate.CalculateStats();

                    // remove candidates that make no sense anymore
                    // TODO not perfect and maybe too large for small mechs
                    engineCandidates = engineCandidates.Where(x => PrecisionUtils.SmallerOrEqualsTo(x.TotalTonnage, freeTonnage + 6 * engineHeatSinkDef.Def.Tonnage + jumpJetTonnage)).ToList();
                }

                // go through all candidates, larger first
                engine = engineCandidates.FirstOrDefault(candidate => PrecisionUtils.SmallerOrEqualsTo(candidate.TotalTonnage, freeTonnage));

                if (engine != null)
                {
                    break;
                }
            }

            if (engine != null)
            {
                Log.Main.Debug?.Log($" engine={engine.CoreDef} freeTonnage={freeTonnage}");
                var dummyCore = builder.Inventory.FirstOrDefault(r => r.ComponentDefID == AutoFixerFeature.settings.MechDefCoreDummy);
                if (dummyCore != null)
                {
                    builder.Remove(dummyCore);
                }
                builder.Add(engine.CoreDef.Def, ChassisLocations.CenterTorso, true);

                // convert internal heat sinks back as external ones if the mech can fit it
                while (internalHeatSinksCount > 0 && builder.Add(engineHeatSinkDef.Def) != null)
                {
                    internalHeatSinksCount--;
                }

                if (internalHeatSinksCount > 0)
                {
                    var heatBlock = builder.Inventory.FirstOrDefault(r => r.Def.Is<EngineHeatBlockDef>());
                    if (heatBlock != null)
                    {
                        builder.Remove(heatBlock);
                    }

                    var heatBlockDefId = $"{AutoFixerFeature.settings.MechDefHeatBlockDef}_{internalHeatSinksCount}";
                    var def = dataManager.HeatSinkDefs.Get(heatBlockDefId);
                    builder.Add(def, ChassisLocations.CenterTorso, true);
                }
            }
        }

        if (engine == null)
        {
            return;
        }

        // add free heat sinks
        {
            var max = engine.HeatSinkExternalFreeMaxCount;
            var current = builder.Inventory.Count(r => r.Def.Is<EngineHeatSinkDef>());
            for (var i = current; i < max; i++)
            {
                builder.Add(engineHeatSinkDef.Def, ChassisLocations.Head, true);
            }
        }

        // find any overused location
        if (builder.HasOveruseAtAnyLocation())
        {
            Log.Main.Info?.Log($" Overuse found");
            // heatsinks, upgrades
            var itemsToBeReordered = builder.Inventory
                .Where(IsMovable)
                .OrderBy(c => MechDefBuilder.LocationCount(c.Def.AllowedLocations))
                .ThenByDescending(c => c.Def.InventorySize)
                .ToList();

            // remove all items that can be reordered: heatsinks, upgrades
            foreach (var item in itemsToBeReordered)
            {
                builder.Remove(item);
            }

            // then add most restricting, and then largest items first (probably double head sinks)
            foreach (var item in itemsToBeReordered)
            {
                if (builder.Add(item.Def) == null)
                {
                    Log.Main.Warning?.Log($" Component {item.ComponentDefID} from {item.MountedLocation} can't be re-added");
                }
                else
                {
                    Log.Main.Debug?.Log($"  Component {item.ComponentDefID} re-added");
                }
            }
        }

        mechDef.SetInventory(builder.Inventory.ToArray());

        {
            var freeTonnage = CalculateFreeTonnage(mechDef);
            if (freeTonnage < 0)
            {
                Log.Main.Debug?.Log($" Found over tonnage {-freeTonnage}");
                var removableItems = builder.Inventory
                    .Where(IsRemovable)
                    .OrderBy(c => c.Def.Tonnage)
                    .ThenByDescending(c => c.Def.InventorySize)
                    .ThenByDescending(c => c.ComponentDefType switch
                    {
                        ComponentType.HeatSink => 2,
                        ComponentType.JumpJet => 1,
                        _ => 0
                    })
                    .ToList();

                while (removableItems.Count > 0 && PrecisionUtils.SmallerThan(freeTonnage, 0))
                {
                    var item = removableItems[0];
                    removableItems.RemoveAt(0);
                    freeTonnage += item.Def.Tonnage;
                    builder.Remove(item);
                    Log.Main.Debug?.Log($"  Removed item, freeTonnage={freeTonnage}");
                }

                mechDef.SetInventory(builder.Inventory.ToArray());
            }
        }

        if (AutoFixerFeature.Shared.Settings.MaximizeArmor)
        {
            sw.Start();
            if (MechArmorState.Maximize(mechDef, true, 5, out var updates))
            {
                foreach (var update in updates)
                {
                    var chassisLocation = MechStructureRules.GetChassisLocationFromArmorLocation(update.Location);
                    var locationDef = mechDef.GetLocationLoadoutDef(chassisLocation);
                    if (update.Location.IsRear())
                    {
                        locationDef.CurrentRearArmor = locationDef.AssignedRearArmor = update.Assigned;
                    }
                    else
                    {
                        locationDef.CurrentArmor = locationDef.AssignedArmor = update.Assigned;
                    }
                    Log.Main.Trace?.Log($"OnMaxArmor.SetArmor update={update}");
                }
            }
            sw.Stop();
            Log.Main.Debug?.Log($"Autofixer MechArmorState.Maximize took {sw.Elapsed}");
        }
    }

    private static readonly Stopwatch sw = new();

    private static float CalculateFreeTonnage(MechDef mechDef)
    {
        var weights = new Weights(mechDef);
        Log.Main.Debug?.Log($"Chassis={mechDef.Chassis.Description.Id} weights={weights}");
        return weights.FreeWeight;
    }

    private static bool IsMovable(MechComponentRef c)
    {
        if (!IsRemovable(c))
        {
            return false;
        }

        var def = c.Def;

        // items in arms and legs are usually bound to a certain side, so lets ignore them from relocation
        if (MechDefBuilder.LocationCount(def.AllowedLocations) <= 2)
        {
            return false;
        }

        //!TODO PONE FIX IT
        //if (def.Is<Category>(out var category) && category.CategoryDescriptor.UniqueForLocation)
        //{
        //    return false;
        //}

        return true;
    }

    private static bool IsRemovable(MechComponentRef c)
    {
        var def = c.Def;

        if (def == null)
        {
            return false;
        }

        if (c.IsFixed)
        {
            return false;
        }

        //!TODO PONE FIX IT
        //if (c.Def.Is<Category>(out var category) && category.CategoryDescriptor.Required)
        //{
        //    return false;
        //}

        return def.ComponentType is ComponentType.HeatSink or ComponentType.JumpJet;
    }
}
