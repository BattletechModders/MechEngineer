using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using CustomComponents;

namespace MechEngineer
{
    internal class EngineValidation : IValidateMech
    {
        internal static EngineValidation Shared = new EngineValidation();
        internal CCValidationAdapter CCValidation;

        internal EngineValidation()
        {
            CCValidation = new CCValidationAdapter(this);
        }

        public void ValidateMech(MechDef mechDef, Dictionary<MechValidationType, List<string>> errorMessages)
        {
            var engine = mechDef.GetEngine();
            if (engine == null)
            {
                return;
            }

            {
                var count = mechDef.Inventory.Count(c => c.ComponentDefType == ComponentType.JumpJet);
                var max = engine.CoreDef.GetMovement(mechDef.Chassis.Tonnage).JumpJetCount;

                if (count > max)
                {
                    errorMessages[MechValidationType.InvalidJumpjets].Add($"JUMP JETS: This Mech mounts too many jumpjets ({count} / {max})");
                }
            }

            if (Control.settings.MinimumHeatSinksOnMech > 0)
            {
                var externalCount = mechDef.Inventory.Count(c => c.Is<EngineHeatSink>());
                var internalCount = engine.CoreDef.InternalHeatSinks;
                var count = internalCount + externalCount;

                var min = Control.settings.MinimumHeatSinksOnMech;

                if (count < min)
                {
                    errorMessages[MechValidationType.InvalidInventorySlots].Add($"HEAT SINKS: This Mech has too few heat sinks ({count} / {min})");
                }
            }

            if (!Control.settings.AllowMixingHeatSinkTypes)
            {
                var types = new HashSet<string>();

                var inventoryHeatSinkTypes = mechDef.Inventory.Select(r => r.GetComponent<EngineHeatSink>()).Where(hs => hs != null);
                foreach (var hs in inventoryHeatSinkTypes.Union(engine.CoreRef.GetInternalEngineHeatSinkTypes()))
                {
                    types.Add(hs.HSCategory);
                    if (types.Count > 1)
                    {
                        errorMessages[MechValidationType.InvalidInventorySlots].Add("MIXED HEAT SINKS: Heat Sink types cannot be mixed");
                        break;
                    }
                }
            }
        }
    }
}