using BattleTech.UI;
using MechEngineer.Features.ArmorMaximizer.Maximizer;
using MechEngineer.Features.ArmorStructureRatio;
using MechEngineer.Features.MechLabSlots;
using UnityEngine;
using MechEngineer.Features.OverrideTonnage;
using MechEngineer.Helper;

namespace MechEngineer.Features.ArmorMaximizer;

internal static class ArmorMaximizerHandler
{
    internal static void OnStripArmor(MechLabPanel mechLabPanel)
    {
        if (!MechArmorState.Strip(mechLabPanel.activeMechDef, InputUtils.ControlModifierPressed, out var updates))
        {
            return;
        }

        foreach (var update in updates)
        {
            var widget = mechLabPanel.GetLocationWidget(update.Location);
            widget.SetArmor(update.Location.IsRear(), update.Assigned);
            Control.Logger.Trace?.Log($"OnStripArmor.SetArmor update={update}");
        }

        mechLabPanel.FlagAsModified();
        mechLabPanel.ValidateLoadout(false);
    }

    internal static void OnMaxArmor(MechLabPanel mechLabPanel, MechLabMechInfoWidget infoWidget)
    {
        if (!MechArmorState.Maximize(mechLabPanel.activeMechDef, InputUtils.ControlModifierPressed, ArmorUtils.ArmorPerStep, out var updates))
        {
            return;
        }

        foreach (var update in updates)
        {
            var widget = mechLabPanel.GetLocationWidget(update.Location);
            widget.SetArmor(update.Location.IsRear(), update.Assigned);
            Control.Logger.Trace?.Log($"OnMaxArmor.SetArmor update={update}");
        }

        infoWidget.RefreshInfo();
        mechLabPanel.FlagAsModified();
        mechLabPanel.ValidateLoadout(false);
    }

    internal static void OnArmorAddOrSubtract(MechLabLocationWidget widget, bool isRearArmor, float direction)
    {
        var stepPrecision = ArmorMaximizerFeature.Shared.Settings.StepPrecision.Get() ?? ArmorUtils.ArmorPerStep;
        var stepSize = ArmorMaximizerFeature.Shared.Settings.StepSize.Get() ?? ArmorUtils.ArmorPerStep;

        var stepDirection = direction < 0 ? -1 : 1;
        var current = isRearArmor ? widget.currentRearArmor : widget.currentArmor;

        var updated = stepDirection > 0
            ? PrecisionUtils.RoundUp(current + stepSize, stepPrecision)
            : PrecisionUtils.RoundDown(current - stepSize, stepPrecision);

        Control.Logger.Trace?.Log($"HandleArmorUpdate stepDirection={stepDirection} current={current} precision={stepPrecision} isRearArmor={isRearArmor}");
        if (stepDirection > 0)
        {
            var max = isRearArmor ? widget.maxRearArmor : widget.maxArmor;
            updated = Mathf.Min(updated, max);

            var maxTotal = ArmorUtils.GetMaximumArmorPoints(widget.chassisLocationDef);
            var maxOther = maxTotal - updated;
            var currentOther = isRearArmor ? widget.currentArmor : widget.currentRearArmor;
            var updatedOther = Mathf.Min(currentOther,maxOther);

            var otherChanged = !PrecisionUtils.Equals(currentOther, updatedOther);
            if (!otherChanged || !ArmorLocationLocker.IsLocked(widget.loadout.Location, !isRearArmor))
            {
                widget.SetArmor(isRearArmor, updated);
                if (otherChanged)
                {
                    widget.SetArmor(!isRearArmor, updatedOther);
                }
            }

            Control.Logger.Trace?.Log($"HandleArmorUpdate updated={updated} maxTotal={maxTotal} maxOther={maxOther} currentOther={currentOther} updatedOther={updatedOther} isRearArmor={updatedOther}");
        }
        else
        {
            updated = Mathf.Max(updated, 0);
            widget.SetArmor(isRearArmor, updated);
            Control.Logger.Trace?.Log($"HandleArmorUpdate updated={updated}");
        }
    }

    internal static void OnRefreshArmor(MechLabLocationWidget widget)
    {
        void RefreshArmorBar(LanceStat lanceStat, bool isRearArmor)
        {
            lanceStat.SetTextColor(UIColor.White, UIColor.White);
            RefreshBarColor(widget, isRearArmor);

            void SetButtonColor(string buttonId, UIColor uiColor)
            {
                var button = lanceStat.transform.Find(buttonId);
                // the plus icon is actually made of two minus icons
                var icons = button.Find("startButtonFill").GetChildren();
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

    internal static void OnBarClick(MechLabLocationWidget widget, bool isRearArmor)
    {
        ArmorLocationLocker.ToggleLock(widget.loadout.Location, isRearArmor);
        RefreshBarColor(widget, isRearArmor);
    }

    private static void RefreshBarColor(MechLabLocationWidget widget, bool isRearArmor)
    {
        var isLocked = ArmorLocationLocker.IsLocked(widget.loadout.Location, isRearArmor);
        var lanceStat = isRearArmor ? widget.rearArmorBar : widget.armorBar;
        lanceStat.fillColor.SetUIColor(isLocked ? UIColor.Gold : UIColor.White);
        lanceStat.nameTextColor.SetUIColor(isLocked ? UIColor.Gold : UIColor.White);
    }
}
