using System;
using UnityEngine;

namespace MechEngineMod
{
    internal class EngineCalculator
    {
        // r = t * w
        // s = w * 5/3
        // f = d * t / r
        // CDA-2A 350d 40t 320r // 44f
        // SHD-2H 240d 55t 275r // 48f
        // AS7-D 165d 100t 300r // 55f		
        internal static float func_Roundby5(float value)
        {
            if (value % 5f < 2.5)
                return (value - (value % 5f));
            else
                return (value - (value % 5f) + 5f);
        }

        internal bool CalcAvailability(Engine engine, float tonnage)
        {
            Control.mod.Logger.LogDebug("CalcAvailability rating=" + engine.Rating + " tonnage=" + tonnage);
			
            float TTWalkDistance = engine.Rating / tonnage;

            Control.mod.Logger.LogDebug("CalcAvailability" +
                                        " rating=" + engine.Rating +
                                        " tonnage=" + tonnage +
                                        " TTWalkDistance=" + TTWalkDistance +
                                        " Min Walk =" + Control.settings.const_MinTTWalk +
                                        " Max Walk =" + Control.settings.const_MaxTTWalk);

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


        internal void CalcSpeeds(Engine engine, float tonnage, out float walkSpeed, out float runSpeed, out float TTWalkSpeed)
        {

            TTWalkSpeed = engine.Rating / tonnage;
            walkSpeed = Calc_WalkDistance(TTWalkSpeed);
            runSpeed = Calc_SprintDistance(TTWalkSpeed);

			
            Control.mod.Logger.LogDebug("CalcSpeeds" +
                                        " rating=" + engine.Rating +
                                        " tonnage=" + tonnage +
                                        " walkSpeed=" + walkSpeed +
                                        " runSpeed=" + runSpeed 
                                        +
                                        " TTWalkSpeed=" + TTWalkSpeed)
                ;
        }

        internal int CalcInstallTechCost(Engine engine)
        {
            return Mathf.CeilToInt(Control.settings.TechCostPerEngineTon * engine.Def.Tonnage);
        }
        
        internal int CalcJumpJetCount(Engine engine, float tonnage)
        {

            float TTWalkSpeed = engine.Rating / tonnage;
            float AllowedJJs = Math.Min(TTWalkSpeed, Control.settings.const_MaxNumberOfJJ);
			
            if (Control.settings.JJRoundUp == true)
                return Mathf.CeilToInt(AllowedJJs);
            else
                return Mathf.FloorToInt(AllowedJJs);
        }

        private static float Calc_WalkDistance(float TTWalkSpeed)
            // numbers the result of the best fit line for the game movement
        {
            float WalkSpeedFixed = 26.05f;
            float WalkSpeedMult = 23.14f;
		
            if (Control.settings.UseGameWalkValues == true)
                return func_Roundby5(WalkSpeedFixed + TTWalkSpeed * WalkSpeedMult);
            else
                return func_Roundby5(TTWalkSpeed * Control.settings.const_TTWalkMultiplier);
			
        }
		
        private static float Calc_SprintDistance(float TTWalkSpeed)
            // numbers the result of the best fit line for the game movement
        {
            float RunSpeedFixed = 52.43f;
            float RunSpeedMult = 37.29f;
			
            if (Control.settings.UseGameWalkValues == true)
                return func_Roundby5(RunSpeedFixed + TTWalkSpeed * RunSpeedMult);
            else
                return func_Roundby5(TTWalkSpeed * Control.settings.const_TTSprintMultiplier);
        }
		
    }
}