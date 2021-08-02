using System.Collections.Generic;
using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using CustomComponents;
using UnityEngine;

namespace MechEngineer.Features.MechLabSlots
{
    internal static class CustomWidgetsFixLanceMechEquipment
    {
        internal static GameObject TopLeft;
        internal static GameObject TopRight;

        public static void SetupContainers(GameObject template)
        {
            void Setup(MechLabSlotsSettings.WidgetSettings settings, ref GameObject go)
            {
                go = Object.Instantiate(template, null);
                go.name = settings.ShortLabel;
                var labelGo = go.transform.GetChild("CT-txt").gameObject;
                labelGo.name = go.name + "-txt";
                labelGo.GetComponent<LocalizableText>().SetText(settings.ShortLabel);
                go.transform.SetParent(template.transform.parent, false);
                go.transform.SetAsFirstSibling();
            }

            Setup(MechLabSlotsFeature.settings.TopLeftWidget, ref TopLeft);
            Setup(MechLabSlotsFeature.settings.TopRightWidget, ref TopRight);
        }

        public static void SetLoadout(MechDef mechDef, DataManager dataManager, List<GameObject> allComponents)
        {
            var topLeft = new List<MechComponentRef>();
            var topRight = new List<MechComponentRef>();
            foreach (var componentRef in mechDef.Inventory)
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

            void Setup(GameObject widget, List<MechComponentRef> list)
            {
                foreach (var mechComponentRef in list)
                {
                    var gameObject = dataManager.PooledInstantiate(
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
                    allComponents.Add(gameObject);
                }

                widget.SetActive(list.Count > 0);
            }

            Setup(TopLeft, topLeft);
            Setup(TopRight, topRight);
        }
    }
}