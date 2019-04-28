using BattleTech;
using MechEngineer.Features.Engine.Helper;

namespace MechEngineer.Features.Engine.StaticHandler
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