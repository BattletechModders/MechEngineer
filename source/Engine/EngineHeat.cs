using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    internal static class EngineHeat
    {
        // invalidate mech loadouts that mix double and single heatsinks
        internal static void ValidationRulesCheck(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            if (Control.settings.AllowMixingDoubleAndSingleHeatSinks)
            {
                return;
            }

            bool hasSingle = false, hasDouble = false;
            foreach (var componentRef in mechDef.Inventory)
            {
                if (componentRef == null)
                {
                    continue;
                }

                var componentDef = componentRef.Def as HeatSinkDef;
                if (componentDef == null)
                {
                    continue;
                }

                if (componentDef.IsDouble())
                {
                    hasDouble = true;
                }
                else if (componentDef.IsSingle())
                {
                    hasSingle = true;
                }
                else if (componentDef.IsEngineCore())
                {
                    var engineRef = componentRef.GetEngineCoreRef(null);
                    if (engineRef == null)
                    {
                        continue;
                    }

                    if (engineRef.IsDHS)
                    {
                        hasDouble = true;
                    }
                    else
                    {
                        hasSingle = true;
                    }
                }

                if (hasSingle && hasDouble)
                {
                    errorMessages[MechValidationType.InvalidInventorySlots].Add("MIXED HEATSINKS: Standard and Double Heat Sinks cannot be mixed");
                    return;
                }
            }
        }

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
        internal static MechLabLocationWidgetOnMechLabDropPatch.Result DropCheck(
            MechLabLocationWidget widget,
            MechLabPanel mechLab,
            MechLabItemSlotElement dragItem,
            List<MechLabItemSlotElement> localInventory)
        {
            if (widget.loadout.Location != ChassisLocations.CenterTorso)
            {
                return null;
            }

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

            var engineSlotElement = localInventory
                .FirstOrDefault(x => x != null && x.ComponentRef != null && x.ComponentRef.Def != null && x.ComponentRef.Def.IsEngineCore());

            if (engineSlotElement == null)
            {
                if (headSinkDef.IsDHSKit())
                {
                    return new MechLabLocationWidgetOnMechLabDropPatch.ErrorResult(
                        string.Format("Cannot add {0}: No Engine found", newComponentDef.Description.Name)
                    );
                }
                else
                {
                    return null;
                }
            }

            var engineRef = localInventory.Where(c => c != null).Select(c => c.ComponentRef).GetEngineCoreRef();

            if (mechLab.IsSimGame)
            {
                if (dragItem.OriginalDropParentType != MechLabDropTargetType.InventoryList)
                {
                    return new MechLabLocationWidgetOnMechLabDropPatch.ErrorResult(
                        string.Format("Cannot add {0}: Item has to be from inventory", newComponentDef.Description.Name)
                    );
                }

                if (mechLab.originalMechDef.Inventory.Any(c => c.SimGameUID == engineRef.ComponentRef.SimGameUID))
                {
                    return new MechLabLocationWidgetOnMechLabDropPatch.ErrorResult(
                        string.Format("Cannot add {0}: Engine cannot be modified once installed, remove engine first", newComponentDef.Description.Name)
                    );
                }
            }

            if (headSinkDef.IsDHSKit())
            {
                if (engineRef.IsDHS)
                {
                    return new MechLabLocationWidgetOnMechLabDropPatch.ErrorResult(
                        string.Format("Cannot add {0}: Already a DHS engine", newComponentDef.Description.Name)
                    );
                }

                if (!Control.settings.AllowMixingDoubleAndSingleHeatSinks && engineRef.AdditionalSHSCount > 0)
                {
                    return new MechLabLocationWidgetOnMechLabDropPatch.ErrorResult(
                        string.Format("Cannot add {0}: Reinstall engine to remove additional heat sinks before converting", newComponentDef.Description.Name)
                    );
                }

                engineRef.IsDHS = true;
            }
            else
            {
                int minHeatSinks, maxHeatSinks;
                Control.calc.CalcHeatSinks(engineRef.CoreDef, out minHeatSinks, out maxHeatSinks);

                if (engineRef.AdditionalHeatSinkCount >= maxHeatSinks - minHeatSinks)
                {
                    return null;
                    //return new MechLabLocationWidgetOnMechLabDropPatch.ErrorResult(
                    //    string.Format("Cannot add {0}: Maximum additional heat sinks reached for engine", newComponentDef.Description.Name)
                    //);
                }

                if (!Control.settings.AllowMixingDoubleAndSingleHeatSinks)
                {
                    if (engineRef.IsDHS && headSinkDef.IsSingle() || engineRef.IsSHS && headSinkDef.IsDouble())
                    {
                        return new MechLabLocationWidgetOnMechLabDropPatch.ErrorResult(
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

            return new MechLabLocationWidgetOnMechLabDropPatch.RemoveItemResult();
        }
    }
}