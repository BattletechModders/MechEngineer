using System;
using System.Linq;
using BattleTech;
using Harmony;
using UnityEngine;

namespace MechEngineMod
{
    // caluclate heat efficency and support engine heat dissipation
    [HarmonyPriority(500)]
    [HarmonyPatch(typeof(MechStatisticsRules), "CalculateHeatEfficiencyStat")]
    public static class EngineHeatMechStatisticsRulesPatch
    {
        private static CombatGameConstants Combat;

        public static bool Prefix(MechDef mechDef, ref float currentValue, ref float maxValue)
        {
            try
            {
                if (Combat == null)
                {
                    Combat = CombatGameConstants.CreateFromSaved(UnityGameInstance.BattleTechGame);
                }
                var totalHeatSinkDissipation = Combat.Heat.InternalHeatSinkCount * Combat.Heat.DefaultHeatSinkDissipationCapacity;
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

                totalHeatSinkDissipation += GetHeatDissipation(mechDef);

                Control.mod.Logger.LogDebug("heatGenerationWeapons=" + heatGenerationWeapons);
                Control.mod.Logger.LogDebug("totalHeatSinkDissipation=" + totalHeatSinkDissipation);

                var maxHeat = Combat.Heat.MaxHeat;
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

                Control.mod.Logger.LogDebug("maxHeat=" + maxHeat);
                Control.mod.Logger.LogDebug("heatGenerationWeapons=" + heatGenerationWeapons);

                if (numberOfJumpJets >= Combat.MoveConstants.MoveTable.Length)
                {
                    numberOfJumpJets = Combat.MoveConstants.MoveTable.Length - 1;
                }

                var heatGenerationJumpJets = 0f;
                var jumpHeatDivisor = 3;
                if (numberOfJumpJets > 0)
                {
                    heatGenerationJumpJets += numberOfJumpJets * Combat.Heat.JumpHeatUnitSize / jumpHeatDivisor;
                }
                else
                {
                    heatGenerationJumpJets = 0f;
                }

                totalHeatSinkDissipation *= Combat.Heat.GlobalHeatSinkMultiplier;
                var totalHeatGeneration = (heatGenerationWeapons + heatGenerationJumpJets) * Combat.Heat.GlobalHeatIncreaseMultiplier;

                Control.mod.Logger.LogDebug("totalHeatGeneration=" + totalHeatGeneration);

                // rounding steps for heatSinkDissipation
                var heatDissipationPercent = Mathf.Min(totalHeatSinkDissipation / totalHeatGeneration * 100f, UnityGameInstance.BattleTechGame.MechStatisticsConstants.MaxHeatEfficiency);
                heatDissipationPercent = Mathf.Max(heatDissipationPercent, UnityGameInstance.BattleTechGame.MechStatisticsConstants.MinHeatEfficiency);

                Control.mod.Logger.LogDebug("heatDissipationPercent=" + heatDissipationPercent);

                totalHeatSinkDissipation = totalHeatGeneration * (heatDissipationPercent / 100f);

                Control.mod.Logger.LogDebug("totalHeatSinkDissipation=" + totalHeatSinkDissipation);

                var heatLeftOver = totalHeatGeneration - totalHeatSinkDissipation;
                var unusedHeatCapacity = maxHeat - heatLeftOver;

                Control.mod.Logger.LogDebug("heatLeftOver=" + heatLeftOver);
                Control.mod.Logger.LogDebug("unusedHeatCapacity=" + unusedHeatCapacity);

                currentValue = Mathf.Round((unusedHeatCapacity / maxHeat) * 10f);
                currentValue = Mathf.Max(Mathf.Min(currentValue, 10f), 1f);
                maxValue = 10f;
                return false;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
                return true;
            }
        }

        internal static void PerformOperation(this StatCollection collection, Statistic statistic, StatisticEffectData data)
        {
            var type = Type.GetType(data.modType);
            var variant = new Variant(type);
            variant.SetValue(data.modValue);
            variant.statName = data.statName;
            collection.PerformOperation(statistic, data.operation, variant);
        }

        internal static float GetHeatDissipation(MechDef mechDef)
        {
            var engine = mechDef.Inventory
                .Select(x => Engine.MainEngineFromDef(x.Def))
                .FirstOrDefault(x => x != null);

            if (engine == null)
            {
                return Control.settings.FallbackHeatSinkCapacity;
            }

            var heatSink = mechDef.Inventory
                .Where(c => c.ComponentDefType == ComponentType.HeatSink)
                .Select(c => c.Def as HeatSinkDef)
                .Where(c => c != null)
                .FirstOrDefault(cd => cd.IsDouble() || cd.IsSingle());

            if (heatSink == null)
            {
                return Control.settings.FallbackHeatSinkCapacity;
            }

            var heatsinks = Control.calc.CalcHeatSinks(engine);
            Control.mod.Logger.LogDebug("GetHeatDissipation rating=" + engine.Rating + " heatsinks=" + heatsinks);

            return heatsinks * heatSink.DissipationCapacity;
        }
    }
}