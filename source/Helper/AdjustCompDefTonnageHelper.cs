using System;
using BattleTech;

namespace MechEngineer
{
    internal class AdjustCompDefTonnageHelper
    {
        private readonly Func<float, float> func;
        private readonly IIdentifier identifier;

        internal AdjustCompDefTonnageHelper(IIdentifier identifier, Func<float, float> func)
        {
            this.identifier = identifier;
            this.func = func;
        }

        internal void AdjustComponentDef(MechComponentDef def)
        {
            if (!identifier.IsComponentDef(def))
            {
                return;
            }

            var newTonnage = func(def.Tonnage);
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