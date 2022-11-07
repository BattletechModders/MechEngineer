using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using BattleTech.UI.Tooltips;
using MechEngineer.Features.MechLabSlots;
using MechEngineer.Helper;
using SVGImporter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MechEngineer.Features.CustomCapacities;

internal class CustomCapacityUIElement : MonoBehaviour
{
    internal void SetData(BaseDescriptionDef description, string text, UIColor color)
    {
        Description = description;
        Text = text;
        Color = color;
        Refresh();
    }

    private BaseDescriptionDef Description = new()
    {
        Id = "Id",
        Name = "Name",
        Details = "Details",
        Icon = "UixSvgIcon_specialEquip_Melee"
    };
    private string Text = "Text";
    private UIColor Color = UIColor.White;

    private void Refresh()
    {
        if (_tooltip != null)
        {
            _tooltip.defaultStateData.SetObject(Description);
        }
        if (_label != null)
        {
            _label.SetText(Text);
        }
        if (_color != null)
        {
            _color.SetUIColor(Color);
        }
        if (_icon != null)
        {
            _icon.vectorGraphics = DataManager.SVGCache.GetAsset(Description.Icon);
        }
    }

    private HBSTooltip? _tooltip;
    private SVGImage? _icon;
    private LocalizableText? _label;
    private UIColorRefTracker? _color;

    private static DataManager DataManager => UnityGameInstance.BattleTechGame.DataManager;

    // since the layout engine from unity sucks we have to do it manually
    private float Height = 24;
    private float MaxWidth = 100 + MechLabLayoutUtils.Shifted;
    private float LeftPad = 8;
    private float TextWidth => MaxWidth - LeftPad - PadBetweenTextAndIcon - IconWidth - PadRight;
    private float PadBetweenTextAndIcon = 6;
    private float IconWidth = 25;
    private float PadRight = 8;
    private float TextAnchorX => - PadBetweenTextAndIcon - IconWidth - PadRight;
    private float IconAnchorX => - PadRight;

    private bool setup;
    public void Awake()
    {
        if (setup)
        {
            return;
        }
        setup = true;

        {
            var go = gameObject;

            go.AddComponent<Image>();

            {
                var component = go.AddComponent<UIColorRefTracker>();
                component.SetUIColor(UIColor.DarkGray);
            }

            {
                var component = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
                component.minHeight = component.preferredHeight = Height;
                component.minWidth = component.preferredWidth = MaxWidth;
            }

            {
                var component = go.AddComponent<ContentSizeFitter>();
                component.horizontalFit = component.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }

            _tooltip = go.AddComponent<HBSTooltip>();
        }

        {
            var go = new GameObject("text");

            _label = go.AddComponent<LocalizableText>();
            _label.autoSizeTextContainer = false;
            _label.enableAutoSizing = true;
            _label.fontSizeMin = 12;
            _label.fontSize = _label.fontSizeMax = 16;
            _label.fontStyle = FontStyles.Bold;
            _label.font = Fonts.MediumFont;
            _label.alignment = TextAlignmentOptions.MidlineRight;

            {
                var component = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
                component.ignoreLayout = true;
                component.minHeight = component.preferredHeight = Height;
                component.minWidth = component.preferredWidth = TextWidth;
            }

            {
                var component = go.AddComponent<ContentSizeFitter>();
                component.horizontalFit = component.verticalFit = ContentSizeFitter.FitMode.MinSize;
            }

            _color = go.AddComponent<UIColorRefTracker>();

            NormalizeRectTransform(go).anchoredPosition = new(TextAnchorX, 0);
            go.transform.SetParent(transform, false);
        }

        {
            var go = new GameObject("icon");

            _icon = go.AddComponent<SVGImage>();

            {
                var component = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
                component.ignoreLayout = true;
                component.minHeight = component.preferredHeight = IconWidth;
                component.minWidth = component.preferredWidth = IconWidth;
            }

            {
                var component = go.AddComponent<ContentSizeFitter>();
                component.horizontalFit = component.verticalFit = ContentSizeFitter.FitMode.MinSize;
            }

            NormalizeRectTransform(go).anchoredPosition = new(IconAnchorX, 0);
            go.transform.SetParent(transform, false);
        }
    }

    private static RectTransform NormalizeRectTransform(GameObject go)
    {
        var component = go.GetComponent<RectTransform>();
        component.anchorMin = new(1, 0.5f);
        component.anchorMax = new(1, 0.5f);
        component.pivot = new(1, 0.5f);
        component.anchoredPosition = new(0, 0);
        return component;
    }
}