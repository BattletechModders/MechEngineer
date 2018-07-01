using System;
using BattleTech;

namespace MechEngineer
{
    internal class AdjustCompDefInvSizeHelper
    {
        private readonly Func<int, int> func;
        private readonly IIdentifier identifier;

        internal AdjustCompDefInvSizeHelper(IIdentifier identifier, Func<int, int> func)
        {
            this.identifier = identifier;
            this.func = func;
        }

        internal void AdjustComponentDef(MechComponentDef def)
        {
            if (!identifier.IsCustomType(def))
            {
                return;
            }

            var newSize = func(def.InventorySize);
            if (newSize < 0)
            {
                return;
            }

            var value = newSize;
            var propInfo = typeof(UpgradeDef).GetProperty("InventorySize");
            var propValue = Convert.ChangeType(value, propInfo.PropertyType);
            propInfo.SetValue(def, propValue, null);
        }
    }
}