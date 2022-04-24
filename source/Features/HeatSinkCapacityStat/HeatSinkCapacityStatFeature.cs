using System.Linq;
using BattleTech;
using CustomComponents;
using MechEngineer.Features.CriticalEffects;
using MechEngineer.Features.Engines;
using MechEngineer.Features.Engines.Helper;
using MechComponentDefExtensions = MechEngineer.Helper.MechComponentDefExtensions;
using StatCollectionExtension = MechEngineer.Features.Engines.Helper.StatCollectionExtension;

namespace MechEngineer.Features.HeatSinkCapacityStat;

internal class HeatSinkCapacityStatFeature : Feature<HeatSinkCapacityStatSettings>
{
    internal static readonly HeatSinkCapacityStatFeature Shared = new();

    internal override HeatSinkCapacityStatSettings Settings => Control.settings.HeatSinkCapacityStat;

    internal bool IgnoreShutdown(MechComponent mechComponent)
    {
        if (Settings.ShutdownStatusEffectsExcludedComponentTypes.Contains(mechComponent.componentType))
        {
            return true;
        }

        var componentTags = mechComponent.componentDef.ComponentTags;
        return Settings.ShutdownStatusEffectsExcludedComponentTags.Any(componentTags.Contains);
    }

    internal void InitEffectStats(Mech mech)
    {
        // mech.statCollection["HeatSinkCapacity"].OnStatValueChanged +=
        //     statistic => Control.Logger.Debug?.Log($"Changed stat {statistic.name} to {statistic.value} for {mech.Nickname} {mech.GUID}\n{Environment.StackTrace}");

        var core = mech.miscComponents.FirstOrDefault(x => x.componentDef.Is<EngineCoreDef>());
        if (core != null)
        {
            if (core.baseComponentRef.DamageLevel >= ComponentDamageLevel.NonFunctional)
            {
                return;
            }

            var effectData = EngineEffectForMechDef(mech.MechDef);

            EffectIdUtil.CreateEffect(core, effectData, $"{effectData.Description.Id}_{mech.GUID}");
        }
    }

    internal static EffectData EngineEffectForMechDef(MechDef mechDef)
    {
        var engineDissipation = mechDef.GetEngine()?.EngineHeatDissipation
                                ?? EngineFeature.settings.EngineMissingFallbackHeatSinkCapacity;

        var statisticData = StatCollectionExtension
            .HeatSinkCapacity(null)
            .CreateStatisticData(
                StatCollection.StatOperation.Int_Add,
                (int)engineDissipation
            );

        return MechComponentDefExtensions.CreatePassiveEffectData("EngineCoreEffect", statisticData);
    }
}