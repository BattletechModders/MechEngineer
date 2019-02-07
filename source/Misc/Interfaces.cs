using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    internal interface IIdentifier
    {
        bool IsCustomType(MechComponentDef def);
    }

    internal interface IValidateMech
    {
        void ValidateMech(MechDef mechDef, Errors errors);
    }

    internal interface ITonnageChanges
    {
        float TonnageChanges(MechDef mechDef);
    }

    internal interface IAdjustUpgradeDef
    {
        void AdjustUpgradeDef(UpgradeDef upgradeDef);
    }

    internal interface IAutoFixMechDef
    {
        void AutoFixMechDef(MechDef mechDef);
    }
}