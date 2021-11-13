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

        public static void SetLoadout(LocalizableText centerTorsoLabel, MechDef mechDef, DataManager dataManager, List<GameObject> allComponents)
        {
            var centerTorso = centerTorsoLabel.transform.parent.gameObject;

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

            void Setup(MechLabSlotsSettings.WidgetSettings settings, List<MechComponentRef> list)
            {
                var widget = GetWidgetViaCenterTorso(settings, centerTorso);
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

            Setup(MechLabSlotsFeature.settings.TopLeftWidget, topLeft);
            Setup(MechLabSlotsFeature.settings.TopRightWidget, topRight);
        }

        private static GameObject GetWidgetViaCenterTorso(MechLabSlotsSettings.WidgetSettings settings, GameObject centerTorso)
        {
            return centerTorso.transform.parent.GetChild(settings.ShortLabel)?.gameObject;
        }
    }
}