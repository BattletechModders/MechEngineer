using System.Linq;
using BattleTech;

namespace MechEngineMod
{
    internal static class CalculateTonnageFacade
    {
        internal static float AdditionalTonnage(MechDef mechDef)
        {
            float tonnage = 0;
            tonnage += Engine.AdditionalHeatSinkTonnage(mechDef);
            tonnage -= ArmorStructure.TonnageSavings(mechDef);
            return tonnage;
        }
    }
}