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
            var headSinkDef = newComponentDef as HeatSinkDef;

            if (headSinkDef == null)
            {
                return null;
            }

            // check if we can work with it
            if (!headSinkDef.IsDHSKit() && !headSinkDef.IsSingle() && !headSinkDef.IsDouble())
            {
                return null;
            }

            var engineSlotElement = localInventory.FirstOrDefault(x => x?.ComponentRef?.Def is EngineCoreDef);

            if (engineSlotElement == null)
            {
                if (headSinkDef.IsDHSKit())
                {
                    return new MechLabDropErrorResult(
                        string.Format("Cannot add {0}: No Engine found", newComponentDef.Description.Name)
                    );
                }

                return null;
            }

            var engineRef = localInventory.Where(c => c != null).Select(c => c.ComponentRef).GetEngineCoreRef();
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

            if (headSinkDef.IsDHSKit())
            {
                if (engineRef.IsDHS)
                {
                    return new MechLabDropErrorResult(
                        string.Format("Cannot add {0}: Already a DHS engine", newComponentDef.Description.Name)
                    );
                }

                if (!Control.settings.AllowMixingDoubleAndSingleHeatSinks && engineRef.AdditionalSHSCount > 0)
                {
                    return new MechLabDropErrorResult(
                        string.Format("Cannot add {0}: Reinstall engine to remove additional heat sinks before converting", newComponentDef.Description.Name)
                    );
                }

                engineRef.IsDHS = true;
            }
            else
            {
                if (engineRef.AdditionalHeatSinkCount >= engineDef.MaxHeatSinks - engineDef.MinHeatSinks)
                {
                    return null;
                }

                if (!Control.settings.AllowMixingDoubleAndSingleHeatSinks)
                {
                    if (engineRef.IsDHS && headSinkDef.IsSingle() || engineRef.IsSHS && headSinkDef.IsDouble())
                    {
                        return new MechLabDropErrorResult(
                            string.Format("Cannot add {0}: Mixing DHS and SHS is not allowed", newComponentDef.Description.Name)
                        );
                    }
                }

                if (headSinkDef.IsDouble())
                {
                    engineRef.AdditionalDHSCount++;
                }
                else if (headSinkDef.IsSingle())
                {
                    engineRef.AdditionalSHSCount++;
                }
            }

            EnginePersistence.SaveEngineState(engineRef, mechLab);
            mechLab.ValidateLoadout(false);

            Traverse.Create(engineSlotElement).Method("RefreshInfo").GetValue();

            return new MechLabDropRemoveDragItemResult();
        }
    }
}