using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomComponents;
using Harmony;

namespace MechEngineer
{
    internal class EngineHeat : IValidateDrop
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

        public static EngineHeat Shared = new EngineHeat();

        // only allow one engine part per specific location
        public MechLabDropResult ValidateDrop(MechLabItemSlotElement dragItem, MechLabLocationWidget widget)
        {
            var newComponentRef = dragItem.ComponentRef;
            var newComponentDef = newComponentRef.Def;

            var heatSinkDef = newComponentDef.GetComponent<EngineHeatSink>();
            var heatSinkKitDef = newComponentDef.GetComponent<EngineHeatSinkKit>();

            // check if we can work with it
            if (heatSinkDef == null && heatSinkKitDef == null)
            {
                return null;
            }

            var adapter = new MechLabLocationWidgetAdapter(widget);
            var localInventory = adapter.localInventory;

            var engineSlotElement = localInventory.FirstOrDefault(x => x?.ComponentRef?.Def?.GetComponent<EngineCoreDef>() != null);

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
            
            var mechLab = adapter.mechLab;
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
                if (engineRef.HeatSinkDef != newComponentDef.DataManager.GetDefaultEngineHeatSinkDef())
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

                engineRef.HeatSinkDef = mechLab.dataManager.GetEngineHeatSinkDef(heatSinkKitDef.HeatSinkDefId);
            }
            else
            {
                if (engineRef.AdditionalHeatSinkCount >= engineDef.MaxAdditionalHeatSinks)
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
            
            EngineCoreRefHandler.Shared.RefreshSlotElement(engineSlotElement);

            return new MechLabDropRemoveDragItemResult();
        }
    }
}