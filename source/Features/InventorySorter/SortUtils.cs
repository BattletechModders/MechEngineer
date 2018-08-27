using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    public static class SortUtils
    {
        public static string SortKey(MechComponentDef def)
        {
            return def?.GetComponent<InventorySorter>()?.SortKey ?? string.Empty;
        }
    }
}