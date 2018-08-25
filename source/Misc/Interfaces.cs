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

    internal interface IProcessWeaponHit
    {
        bool ProcessWeaponHit(MechComponent mechComponent, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects);
    }

    internal interface ITonnageChanges
    {
        float TonnageChanges(MechDef mechDef);
    }

    internal interface IValidateDrop
    {
        MechLabDropResult ValidateDrop(MechLabItemSlotElement dragItem, MechLabLocationWidget widget);
    }

    internal interface IAdjustUpgradeDef
    {
        void AdjustUpgradeDef(UpgradeDef upgradeDef);
    }

    internal interface IAutoFixMechDef
    {
        void AutoFixMechDef(MechDef mechDef, float originalTotalTonnage);
    }
}