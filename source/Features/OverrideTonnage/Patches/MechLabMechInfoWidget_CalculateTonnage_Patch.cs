using System;
using BattleTech.UI;
using Harmony;
using TMPro;
using UnityEngine;

namespace MechEngineer.Features.OverrideTonnage.Patches
{
    [HarmonyPatch(typeof(MechLabMechInfoWidget), "CalculateTonnage")]
    public static class MechLabMechInfoWidget_CalculateTonnage_Patch
    {
        public static void Postfix(
            MechLabPanel ___mechLab,
            ref float ___currentTonnage,
            TextMeshProUGUI ___totalTonnage,
            UIColorRefTracker ___totalTonnageColor,
            TextMeshProUGUI ___remainingTonnage,
            UIColorRefTracker ___remainingTonnageColor)
        {
            try
            {
                var totalTonnage = ___totalTonnage;
                var mechLab = ___mechLab;
                ref var currentTonnage = ref ___currentTonnage;
                var totalTonnageColor = ___totalTonnageColor;
                var remainingTonnage = ___remainingTonnage;
                var remainingTonnageColor = ___remainingTonnageColor;

                var mechDef = mechLab.CreateMechDef();
                if (mechDef == null)
                {
                    return;
                }

                currentTonnage += WeightsHandler.Shared.TonnageChanges(mechDef);

                var precisionHelper = InfoTonnageHelper.KilogramStandard;

                var maxTonnage = mechDef.Chassis.Tonnage;

                if (precisionHelper.IsSmaller(maxTonnage, currentTonnage))
                {
                    totalTonnageColor.SetUIColor(UIColor.Red);
                    remainingTonnageColor.SetUIColor(UIColor.Red);
                }
                else
                {
                    totalTonnageColor.SetUIColor(UIColor.WhiteHalf);
                    if (precisionHelper.IsSmaller(maxTonnage, currentTonnage + OverrideTonnageFeature.settings.UnderweightWarningThreshold))
                    {
                        remainingTonnageColor.SetUIColor(UIColor.White);
                    }
                    else
                    {
                        remainingTonnageColor.SetUIColor(UIColor.Gold);
                    }
                }
            
                totalTonnage.SetText(string.Format("{0} / {1}", InfoTonnageHelper.TonnageStandard.AsString(currentTonnage), maxTonnage));
                if (precisionHelper.IsSmaller(maxTonnage, currentTonnage, out var tonnageLeft))
                {
                    tonnageLeft = Mathf.Abs(tonnageLeft);
                    var left = precisionHelper.AsString(tonnageLeft);
                    var s = precisionHelper.IsSame(tonnageLeft, 1f) ? "s" : string.Empty;
                    remainingTonnage.SetText(string.Format("{0} ton{1} overweight", left, s));
                }
                else
                {
                    var left = precisionHelper.AsString(tonnageLeft);
                    var s = precisionHelper.IsSame(tonnageLeft, 1f) ? "s" : string.Empty;
                    remainingTonnage.SetText(string.Format("{0} ton{1} remaining", left, s));
                }
            }
            catch (Exception e)
            {
                Control.Logger.Error.Log(e);
            }
        }

        internal class InfoTonnageHelper
        {
            internal float Precision { get; }

            internal static InfoTonnageHelper KilogramStandard => new InfoTonnageHelper(OverrideTonnageFeature.settings.KilogramStandardPrecision);
            internal static InfoTonnageHelper TonnageStandard => new InfoTonnageHelper(OverrideTonnageFeature.settings.TonnageStandardPrecision);

            private InfoTonnageHelper(float precision)
            {
                Precision = precision;
            }

            private float Round(float value)
            {
                return PrecisionUtils.RoundUp(value, Precision);
            }

            internal bool IsSame(float a, float b)
            {
                return PrecisionUtils.Equals(a, b, Precision / 2);
            }

            internal bool IsSmaller(float a, float b)
            {
                if (IsSame(a, b))
                {
                    return false;
                }
                return a < b;
            }

            internal string AsString(float number)
            {
                var rounded = PrecisionUtils.RoundUp(number, Precision);
                var digits = new string('#', OverrideTonnageFeature.settings.MechLabMechInfoWidgetDecimalPlaces);
                return string.Format("{0:0." + digits +"}", rounded);
            }

            public bool IsSmaller(float a, float b, out float left)
            {
                if (IsSame(a, b))
                {
                    left = 0f;
                    return false;
                }

                left = Round(a) - Round(b);
                return left < 0;
            }
        }
    }
}