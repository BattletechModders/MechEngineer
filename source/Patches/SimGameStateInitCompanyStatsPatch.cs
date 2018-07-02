using System;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(SimGameState), "InitCompanyStats")]
    public static class SimGameStateInitCompanyStatsPatch
    {
        public static void Postfix(StatCollection ___companyStats)
        {
            try
            {
                MechDefAutoFixFacade.InitCompanyStats(___companyStats);
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}