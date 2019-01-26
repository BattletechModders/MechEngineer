using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;
using Harmony;

namespace MechEngineer
{
    internal class EngineHeat
    {
        internal static float GetEngineHeatDissipation(MechComponentRef[] inventory)
        {
            var engine = inventory.GetEngine();
            if (engine == null)
            {
                return Control.settings.EngineMissingFallbackHeatSinkCapacity;
            }

            return engine.EngineHeatDissipation;
        }
    }
}