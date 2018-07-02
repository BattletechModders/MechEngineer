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

            var heatSinkDef = newComponentDef as EngineHeatSinkDef;
            var heatSinkKitDef = newComponentDef as EngineHeatSinkKitDef;

            // check if we can work with it
            if (heatSinkDef == null && heatSinkKitDef == null)
            {
                return null;
            }

            var engineSlotElement = localInventory.FirstOrDefault(x => x?.ComponentRef?.Def is EngineCoreDef);

            if (engineSlotElement == null)
            {
                if (heatSinkKitDef != null)
                {
                    return new MechLabDropErrorResult(
                        $"Cannot add {newComponentDef.Description.Name}: No Engine found"
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
                        $"Cannot add {newComponentDef.Description.Name}: Item has to be from inventory"
                    );
                }

                if (mechLab.originalMechDef.Inventory.Any(c => c.SimGameUID == engineRef.ComponentRef.SimGameUID))
                {
                    return new MechLabDropErrorResult(
                        $"Cannot add {newComponentDef.Description.Name}: Engine cannot be modified once installed, remove engine first"
                    );
                }
            }

            if (heatSinkKitDef != null)
            {
                if (engineRef.HeatSinkDef != newComponentDef.DataManager.GetStandardHeatSinkDef())
                {
                    return new MechLabDropErrorResult(
                        $"Cannot add {newComponentDef.Description.Name}: Reinstall engine to remove internal heat sinks"
                    );
                }

                if (engineRef.AdditionalHeatSinkCount > 0)
                {
                    return new MechLabDropErrorResult(
                        $"Cannot add {newComponentDef.Description.Name}: Reinstall engine to remove additional heat sinks before converting"
                    );
                }

                engineRef.HeatSinkDef = heatSinkKitDef.HeatSinkDef;
            }
            else
            {
                if (engineRef.AdditionalHeatSinkCount >= engineDef.MaxHeatSinks - engineDef.MinHeatSinks)
                {
                    return null;
                }

                if (!Control.settings.AllowMixingHeatSinkTypes)
                {
                    if (engineRef.HeatSinkDef.HSCategory != heatSinkDef.HSCategory)
                    {
                        return new MechLabDropErrorResult(
                            $"Cannot add {newComponentDef.Description.Name}: Mixing heat sink types is not allowed"
                        );
                    }
                }

                engineRef.Query(heatSinkDef).AdditionalCount++;
            }

            EnginePersistence.SaveEngineState(engineRef, mechLab);
            mechLab.ValidateLoadout(false);

            Traverse.Create(engineSlotElement).Method("RefreshInfo").GetValue();

            return new MechLabDropRemoveDragItemResult();
        }
    }
}