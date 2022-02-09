using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using CustomComponents;
using Localize;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MechEngineer.Features.MechLabSlots
{
    internal static class CustomWidgetsFixLanceMechEquipment
    {
        public static void Awake(LocalizableText centerTorsoLabel)
        {
            var centerTorso = centerTorsoLabel.transform.parent.gameObject;
            void Setup(MechLabSlotsSettings.WidgetSettings settings)
            {
                var go = GetWidgetViaCenterTorso(settings, centerTorso);
                if (go != null)
                {
                    return;
                }
                go = Object.Instantiate(centerTorso, null);
                go.name = settings.ShortLabel; // required for identification, needs to be unique
                var labelGo = go.transform.GetChild("CT-txt").gameObject;
                labelGo.name = go.name + "-txt";
                labelGo.GetComponent<LocalizableText>().SetText(settings.ShortLabel);
                go.transform.SetParent(centerTorso.transform.parent, false);
                go.transform.SetAsFirstSibling();
            }

            Setup(MechLabSlotsFeature.settings.TopLeftWidget);
            Setup(MechLabSlotsFeature.settings.TopRightWidget);
        }

        public static void SetLoadout_LabelDefaults(LanceMechEquipmentList el)
        {
            SetupWidgetSections(el);

            void SetLabel(LocalizableText label, ChassisLocations location)
            {
                var text = Mech.GetAbbreviatedChassisLocation(location);
                label.SetText(text);
            }
            SetLoadout(el, SetLabel);
        }

        public static void SetLoadout_LabelOverrides(LanceMechEquipmentList el)
        {
            var chassisDef = el.activeMech.Chassis;
            if (!chassisDef.Is<ChassisLocationNaming>(out var naming))
            {
                return;
            }

            void SetLabel(LocalizableText label, ChassisLocations location)
            {
                var shortLabel = naming.Names
                    .Where(x => x.Location == location)
                    .Select(x => x.ShortLabel)
                    .FirstOrDefault();

                if (shortLabel != null)
                {
                    var text = new Text(shortLabel);
                    label.SetText(text);
                }
            }
            SetLoadout(el, SetLabel);
        }

        // have to iterate ourselves, in case CustomUnits skips the vanilla iteration
        private static void SetLoadout(LanceMechEquipmentList list, Action<LocalizableText, ChassisLocations> setLabel)
        {
            setLabel(list.headLabel, ChassisLocations.Head);
            setLabel(list.leftArmLabel, ChassisLocations.LeftArm);
            setLabel(list.leftTorsoLabel, ChassisLocations.LeftTorso);
            setLabel(list.centerTorsoLabel, ChassisLocations.CenterTorso);
            setLabel(list.rightTorsoLabel, ChassisLocations.RightTorso);
            setLabel(list.rightArmLabel, ChassisLocations.RightArm);
            setLabel(list.leftLegLabel, ChassisLocations.LeftLeg);
            setLabel(list.rightLegLabel, ChassisLocations.RightLeg);
        }

        private static void SetupWidgetSections(LanceMechEquipmentList el)
        {
            var centerTorso = el.centerTorsoLabel.transform.parent.gameObject;

            var topLeft = new List<MechComponentRef>();
            var topRight = new List<MechComponentRef>();
            foreach (var componentRef in el.activeMech.Inventory)
            {
                if (componentRef.Flags<CCFlags>().HideFromEquip)
                {
                    continue;
                }

                if (!componentRef.Is<CustomWidget>(out var widget))
                {
                    continue;
                }

                switch (widget.Location)
                {
                    case CustomWidget.MechLabWidgetLocation.TopLeft:
                        topLeft.Add(componentRef);
                        break;
                    case CustomWidget.MechLabWidgetLocation.TopRight:
                        topRight.Add(componentRef);
                        break;
                }
            }

            void Setup(MechLabSlotsSettings.WidgetSettings settings, List<MechComponentRef> list)
            {
                var widget = GetWidgetViaCenterTorso(settings, centerTorso);
                foreach (var mechComponentRef in list)
                {
                    var gameObject = el.dataManager.PooledInstantiate(
                        "uixPrfPanl_LC_MechLoadoutItem",
                        BattleTechResourceType.UIModulePrefabs
                    );
                    var component = gameObject.GetComponent<LanceMechEquipmentListItem>();
                    var bgColor = MechComponentRef.GetUIColor(mechComponentRef);
                    if (mechComponentRef.DamageLevel == ComponentDamageLevel.Destroyed)
                    {
                        bgColor = UIColor.Disabled;
                    }

                    component.SetData(
                        mechComponentRef.Def.Description.UIName,
                        mechComponentRef.DamageLevel,
                        UIColor.White,
                        bgColor
                    );
                    component.SetTooltipData(mechComponentRef.Def);
                    gameObject.transform.SetParent(widget.transform, false);
                    el.allComponents.Add(gameObject);
                }

                widget.SetActive(list.Count > 0);
            }

            Setup(MechLabSlotsFeature.settings.TopLeftWidget, topLeft);
            Setup(MechLabSlotsFeature.settings.TopRightWidget, topRight);
        }

        private static GameObject GetWidgetViaCenterTorso(MechLabSlotsSettings.WidgetSettings settings, GameObject centerTorso)
        {
            return centerTorso.transform.parent.GetChild(settings.ShortLabel)?.gameObject;
        }
    }
}