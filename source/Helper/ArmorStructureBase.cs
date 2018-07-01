using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    internal abstract class ArmorStructureBase : IValidationRulesCheck, IProcessWeaponHit, ITonnageChanges, IAdjustTooltip, IIdentifier
    {
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

            tooltip.bonusesText.text = string.Format("- {0} ton,  {1} / {2}", tonnage, savings.Count, savings.RequiredCount);
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

        public void ValidationRulesCheck(MechDef mechDef, Dictionary<MechValidationType, List<string>> errorMessages)
        {
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