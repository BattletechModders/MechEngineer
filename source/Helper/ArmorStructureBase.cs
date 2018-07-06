using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    public interface IWeightSavingSlotType
    {
        int RequiredCriticalSlotCount { get; }
        float WeightSavingsFactor { get; }
    }

    internal abstract class ArmorStructureBase : IValidateMech, IValidateDrop, IProcessWeaponHit, ITonnageChanges, IAdjustTooltip, IIdentifier, IDescription
    {
        private readonly ValidationHelper checker;

        protected ArmorStructureBase()
        {
            checker = new ValidationHelper(this, this) {Required = false, Unique = UniqueConstraint.Mech};
        }

        public abstract string CategoryName { get; }

        public MechLabDropResult ValidateDrop(MechLabItemSlotElement dragItem, MechLabLocationWidget widget)
        {
            return checker.ValidateDrop(dragItem, widget);
        }

        public void AdjustTooltip(TooltipPrefab_EquipmentAdapter tooltip, MechLabPanel panel, MechComponentDef mechComponentDef)
        {
            var mechDef = panel.activeMechDef;
            WeightSavings savings;
            if (IsCustomType(mechComponentDef))
            {
                savings = CalculateWeightSavings(mechDef, mechComponentDef);
            }
            else
            {
                return;
            }

            var tonnage = savings.TonnageSaved;

            tooltip.bonusesText.text = $"- {tonnage} ton, - {savings.RequiredCount} slots";
            tooltip.bonusesText.SetAllDirty();
        }

        public abstract bool IsCustomType(MechComponentDef def);

        public bool ProcessWeaponHit(MechComponent mechComponent, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            if (IsCustomType(mechComponent.componentDef))
            {
                return false;
            }

            return true;
        }

        public float TonnageChanges(MechDef mechDef)
        {
            float tonnageSaved = 0;
            {
                var weightSavings = CalculateWeightSavings(mechDef);
                tonnageSaved += weightSavings.TonnageSaved;
            }

            return -tonnageSaved;
        }

        public void ValidateMech(MechDef mechDef, Dictionary<MechValidationType, List<string>> errorMessages)
        {
            checker.ValidateMech(mechDef, errorMessages);

            var errors = new List<string>();

            {
                var weightSavings = CalculateWeightSavings(mechDef);
                errors.AddRange(weightSavings.ErrorMessages);
            }

            errorMessages[MechValidationType.InvalidInventorySlots].AddRange(errors);
        }

        protected abstract WeightSavings CalculateWeightSavings(MechDef mechDef, MechComponentDef def = null);
    }
}