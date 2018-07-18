using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;
using fastJSON;
using UnityEngine;

namespace MechEngineer
{
    [CustomComponent("EngineCore")]
    public class EngineCoreDef : SimpleCustomComponent, IMechValidate
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
        internal HeatSinkDef HeatSinkDef => Def as HeatSinkDef; // TODO reintroduce GenericCustomComponent

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

        private bool JumpJetCheck(MechDef mechDef, out int count, out int max)
        {
            count = mechDef.Inventory.Count(c => c.ComponentDefType == ComponentType.JumpJet);
            max = GetMovement(mechDef.Chassis.Tonnage).JumpJetCount;
            return count <= max;
        }


        private bool MixedHSCheck(MechDef def, MechComponentRef componentRef)
        {
            var engineRef = componentRef.GetEngineCoreRef();
            if (engineRef == null)
            {
                Control.mod.Logger.LogError("No core ref!");
                return true;
            }

            var enginehs = engineRef.HeatSinkDef.HSCategory;

            return def.Inventory.Any(d => d.Is<EngineHeatSink>(out var hs) && hs.HSCategory != enginehs);
        }

        public void ValidateMech(Dictionary<MechValidationType, List<string>> errors,
            MechValidationLevel validationLevel, MechDef mechDef,
            MechComponentRef componentRef)
        {
            if (!JumpJetCheck(mechDef, out var count, out var max))
            {
                errors[MechValidationType.InvalidJumpjets].Add($"JUMPJETS: This Mech mounts too many jumpjets ({count} out of {max})");
            }

            if (!Control.settings.AllowMixingHeatSinkTypes && MixedHSCheck(mechDef, componentRef))
            {
                errors[MechValidationType.InvalidInventorySlots].Add("MIXED HEATSINKS: Heat Sink types cannot be mixed");
            }
        }

        public bool ValidateMechCanBeFielded(MechDef mechDef, MechComponentRef componentRef)
        {
            if (!JumpJetCheck(mechDef, out _, out _))
                return false;

            if (!Control.settings.AllowMixingHeatSinkTypes && MixedHSCheck(mechDef, componentRef))
            {
                return false;
            }

            return true;
        }
    }
}