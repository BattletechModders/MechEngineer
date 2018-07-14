using System;
using BattleTech;

namespace MechEngineer
{
    internal class AdjustCompDefTonnageHelper
    {
        private readonly IIdentifier identifier;
        private readonly ValueChange<float> change;

        internal AdjustCompDefTonnageHelper(IIdentifier identifier, ValueChange<float> change)
        {
            this.identifier = identifier;
            this.change = change;
        }

        internal void AdjustComponentDef(MechComponentDef def)
        {
            if (change == null)
            {
                return;
            }

            if (!identifier.IsCustomType(def))
            {
                return;
            }
            
            var newTonnage = change.Change(def.Tonnage);
            if (newTonnage < 0)
            {
                return;
            }

            var value = newTonnage;
            var propInfo = typeof(UpgradeDef).GetProperty("Tonnage");
            var propValue = Convert.ChangeType(value, propInfo.PropertyType);
            propInfo.SetValue(def, propValue, null);
        }
    }
}