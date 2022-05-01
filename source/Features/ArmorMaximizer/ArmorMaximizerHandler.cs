using System;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using MechEngineer.Features.ArmorStructureRatio;
using MechEngineer.Features.DynamicSlots;
using MechEngineer.Features.MechLabSlots;
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
        var changes = false;

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
                changes = true;

                Control.Logger.Trace?.Log($"OnMaxArmor location={location.GetShortString()} locationState={locationState}");
            }
        }

        if (!changes)
        {
            return;
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

    internal static void HandleArmorUpdate(MechLabLocationWidget widget, bool isRearArmor, float direction)
    {
        var precision = AltModifierPressed ? 1 : ArmorStructureRatioFeature.ArmorPerStep;
        var stepSize = ShiftModifierPressed ? 25 : (ControlModifierPressed ? 999 : 1);

        var stepDirection = direction < 0 ? -1 : 1;
        var current = isRearArmor ? widget.currentRearArmor : widget.currentArmor;

        var updated = stepDirection > 0
            ? PrecisionUtils.RoundUp(current + stepSize, precision)
            : PrecisionUtils.RoundDown(current - stepSize, precision);

        Control.Logger.Trace?.Log($"HandleArmorUpdate stepDirection={stepDirection} current={current} precision={precision} isRearArmor={isRearArmor}");
        if (stepDirection > 0)
        {
            var max = isRearArmor ? widget.maxRearArmor : widget.maxArmor;
            updated = Mathf.Min(updated, max);

            var maxTotal = ArmorStructureRatioFeature.GetMaximumArmorPoints(widget.chassisLocationDef);
            var maxOther = maxTotal - updated;
            var currentOther = isRearArmor ? widget.currentArmor : widget.currentRearArmor;
            var updatedOther = Mathf.Min(currentOther,maxOther);
            widget.SetArmor(!isRearArmor, updatedOther);
            Control.Logger.Trace?.Log($"HandleArmorUpdate maxTotal={maxTotal} maxOther={maxOther} currentOther={currentOther} updated={updatedOther} isRearArmor={updatedOther}");
        }
        else
        {
            updated = Mathf.Max(updated, 0);
        }
        Control.Logger.Trace?.Log($"HandleArmorUpdate updated={updated}");
        widget.SetArmor(isRearArmor, updated);
    }

    internal static void OnRefreshArmor(MechLabLocationWidget widget)
    {
        void RefreshArmorBar(LanceStat armorBar, bool isRearArmor) {
            armorBar.SetTextColor(UIColor.White, UIColor.White);

            void SetButtonColor(string buttonId, UIColor uiColor)
            {
                var button = armorBar.transform.GetChild(buttonId);
                // the plus icon is actually made of two minus icons
                var icons = button.GetChild("startButtonFill").GetChildren();
                foreach (var icon in icons)
                {
                    var colorRefTracker = icon.GetComponent<UIColorRefTracker>();
                    colorRefTracker.SetUIColor(uiColor);
                }
            }

            const UIColor limitReachedColor = UIColor.MedGray;
            {
                var max = isRearArmor ? widget.maxRearArmor : widget.maxArmor;
                var current = isRearArmor ? widget.currentRearArmor : widget.currentArmor;
                var maxReached = PrecisionUtils.SmallerOrEqualsTo(max, current);
                SetButtonColor("bttn_plus", maxReached ? limitReachedColor : UIColor.White);
            }
            {
                var minReached = PrecisionUtils.SmallerOrEqualsTo(isRearArmor ? widget.currentRearArmor : widget.currentArmor, 0);
                SetButtonColor("bttn_minus", minReached ? limitReachedColor : UIColor.White);
            }
        }

        RefreshArmorBar(widget.armorBar, false);
        if (widget.useRearArmor)
        {
            RefreshArmorBar(widget.rearArmorBar, true);
        }
    }

    private static bool ShiftModifierPressed => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    private static bool ControlModifierPressed => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
    private static bool AltModifierPressed => Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
}