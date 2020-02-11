using BattleTech;
using MechEngineer.Features.ArmorStructureChanges;
using MechEngineer.Features.OverrideStatTooltips.Helper;

namespace MechEngineer.Features.OverrideStatTooltips
{
    internal class DurabilityStat : IStatHandler
    {
        public void SetupTooltip(StatTooltipData tooltipData, MechDef mechDef)
        {
            tooltipData.dataList.Clear();
            
			tooltipData.dataList.Add("TODO", "TODO");
/*
MaxStructure
MaxStability + UnstreadThreshold
MaxEvasivePips

DamageReductionAll / Melee
ToHitThisActor / Melee

Energy / Anti / Ballistic / Missle
Energy / Anti / Ballistic / Missle / DirectFire

mechDef.MechDefMaxStructure; stats.MaxStructureFactor;

ReceivedInstabilityMultiplier
("MinStability", 0f);
("MaxStability", this.MechDef.Chassis.Stability);
("UnsteadyThreshold", base.Combat.Constants.ResolutionConstants.DefaultUnsteadyThreshold);
<int>("MaxEvasivePips", 6);

<float>("DamageReductionMultiplierAll", 1f);
<float>("DamageReductionMultiplierMelee", 1f);
<float>("DamageReductionMultiplierEnergy", 1f);
<float>("DamageReductionMultiplierBallistic", 1f);
<float>("DamageReductionMultiplierMissile", 1f);
<float>("DamageReductionMultiplierAntipersonnel", 1f);
<float>("ToHitThisActor", 0f);
<float>("ToHitThisActorBallistic", 0f);
<float>("ToHitThisActorEnergy", 0f);
<float>("ToHitThisActorMissile", 0f);
<float>("ToHitThisActorSmall", 0f);
<float>("ToHitThisActorMelee", 0f);
<float>("ToHitThisActorDirectFire", 0f);
*/
        }

        public float BarValue(MechDef mechDef)
        {
			var armor = mechDef.MechDefAssignedArmor;
            armor *= ArmorMultiplier(mechDef);
            armor *= DamageReductionMultiplierAll(mechDef);

			var stats = UnityGameInstance.BattleTechGame.MechStatisticsConstants;
            return MechStatUtils.NormalizeToFraction(armor, 0, stats.MaxArmorFactor);
        }

        private float ArmorMultiplier(MechDef mechDef)
        {
            StatCollection statCollection = new StatCollection();
            var stat = statCollection.ArmorMultiplier();
            stat.Create();
            return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
        }

        private float DamageReductionMultiplierAll(MechDef mechDef)
        {
            StatCollection statCollection = new StatCollection();
            var stat = DamageReductionMultiplierAll(statCollection);
            stat.Create();
            return MechDefStatisticModifier.ModifyStatistic(stat, mechDef);
        }

        private static StatisticAdapter<float> DamageReductionMultiplierAll(StatCollection statCollection)
        {
            return new StatisticAdapter<float>("DamageReductionMultiplierAll", statCollection, 1);
        }
    }
}
