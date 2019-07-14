using BattleTech;
using MechEngineer.Features.Engines.Helper;

namespace MechEngineer.Features.Engines.StaticHandler
{
    internal class EngineHeat
    {
        internal static float GetEngineHeatDissipation(MechDef mechDef)
        {
            var engine = mechDef.GetEngine();
            if (engine == null)
            {
                return EngineFeature.settings.EngineMissingFallbackHeatSinkCapacity;
            }

            return engine.EngineHeatDissipation;
        }
    }
}