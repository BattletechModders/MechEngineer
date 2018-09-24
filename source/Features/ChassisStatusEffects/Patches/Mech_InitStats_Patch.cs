using System;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(Mech), "InitStats")]
    public static class Mech_InitStats_Patch
    {
        public const string LOGGER_NAME = "MechEngineer.MechInitStats";

        [HarmonyPriority(Priority.Last)]
        public static void Postfix(Mech __instance)
        {
            try
            {
                var mech = __instance;

                var logger = LogManager.GetLogger(LOGGER_NAME);
                if (!logger.IsDebugEnabled)
                {
                    return;
                }
                logger.LogDebug($"listing effects for mech {mech.LogDisplayName}");
                foreach (var effect in mech.Combat.EffectManager.GetAllEffectsTargeting(mech))
                {
                    logger.LogDebug($" {effect.id}");
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}