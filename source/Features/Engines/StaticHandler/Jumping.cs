using BattleTech;
using MechEngineer.Features.Engines.Helper;
using System.Linq;
using UnityEngine;

namespace MechEngineer.Features.Engines.StaticHandler
{
    internal class Jumping
    {
        internal static void InitEffectStats(Mech mech)
        {
            mech.StatCollection.JumpCapacity().Create(0);
            mech.StatCollection.JumpHeat().Create(0);
        }

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

            {
                var statisticData = new StatisticEffectData()
                {
                    statName = mech.StatCollection.JumpCapacity().Key,
                    operation = StatCollection.StatOperation.Float_Add,
                    modValue = jumpJetDef.JumpCapacity.ToString(),
                    modType = "System.Single"
                };

                CreatePassiveEffect(mech, mechComponent, statisticData);
            }

            if (EngineFeature.settings.JumpJetDefaultJumpHeat.HasValue)
            {
                var statisticData = new StatisticEffectData()
                {
                    statName = mech.StatCollection.JumpHeat().Key,
                    operation = StatCollection.StatOperation.Float_Add,
                    modValue = EngineFeature.settings.JumpJetDefaultJumpHeat.ToString(),
                    modType = "System.Single"
                };

                CreatePassiveEffect(mech, mechComponent, statisticData);
            }
        }

        internal static void CreatePassiveEffect(Mech mech, MechComponent mechComponent, StatisticEffectData statisticData)
        {
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
            
            effectData.Description = new BaseDescriptionDef(statisticData.statName, statisticData.statName, "", null);

            effectData.statisticData = statisticData;

			var effectID = string.Format("PassiveEffect_{0}_{1}_JumpCapacity", mech.GUID, mechComponent.uid);
            mech.Combat.EffectManager.CreateEffect(effectData, effectID, -1, mech, mech, default, 0, false);
			mechComponent.createdEffectIDs.Add(effectID);
        }

        internal static int CalcJumpHeat(Mech mech, float jumpDistance)
        {
            var jumpCapacity = mech.StatCollection.JumpCapacity().Get();
            var maxJumpDistance = EngineMovement.ConvertMPToGameDistance(jumpCapacity);

            jumpDistance = Mathf.Max(jumpDistance, EngineFeature.settings.MinimumJumpDistanceForHeat);
            var jumpRatio = jumpDistance / maxJumpDistance;
            
            var jumpMaxHeat = mech.StatCollection.JumpHeat().Get();
            var jumpHeat = jumpRatio * jumpMaxHeat;
            return Mathf.CeilToInt(jumpHeat);
        }

        internal static float CalcMaxJumpDistance(Mech mech)
        {
            if (!mech.IsOperational || mech.IsProne)
			{
                return 0f;
			}

            var jumpCapacity = mech.StatCollection.JumpCapacity().Get();
            if (jumpCapacity < 0.1)
            {
                return 0f;
            }
            var jumpjetDistance = EngineMovement.ConvertMPToGameDistance(jumpCapacity);

            var mechJumpDistanceMultiplier = mech.StatCollection.JumpDistanceMultiplier().Get();
            return jumpjetDistance * mechJumpDistanceMultiplier;
        }

        internal static float GetJumpCapacity(MechDef mechDef)
        {
            return mechDef.Inventory
                .Select(x => x.Def as JumpJetDef)
                .Sum(x => x?.JumpCapacity ?? 0);
        }
    }
}