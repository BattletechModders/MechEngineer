using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    internal interface IIdentifier
    {
        bool IsComponentDef(MechComponentDef def);
    }

    internal interface IDescription
    {
        string CategoryName { get; }
    }

    internal interface IValidationRulesCheck
    {
        void ValidationRulesCheck(MechDef mechDef, Dictionary<MechValidationType, List<string>> errorMessages);
    }

    internal interface IProcessWeaponHit
    {
        bool ProcessWeaponHit(MechComponent mechComponent, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects);
    }

    internal interface ITonnageChanges
    {
        float TonnageChanges(MechDef mechDef);
    }

    internal interface IAdjustTooltip
    {
        void AdjustTooltip(TooltipPrefab_EquipmentAdapter tooltip, MechLabPanel panel, MechComponentDef mechComponentDef);
    }

    internal interface IValidateAdd
    {
        void ValidateAdd(MechComponentDef newComponentDef,
            List<MechLabItemSlotElement> localInventory,
            ref string dropErrorMessage,
            ref bool result);
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