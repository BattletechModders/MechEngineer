using System;
using UnityEngine;

namespace MechEngineMod
{
    internal class EngineCalculator
    {
        internal bool CalcAvailability(EngineDef engineDef, float tonnage)
        {
            //Control.mod.Logger.LogDebug("CalcAvailability rating=" + engineDef.Rating + " tonnage=" + tonnage);
			
            float TTWalkDistance = engineDef.Rating / tonnage;

            //Control.mod.Logger.LogDebug("CalcAvailability" +
            //                            " rating=" + engineDef.Rating +
            //                            " tonnage=" + tonnage +
            //                            " TTWalkDistance=" + TTWalkDistance +
            //                            " Min Walk =" + Control.settings.const_MinTTWalk +
            //                            " Max Walk =" + Control.settings.const_MaxTTWalk);

            // check if over max walk distance
            if (TTWalkDistance > Control.settings.const_MaxTTWalk)
            {
                return false;
            }

            // check if below min walk distance
            if (TTWalkDistance < Control.settings.const_MinTTWalk)
            {
                return false;
            }
			
            //check if non integer TT walk values are allowed
            if (Control.settings.AllowNonIntWalkValues == false)
            {
                //if not, check that walk value is an integer
                if ((TTWalkDistance % 1f) != 0)
                    return false;
            }

            return true;
        }


        internal void CalcSpeeds(EngineDef engineDef, float tonnage, out float walkSpeed, out float runSpeed, out float TTWalkSpeed)
        {
            TTWalkSpeed = engineDef.Rating / tonnage;
            walkSpeed = Calc_WalkDistance(TTWalkSpeed);
            runSpeed = Calc_SprintDistance(TTWalkSpeed);

            //Control.mod.Logger.LogDebug("CalcSpeeds" +
            //                            " rating=" + engineDef.Rating +
            //                            " tonnage=" + tonnage +
            //                            " walkSpeed=" + walkSpeed +
            //                            " runSpeed=" + runSpeed +
            //                            " TTWalkSpeed=" + TTWalkSpeed);
        }

        internal int CalcInstallTechCost(EngineDef engineDef)
        {
            return Mathf.CeilToInt(Control.settings.TechCostPerEngineTon * engineDef.Def.Tonnage);
        }

        internal void CalcHeatSinks(EngineDef engineDef, out int minHeatSinks, out int maxHeatSinks)
        {
            maxHeatSinks = engineDef.Rating / 25;
            minHeatSinks = Math.Min(maxHeatSinks, 10);
        }

        internal float CalcGyroWeight(EngineDef engineDef)
        {
            // for now only used for engine details text, not for any actual tonnage calculations
            return (int)(engineDef.Rating / 100);
        }

        internal int CalcJumpJetCount(EngineDef engineDef, float tonnage)
        {

            float TTWalkSpeed = engineDef.Rating / tonnage;
            float AllowedJJs = Math.Min(TTWalkSpeed, Control.settings.const_MaxNumberOfJJ);
			
            if (Control.settings.JJRoundUp)
                return Mathf.CeilToInt(AllowedJJs);
            else
                return Mathf.FloorToInt(AllowedJJs);
        }

        private static float Calc_WalkDistance(float TTWalkSpeed)
            // numbers the result of the best fit line for the game movement
        {
            float WalkSpeedFixed = 26.05f;
            float WalkSpeedMult = 23.14f;
		
            if (Control.settings.UseGameWalkValues)
                return (WalkSpeedFixed + TTWalkSpeed * WalkSpeedMult).RoundBy5();
            else
                return (TTWalkSpeed * Control.settings.const_TTWalkMultiplier).RoundBy5();
			
        }
		
        private static float Calc_SprintDistance(float TTWalkSpeed)
            // numbers the result of the best fit line for the game movement
        {
            float RunSpeedFixed = 52.43f;
            float RunSpeedMult = 37.29f;
			
            if (Control.settings.UseGameWalkValues)
                return (RunSpeedFixed + TTWalkSpeed * RunSpeedMult).RoundBy5();
            else
                return (TTWalkSpeed * Control.settings.const_TTSprintMultiplier).RoundBy5();
        }
		
    }
}