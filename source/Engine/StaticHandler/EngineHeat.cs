using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    internal static class EngineHeat
    {
        internal static float GetEngineHeatDissipation(MechComponentRef[] inventory)
        {
            var engineRef = inventory.GetEngineCoreRef();
            if (engineRef == null)
            {
                return Control.settings.EngineMissingFallbackHeatSinkCapacity;
            }

            return engineRef.EngineHeatDissipation;
        }

        // only allow one engine part per specific location
        internal static MechLabDropResult ValidateDrop(
            MechLabPanel mechLab,
            MechLabItemSlotElement dragItem,
            List<MechLabItemSlotElement> localInventory
            )
        {
            var newComponentRef = dragItem.ComponentRef;
            var newComponentDef = newComponentRef.Def;
            var heatSinkDef = newComponentDef as HeatSinkDef;

            if (heatSinkDef == null)
            {
                return null;
            }

            // check if we can work with it
            if (!heatSinkDef.IsDHSKit() && !heatSinkDef.IsCDHSKit() && !heatSinkDef.IsSingle() && !heatSinkDef.IsDouble() && !heatSinkDef.IsDoubleClan())
            {
                return null;
            }

            var engineSlotElement = localInventory.FirstOrDefault(x => x?.ComponentRef?.Def is EngineCoreDef);

            if (engineSlotElement == null)
            {
                if (heatSinkDef.IsDHSKit() || heatSinkDef.IsCDHSKit())
                {
                    return new MechLabDropErrorResult(
                        string.Format("Cannot add {0}: No Engine found", newComponentDef.Description.Name)
                    );
                }

                return null;
            }

            var engineRef = engineSlotElement.ComponentRef.GetEngineCoreRef();
            var engineDef = engineRef.CoreDef;

            if (mechLab.IsSimGame)
            {
                if (dragItem.OriginalDropParentType != MechLabDropTargetType.InventoryList)
                {
                    return new MechLabDropErrorResult(
                        string.Format("Cannot add {0}: Item has to be from inventory", newComponentDef.Description.Name)
                    );
                }

                if (mechLab.originalMechDef.Inventory.Any(c => c.SimGameUID == engineRef.ComponentRef.SimGameUID))
                {
                    return new MechLabDropErrorResult(
                        string.Format("Cannot add {0}: Engine cannot be modified once installed, remove engine first", newComponentDef.Description.Name)
                    );
                }
            }

            if (heatSinkDef.IsDHSKit() || heatSinkDef.IsCDHSKit())
            {
                if (!engineRef.Is(HeatSinkType.SHS))
                {
                    return new MechLabDropErrorResult(
                        string.Format("Cannot add {0}: Reinstall engine to remove internal heat sinks", newComponentDef.Description.Name)
                    );
                }

                if (!Control.settings.AllowMixingHeatSinkTypes && engineRef.Query(HeatSinkType.SHS).AdditionalCount > 0)
                {
                    return new MechLabDropErrorResult(
                        string.Format("Cannot add {0}: Reinstall engine to remove additional heat sinks before converting", newComponentDef.Description.Name)
                    );
                }

                if (heatSinkDef.IsCDHSKit())
                {
                    engineRef.HSType = HeatSinkType.CDHS;
                }
                else if (heatSinkDef.IsDHSKit())
                {
                    engineRef.HSType = HeatSinkType.DHS;
                }
            }
            else
            {
                if (engineRef.AdditionalHeatSinkCount >= engineDef.MaxHeatSinks - engineDef.MinHeatSinks)
                {
                    return null;
                }

                HeatSinkType hstype;
                if (heatSinkDef.IsDouble())
                {
                    hstype = HeatSinkType.DHS;
                }
                else if (heatSinkDef.IsDoubleClan())
                {
                    hstype = HeatSinkType.CDHS;
                }
                else
                {
                    hstype = HeatSinkType.SHS;
                }

                if (!Control.settings.AllowMixingHeatSinkTypes)
                {
                    if (!engineRef.Is(hstype))
                    {
                        return new MechLabDropErrorResult(
                            string.Format("Cannot add {0}: Mixing heat sink types is not allowed", newComponentDef.Description.Name)
                        );
                    }
                }
                engineRef.Query(hstype).AdditionalCount++;
            }

            EnginePersistence.SaveEngineState(engineRef, mechLab);
            mechLab.ValidateLoadout(false);

            Traverse.Create(engineSlotElement).Method("RefreshInfo").GetValue();

            return new MechLabDropRemoveDragItemResult();
        }
    }
}