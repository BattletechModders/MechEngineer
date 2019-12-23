using BattleTech;
using MechEngineer.Features.Engines.Helper;
using System.Linq;

namespace MechEngineer.Features.Engines.StaticHandler
{
    internal class EngineJumpJet
    {
        internal static void InitPassiveSelfEffects(MechComponent mechComponent)
        {
            if (!EngineFeature.settings.AutoConvertJumpCapacityInDefToStat)
            {
                return;
            }
            if (!(mechComponent is Jumpjet jumpJet))
            {
                return;
            }
            if (!(mechComponent.parent is Mech mech))
            {
                return;
            }
            if (!(jumpJet.componentDef is JumpJetDef jumpJetDef))
            {
                return;
            }
            if (mechComponent.DamageLevel < ComponentDamageLevel.NonFunctional)
            {
                return;
            }

            var effectData = new EffectData
            {
                effectType = EffectType.StatisticEffect,
                nature = EffectNature.Buff
            };

            effectData.durationData = new EffectDurationData
            {
                duration = -1,
                stackLimit = -1
            };

            effectData.targetingData = new EffectTargetingData
            {
                effectTriggerType = EffectTriggerType.Passive,
                effectTargetType = EffectTargetType.Creator
            };
            
            effectData.Description = new BaseDescriptionDef("JumpCapacity", "JumpCapacity", "", null);

            effectData.statisticData = new StatisticEffectData()
            {
                statName = JumpCapacityStatisticKey,
                operation = StatCollection.StatOperation.Float_Add,
                modValue = jumpJetDef.JumpCapacity.ToString(),
                modType = "System.Single"
            };

			var effectID = string.Format("PassiveEffect_{0}_{1}_JumpCapacity", mech.GUID, mechComponent.uid);
            mech.Combat.EffectManager.CreateEffect(effectData, effectID, -1, mech, mech, default, 0, false);
			mechComponent.createdEffectIDs.Add(effectID);
        }

        internal static float CalcJumpDistance(Mech mech)
        {
            if (!mech.IsOperational || mech.IsProne)
			{
                return 0f;
			}

            var jumpCapacity = GetJumpCapacity(mech);
            if (jumpCapacity < 0.1)
            {
                return 0f;
            }
            var jumpjetDistance = EngineMovement.ConvertMPToGameDistance(jumpCapacity);

            var mechJumpDistanceMultiplier = mech.StatCollection.GetValue<float>("JumpDistanceMultiplier");
            return jumpjetDistance * mechJumpDistanceMultiplier;
        }

        internal static float GetJumpCapacity(MechDef mechDef)
        {
            return mechDef.Inventory
                .Select(x => x.Def as JumpJetDef)
                .Sum(x => x?.JumpCapacity ?? 0);
        }

        internal static void CreateJumpCapacity(Mech mech)
        {
            mech.StatCollection.AddStatistic<float>(JumpCapacityStatisticKey, 0);
        }

        private static float GetJumpCapacity(Mech mech)
        {
            return mech.StatCollection.GetValue<float>(JumpCapacityStatisticKey);
        }

        private const string JumpCapacityStatisticKey = "JumpCapacity";
    }
}