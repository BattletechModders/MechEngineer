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

        internal void InitStats(Mech mech)
        {
            // these steps are not necessary as everything should be 0 when using standard ME
            // we need this to initialize before passives or replace the default call -> transpiler
            //var stat = mech.StatCollection.HeatSinkCapacity();
            //var heatConstants = mech.Combat.Constants.Heat;
            //var defaultHeatSinking = (int) (heatConstants.InternalHeatSinkCount * heatConstants.DefaultHeatSinkDissipationCapacity);
            //stat.Set(stat.Get() + mech.MechDef.Chassis.Heatsinks + defaultHeatSinking);

            var core = mech.miscComponents.FirstOrDefault(x => x.componentDef.Is<EngineCoreDef>());
            if (core != null)
            {
                if (core.baseComponentRef.DamageLevel >= ComponentDamageLevel.NonFunctional)
                {
                    return;
                }

                var engine = mech.MechDef.GetEngine();
                if (engine == null)
                {
                    return;
                }

                var statisticData = StatCollectionExtension
                    .HeatSinkCapacity(null)
                    .CreateStatisticData(
                        StatCollection.StatOperation.Int_Add,
                        (int)engine.EngineHeatDissipation
                    );

                var effectData = MechComponentDefExtensions.CreatePassiveEffectData("EngineCoreEffect", statisticData);

                EffectIdUtil.CreateEffect(core, effectData, $"{effectData.Description.Id}_{mech.GUID}");
            }
        }
    }
}
