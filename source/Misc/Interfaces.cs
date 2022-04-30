using BattleTech;
using MechEngineer.Helper;

namespace MechEngineer.Misc;

internal interface IIdentifier
{
    bool IsCustomType(MechComponentDef def);
}

internal interface IValidateMech
{
    void ValidateMech(MechDef mechDef, Errors errors);
}

internal interface IAdjustUpgradeDef
{
    void AdjustUpgradeDef(UpgradeDef upgradeDef);
}

internal interface IAutoFixMechDef
{
    void AutoFixMechDef(MechDef mechDef);
}