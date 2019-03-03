using System;
using BattleTech;
using Harmony;
using UnityEngine;

namespace MechEngineer
{
    [HarmonyPatch(typeof(Mech), "InitEffectStats")]
    public static class Mech_InitEffectStats_Patch5
    {
        public static void Prefix(Mech __instance)
        {
            try
            {
                ArmyActuatorsCollection.LeftArmAccuracy(__instance.StatCollection);
                ArmyActuatorsCollection.RightArmAccuracy(__instance.StatCollection);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }

    public static class ArmyActuatorsCollection
    {
        public static float LeftArmAccuracy(StatCollection statCollection) 
        {
            const string key = "ArmAccuracyLeft";
            return ArmAccuracy(statCollection, key);
        }
        
        public static float RightArmAccuracy(StatCollection statCollection) 
        {
            const string key = "ArmAccuracyRight";
            return ArmAccuracy(statCollection, key);
        }
        
        private static float ArmAccuracy(StatCollection statCollection, string collectionKey) 
        {
            var statistic = statCollection.GetStatistic(collectionKey);
            if (statistic == null)
            {
                const float defaultValue = 1.0f;
                statistic = statCollection.AddStatistic(collectionKey, defaultValue);
            }
            return statistic.Value<float>();
        }
    }
}