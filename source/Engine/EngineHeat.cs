using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MechEngineMod
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
            var mixed = mechDef.Inventory
                .Where(c => c.ComponentDefType == ComponentType.HeatSink)
                .Select(c => c.Def as HeatSinkDef)
                .Where(c => c != null)
                .Select(cd =>
                {
                    if (cd.IsSingle())
                    {
                        hasSingle = true;
                    }
                    else if (cd.IsDouble())
                    {
                        hasDouble = true;
                    }
                    return cd;
                })
                .Any(c => hasSingle && hasDouble);

            if (mixed)
            {
                errorMessages[MechValidationType.InvalidInventorySlots].Add("MIXED HEATSINKS: Standard and Double Heat Sinks cannot be mixed");
            }
        }

        internal static float GetEngineHeatDissipation(MechComponentRef[] inventory)
        {
            var componentRef = inventory.SingleOrDefault(x => x.Def.IsMainEngine());
            if (componentRef == null)
            {
                return Control.settings.FallbackHeatSinkCount * Control.Combat.Heat.DefaultHeatSinkDissipationCapacity;
            }

            var engineRef = componentRef.GetEngineRef();
            return engineRef.GetEngineHeatDissipation();
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

            var existingEngine = localInventory
                .Where(x => x != null)
                .Select(x => x.ComponentRef)
                .FirstOrDefault(x => x != null && x.Def != null && x.Def.IsMainEngine());

            if (existingEngine == null)
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

            var engineRef = existingEngine.GetEngineRef();

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
                    return null;
                    //return new MechLabLocationWidgetOnMechLabDropPatch.ErrorResult(
                    //    string.Format("Cannot add {0}: Reinstall engine to remove additional heat sinks before converting", newComponentDef.Description.Name)
                    //);
                }

                engineRef.IsDHS = true;
            }
            else
            {
                int minHeatSinks, maxHeatSinks;
                Control.calc.CalcHeatSinks(engineRef.engineDef, out minHeatSinks, out maxHeatSinks);

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

            return new MechLabLocationWidgetOnMechLabDropPatch.RemoveItemResult();
        }
    }
}