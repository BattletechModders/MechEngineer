using System;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using MechEngineer.Features.ArmorStructureRatio;
using MechEngineer.Features.DynamicSlots;
using UnityEngine;
using MechEngineer.Features.OverrideTonnage;
using MechEngineer.Helper;

namespace MechEngineer.Features.ArmorMaximizer;

internal static class ArmorMaximizerHandler
{
    internal static void OnMaxArmor(MechLabPanel mechLabPanel, MechLabMechInfoWidget infoWidget)
    {
        var settings = ArmorMaximizerFeature.Shared.Settings;
        APState state = new(mechLabPanel.activeMechDef);
        if (MechDefBuilder.Locations.Any(location => mechLabPanel.GetLocationWidget(location).IsDestroyed))
        {
            return;
        }

        var locationStates = state.Locations.Values.ToArray();
        var stepSize = ArmorStructureRatioFeature.ArmorPerStep;

        while (state.Remaining >= stepSize)
        {
            Array.Sort(locationStates, (x, y) =>
            {
                var cmp = -(x.PriorityPrimary - y.PriorityPrimary);
                if (cmp != 0)
                {
                    return cmp;
                }

                return -(x.PrioritySecondary - y.PrioritySecondary);
            });

            var locationState = locationStates[0];
            {
                var location = locationState.Location;
                Control.Logger.Trace?.Log($"OnMaxArmor location={location.GetShortString()} state.Remaining={state.Remaining} stepSize={stepSize}");

                // with priority queues, we would just not re-queued any location that is Full
                if (locationState.IsFull)
                {
                    break;
                }

                locationState.Assigned += stepSize;
                state.Remaining -= stepSize;

                Control.Logger.Trace?.Log($"OnMaxArmor location={location.GetShortString()} locationState={locationState}");
            }
        }

        void SetArmor(ChassisLocations location)
        {
            var widget = mechLabPanel.GetLocationWidget(location);
            var locationState = state.Locations[location];
            if ((location & ChassisLocations.Torso) != ChassisLocations.None)
            {
                var front = PrecisionUtils.RoundDownToInt(locationState.Assigned * settings.TorsoFrontBackRatio);
                if (PrecisionUtils.SmallerThan(5f, locationState.Assigned))
                {
                    front = (int)PrecisionUtils.RoundDown(front, ArmorStructureRatioFeature.ArmorPerStep);
                }
                var rear = locationState.Assigned - front;
                widget.SetArmor(false, front);
                widget.SetArmor(true, rear);
                Control.Logger.Trace?.Log($"SetArmor Assigned={locationState.Assigned} Max={locationState.Max} front={front} rear={rear}");
            }
            else
            {
                widget.SetArmor(false, locationState.Assigned);
                Control.Logger.Trace?.Log($"SetArmor Assigned={locationState.Assigned} Max={locationState.Max}");
            }
        }

        foreach (var location in MechDefBuilder.Locations)
        {
            SetArmor(location);
        }

        infoWidget.RefreshInfo();
        mechLabPanel.FlagAsModified();
        mechLabPanel.ValidateLoadout(false);
    }

    internal static void HandleArmorUpdate(MechLabLocationWidget widget, bool isRearArmor, float amount)
    {
        var mechDef = widget.mechLab.activeMechDef;
        var tonsPerPoint = ArmorUtils.TonPerPointWithFactor(mechDef);
        var freeTonnage = Weights.CalculateFreeTonnage(mechDef);
        freeTonnage = ArmorUtils.RoundUp(freeTonnage, 0.0005f);
        var armorWeight = amount * tonsPerPoint;
        var ratio = widget.loadout.Location == ChassisLocations.Head ? 3 : 2;
        var enforcedArmor = widget.chassisLocationDef.InternalStructure;
        enforcedArmor = ArmorUtils.RoundDown(enforcedArmor, ArmorStructureRatioFeature.ArmorPerStep);
        enforcedArmor *= ratio;
        var currentArmor = widget.currentArmor;
        var currentRearArmor = widget.currentRearArmor;
        var maxArmor = enforcedArmor - currentArmor - currentRearArmor;
        var shiftModifierPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        var altModifierPressed = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        if (isRearArmor)
        {
            currentArmor = widget.currentRearArmor;
        }
        if (shiftModifierPressed)
        {
           amount *= 5f;
            var divisible = ArmorUtils.IsDivisible(currentArmor, amount);
            if (!divisible)
            {
                if (amount > 0)
                {
                    if (freeTonnage < tonsPerPoint) return;
                    var multWeight = armorWeight * amount;
                    if(multWeight > freeTonnage)
                    {
                        var freePoints = freeTonnage / tonsPerPoint;
                        freePoints = ArmorUtils.RoundDown(freePoints,1);
                        if (freePoints > 0)
                        {
                            currentArmor += freePoints;
                            if(freePoints > maxArmor)
                            {
                                currentArmor += maxArmor;
                            }
                            widget.SetArmor(isRearArmor, currentArmor, true);
                            return;
                        }
                        return;
                    }
                    currentArmor = ArmorUtils.RoundUp(currentArmor, amount);
                    widget.SetArmor(isRearArmor, currentArmor, true);
                    return;
                }
                if (amount < 0)
                {
                    if (currentArmor <= 0) return;
                    currentArmor = ArmorUtils.RoundDown(currentArmor, amount);
                    if(currentArmor < 0)
                    {
                        currentArmor = 0;
                    }
                    widget.SetArmor(isRearArmor, currentArmor);
                    return;
                }
            }
        }
        if (amount > 0)
        {
            if (freeTonnage < tonsPerPoint) return;
            if (maxArmor <= 0) return;
            var multWeight = armorWeight * amount;
            if (multWeight > freeTonnage)
            {
                var freePoints = freeTonnage / tonsPerPoint;
                freePoints = ArmorUtils.RoundDown(freePoints, 1);
                if (freePoints > 0)
                {
                    currentArmor += freePoints;
                    if (freePoints > maxArmor)
                    {
                        currentArmor += maxArmor;
                    }
                    widget.SetArmor(isRearArmor, currentArmor, true);
                    return;
                }
                return;
            }
        }
        if (amount < 0)
        {
            if (currentArmor <= 0) return;
        }
        widget.ModifyArmor(isRearArmor, amount, true);
    }
}