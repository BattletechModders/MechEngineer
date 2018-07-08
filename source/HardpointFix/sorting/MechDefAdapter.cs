using BattleTech;
using BattleTech.Data;
using Harmony;

namespace MechEngineer
{
    internal class MechDefAdapter
    {
        private readonly MechDef _instance;
        private readonly Traverse _traverse;

        internal MechDefAdapter(MechDef instance)
        {
            _instance = instance;
            _traverse = Traverse.Create(instance);
        }

        internal MechComponentRef[] Inventory => _traverse.Field("inventory").GetValue() as MechComponentRef[];

        internal DataManager DataManager => _traverse.Field("dataManager").GetValue() as DataManager;

        internal ChassisDef Chassis => _instance.Chassis;
    }
}