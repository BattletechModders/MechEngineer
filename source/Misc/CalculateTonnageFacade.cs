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
            tonnage -= Structure.WeightSavings(mechDef);
            tonnage -= Armor.WeightSavings(mechDef);
            return tonnage;
        }
    }
}