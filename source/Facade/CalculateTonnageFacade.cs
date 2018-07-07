using BattleTech;

namespace MechEngineer
{
    internal static class CalculateTonnageFacade
    {
        internal static float AdditionalTonnage(MechDef mechDef)
        {
            float tonnage = 0;
            tonnage += EngineHandler.Shared.TonnageChanges(mechDef);
            tonnage += WeightSavingsHandler.Shared.TonnageChanges(mechDef);
            return tonnage;
        }
    }
}