using BattleTech;
using CustomComponents;
using fastJSON;
using UnityEngine;

namespace MechEngineer
{
    [CustomComponent("EngineCore")]
    public class EngineCoreDef : SimpleCustomComponent
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
        internal float GyroTonnage => (Rating / 100f).RoundStandard();
        internal float StandardEngineTonnage => Def.Tonnage - GyroTonnage;
        internal HeatSinkDef HeatSinkDef => Def as HeatSinkDef; // TODO reintroduce GenericCustomComponent

        internal EngineMovement GetMovement(float tonnage)
        {
            return new EngineMovement(Rating, tonnage);
        }

        public override string ToString()
        {
            return Def.Description.Id + " Rating=" + Rating;
        }
    }
}