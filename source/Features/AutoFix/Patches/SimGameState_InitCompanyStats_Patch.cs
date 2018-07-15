using System;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(SimGameState), "InitCompanyStats")]
    public static class SimGameState_InitCompanyStats_Patch
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