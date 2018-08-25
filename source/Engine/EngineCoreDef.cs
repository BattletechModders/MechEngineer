using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;
using fastJSON;
using UnityEngine;

namespace MechEngineer
{
    [CustomComponent("EngineCore")]
    public class EngineCoreDef : SimpleCustom<HeatSinkDef>, IAdjustSlotElement, IAdjustTooltip
    {
        [JsonIgnore]
        private int _rating;

        public int Rating
        {
            get => _rating;
            set
            {
                _rating = value;
                CalcHeatSinks();
                CalcTonnages();
            }
        }

        private void CalcHeatSinks()
        {
            var free = 10;
            var total = Rating / 25;
            InternalHeatSinks = Mathf.Min(free, total);
            MaxAdditionalHeatSinks = Mathf.Max(0, total - free);
            MaxFreeExternalHeatSinks = free - InternalHeatSinks;
        }

        [JsonIgnore]
        internal int InternalHeatSinks { get; private set; }
        [JsonIgnore]
        internal int MaxAdditionalHeatSinks { get; private set; }
        [JsonIgnore]
        internal int MaxFreeExternalHeatSinks { get; private set; }

        internal float MaxInternalHeatSinks => InternalHeatSinks + MaxAdditionalHeatSinks;

        private void CalcTonnages()
        {
            StandardGyroTonnage = Mathf.Ceil(Rating / 100f);
        }

        [JsonIgnore]
        internal float StandardGyroTonnage { get; private set; }

        internal float StandardEngineTonnage => Def.Tonnage - StandardGyroTonnage;

        internal EngineMovement GetMovement(float tonnage)
        {
            return new EngineMovement(Rating, tonnage);
        }

        public override string ToString()
        {
            return Def.Description.Id + " Rating=" + Rating;
        }

        public void AdjustTooltip(TooltipPrefab_Equipment tooltip, MechComponentDef componentDef)
        {
            EngineHandler.Shared.AdjustTooltip(tooltip, componentDef);
        }

        public void AdjustSlotElement(MechLabItemSlotElement element, MechLabPanel panel)
        {
            EngineCoreRefHandler.Shared.AdjustSlotElement(element, panel);
        }
    }
}