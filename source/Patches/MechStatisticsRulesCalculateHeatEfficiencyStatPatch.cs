using System;
using System.Linq;
using BattleTech;
using Harmony;
using UnityEngine;

namespace MechEngineMod
{
    // caluclate heat efficency and support engine heat dissipation
    [HarmonyPatch(typeof(MechStatisticsRules), "CalculateHeatEfficiencyStat")]
    public static class MechStatisticsRulesCalculateHeatEfficiencyStatPatch
    {

        public static bool Prefix(MechDef mechDef, ref float currentValue, ref float maxValue)
        {
            try
            {
                return CalculateHeatEfficiencyStat(mechDef, ref currentValue, ref maxValue);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
                return true;
            }
        }

        private static float AdditionalHeatSinkDissipation(MechDef mechDef)
        {
            var engineHeatSinkDissipation = EngineHeat.GetEngineHeatDissipation(mechDef.Inventory);
            //Control.mod.Logger.LogDebug("engineHeatSinkDissipation=" + engineHeatSinkDissipation);
            return engineHeatSinkDissipation;
        }

        private static bool CalculateHeatEfficiencyStat(MechDef mechDef, ref float currentValue, ref float maxValue)
        {
            var totalHeatSinkDissipation = Control.Combat.Heat.InternalHeatSinkCount * Control.Combat.Heat.DefaultHeatSinkDissipationCapacity;
            var heatGenerationWeapons = 0f;
            var numberOfJumpJets = 0;

            foreach (var mechComponentRef in mechDef.Inventory)
            {
                if (mechComponentRef.Def == null)
                {
                    mechComponentRef.RefreshComponentDef();
                }
                if (mechComponentRef.Def is WeaponDef)
                {
                    var weaponDef = (WeaponDef)mechComponentRef.Def;
                    heatGenerationWeapons += weaponDef.HeatGenerated;
                }
                else if (mechComponentRef.ComponentDefType == ComponentType.JumpJet)
                {
                    if (mechComponentRef.DamageLevel < ComponentDamageLevel.NonFunctional)
                    {
                        numberOfJumpJets++;
                    }
                }
                else if (mechComponentRef.Def is HeatSinkDef)
                {
                    var heatSinkDef = (HeatSinkDef)mechComponentRef.Def;
                    totalHeatSinkDissipation += heatSinkDef.DissipationCapacity;
                }
            }
            
            totalHeatSinkDissipation += AdditionalHeatSinkDissipation(mechDef);

            //Control.mod.Logger.LogDebug("heatGenerationWeapons=" + heatGenerationWeapons);
            //Control.mod.Logger.LogDebug("totalHeatSinkDissipation=" + totalHeatSinkDissipation);

            var maxHeat = Control.Combat.Heat.MaxHeat;
            {
                var stats = new StatCollection();
                var maxHeatStatistic = stats.AddStatistic("MaxHeat", maxHeat);
                var heatGeneratedStatistic = stats.AddStatistic("HeatGenerated", heatGenerationWeapons);

                foreach (var mechComponentRef in mechDef.Inventory)
                {
                    if (mechComponentRef.Def == null || mechComponentRef.Def.statusEffects == null)
                    {
                        continue;
                    }

                    var statusEffects = mechComponentRef.Def.statusEffects;
                    foreach (var effect in statusEffects)
                    {
                        switch (effect.statisticData.statName)
                        {
                            case "MaxHeat":
                                stats.PerformOperation(maxHeatStatistic, effect.statisticData);
                                break;
                            case "HeatGenerated":
                                if (effect.statisticData.targetCollection == StatisticEffectData.TargetCollection.Weapon)
                                {
                                    stats.PerformOperation(heatGeneratedStatistic, effect.statisticData);
                                }
                                break;
                        }
                    }
                }

                maxHeat = maxHeatStatistic.CurrentValue.Value<int>();
                heatGenerationWeapons = heatGeneratedStatistic.CurrentValue.Value<float>();
            }

            //Control.mod.Logger.LogDebug("maxHeat=" + maxHeat);
            //Control.mod.Logger.LogDebug("heatGenerationWeapons=" + heatGenerationWeapons);

            if (numberOfJumpJets >= Control.Combat.MoveConstants.MoveTable.Length)
            {
                numberOfJumpJets = Control.Combat.MoveConstants.MoveTable.Length - 1;
            }

            var heatGenerationJumpJets = 0f;
            var jumpHeatDivisor = 3;
            if (numberOfJumpJets > 0)
            {
                heatGenerationJumpJets += numberOfJumpJets * Control.Combat.Heat.JumpHeatUnitSize / jumpHeatDivisor;
            }
            else
            {
                heatGenerationJumpJets = 0f;
            }

            //Control.mod.Logger.LogDebug("heatGenerationJumpJets=" + heatGenerationJumpJets);

            totalHeatSinkDissipation *= Control.Combat.Heat.GlobalHeatSinkMultiplier;
            var totalHeatGeneration = (heatGenerationWeapons + heatGenerationJumpJets) * Control.Combat.Heat.GlobalHeatIncreaseMultiplier;

            //Control.mod.Logger.LogDebug("totalHeatGeneration=" + totalHeatGeneration);

            // rounding steps for heatSinkDissipation
            var heatDissipationPercent = Mathf.Min(totalHeatSinkDissipation / totalHeatGeneration * 100f, UnityGameInstance.BattleTechGame.MechStatisticsConstants.MaxHeatEfficiency);
            heatDissipationPercent = Mathf.Max(heatDissipationPercent, UnityGameInstance.BattleTechGame.MechStatisticsConstants.MinHeatEfficiency);

            //Control.mod.Logger.LogDebug("heatDissipationPercent=" + heatDissipationPercent);

            totalHeatSinkDissipation = totalHeatGeneration * (heatDissipationPercent / 100f);

            //Control.mod.Logger.LogDebug("totalHeatSinkDissipation=" + totalHeatSinkDissipation);

            var heatLeftOver = totalHeatGeneration - totalHeatSinkDissipation;
            var unusedHeatCapacity = maxHeat - heatLeftOver;

            //Control.mod.Logger.LogDebug("heatLeftOver=" + heatLeftOver);
            //Control.mod.Logger.LogDebug("unusedHeatCapacity=" + unusedHeatCapacity);

            currentValue = Mathf.Round((unusedHeatCapacity / maxHeat) * 10f);
            currentValue = Mathf.Max(Mathf.Min(currentValue, 10f), 1f);
            maxValue = 10f;
            return false;
        }
    }
}