using System.Linq;
using BattleTech;
using CustomComponents;
using MechEngineer.Features.CriticalEffects;
using MechEngineer.Features.Engines;
using MechEngineer.Features.Engines.Helper;
using StatCollectionExtension = MechEngineer.Features.Engines.Helper.StatCollectionExtension;

namespace MechEngineer.Features.HeatSinkCapacityStat
{
    internal class HeatSinkCapacityStatFeature : Feature<HeatSinkCapacityStatSettings>
    {
        internal static HeatSinkCapacityStatFeature Shared = new HeatSinkCapacityStatFeature();

        internal override HeatSinkCapacityStatSettings Settings => Control.settings.HeatSinkCapacityStat;

        internal void InitEffectStats(Mech mech)
        {
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
}
