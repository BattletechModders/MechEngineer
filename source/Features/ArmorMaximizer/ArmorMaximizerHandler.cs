using System.Linq;
using BattleTech;
using BattleTech.UI;
using MechEngineer.Features.DynamicSlots;
using UnityEngine;
using MechEngineer.Features.OverrideTonnage;

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

        var skipArmsAndLegs = false;
        var armsAndLegsLocation = ChassisLocations.Arms | ChassisLocations.Legs;

        while (state.Remaining >= 1)
        {
            // 4 that always get called
            // 4 that only get called every odd numbers
            // 2 iterations: 2x4 + 1x4
            var stepSize = state.Remaining >= (2*4 + 1*4) * 5 ? 5 : 1;

            foreach (var location in MechDefBuilder.Locations)
            {
                Control.Logger.Trace?.Log($"location={location} state.Remaining={state.Remaining} stepSize={stepSize} skipArmsAndLegs={skipArmsAndLegs}");

                if (state.Remaining < stepSize)
                {
                    break;
                }

                if (skipArmsAndLegs && (location & armsAndLegsLocation) != ChassisLocations.None)
                {
                    continue;
                }

                var locationState = state.Locations[location];
                Control.Logger.Trace?.Log($"location={location} locationState={locationState}");

                if (locationState.IsFull)
                {
                    continue;
                }

                locationState.Assigned += stepSize;
                state.Remaining -= stepSize;

                Control.Logger.Trace?.Log($"location={location} locationState={locationState}");
            }

            skipArmsAndLegs = !skipArmsAndLegs;
        }

        void SetArmor(ChassisLocations location)
        {
            var widget = mechLabPanel.GetLocationWidget(location);
            var locationState = state.Locations[location];
            if ((location & ChassisLocations.Torso) != ChassisLocations.None)
            {
                var front = PrecisionUtils.RoundUp(locationState.Assigned * settings.TorsoFrontBackRatio, 1f);
                if (PrecisionUtils.SmallerThan(5f, locationState.Assigned))
                {
                    front = PrecisionUtils.RoundDown(front, 5);
                }
                var rear = locationState.Assigned - front;
                widget.SetArmor(false, front, true);
                widget.SetArmor(true, rear, true);
            }
            else
            {
                widget.SetArmor(false, locationState.Assigned);
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
        enforcedArmor = ArmorUtils.RoundDown(enforcedArmor, 5);
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
                    widget.SetArmor(isRearArmor, currentArmor, true);
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