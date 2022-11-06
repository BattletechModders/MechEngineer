using System;
using System.Collections.Generic;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using HBS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MechEngineer.Features.TagManager;

internal class OptionsPanel
{
    private const float MaxWidth = 1600f;
    private const float MaxHeight = 800f;

    internal OptionsPanel(Action<TagManagerSettings.TagsFilterSet> loadCallback)
    {
        LoadCallback = loadCallback;

        {
            var module = LazySingletonBehavior<UIManager>.Instance.GetOrCreatePopupModule<GenericPopup>();
            _root = Object.Instantiate(module.gameObject, UIManager.Instance.popupNode.nodeTransform);
            _root.GetComponent<GenericPopup>().OnPooled();
            Object.DestroyImmediate(_root.GetComponent<GenericPopup>());
            _background = _root.transform.Find("Representation/secondLayerBackfill-CONDITIONAL").gameObject;
            _expanderViewport = _root.transform.Find("Representation/ExpanderViewport").gameObject;
            _layout = _expanderViewport.transform.Find("popupContainerLayout").gameObject;
            _input = _layout.transform.Find("InputField_content").gameObject;
            _inputTitle = _input.transform.Find("text_inputTitle").gameObject;
            _inputField = _input.transform.Find("uixPrfField_inputField").gameObject;
            _container = _layout.transform.Find("popup_buttonLayout").gameObject;
            _buttonCancelTemplate = Object.Instantiate(module._prefabButtons[0].gameObject, _layout.transform);
            _buttonTemplate = Object.Instantiate(module._prefabButtons[1].gameObject, _layout.transform);
            module.Pool();
        }

        {
            _root.SetActive(false);
            _root.name = "MechEngineer_TagsSelectionPanel";
            {
                _background.SetActive(true);
                _background.GetComponent<UIColorRefTracker>().SetUIColor(UIColor.Black);
            }
            _expanderViewport.GetComponent<RectTransform>().sizeDelta = new(MaxWidth, MaxHeight);

            var layoutTransform = _layout.transform;
            Object.DestroyImmediate(layoutTransform.Find("bgFill").gameObject);
            Object.DestroyImmediate(layoutTransform.Find("popUpTitle").gameObject);
            Object.DestroyImmediate(layoutTransform.Find("popup_subtitle-Optional").gameObject);
            Object.DestroyImmediate(layoutTransform.Find("Text_content").gameObject);

            _input.SetActive(true);
            _inputTitleText = _inputTitle.GetComponent<LocalizableText>();
            var inputFieldComponent = _inputField.GetComponent<HBS_InputField>();
            inputFieldComponent.characterValidation = HBS_InputField.CharacterValidation.None;
            inputFieldComponent.richText = false;
            inputFieldComponent.onValueChanged.AddListener(SetSearchText);

            for (var i = 0; i < 3; i++)
            {
                foreach (Transform child in _container.transform)
                {
                    var go = child.gameObject;
                    go.SetActive(false);
                    Object.DestroyImmediate(go);
                }
            }

            Object.DestroyImmediate(_container.GetComponent<HorizontalLayoutGroup>());
            Object.DestroyImmediate(_container.GetComponent<Image>());

            {
                _container.GetComponent<RectTransform>().sizeDelta = new(MaxWidth, MaxHeight);
                _container.SetActive(true);
                var vlg = _container.AddComponent<VerticalLayoutGroup>();
                vlg.enabled = true;
                vlg.childAlignment = TextAnchor.MiddleLeft;
                vlg.childControlHeight = false;
                vlg.childControlWidth = false;
                vlg.childForceExpandHeight = false;
                vlg.childForceExpandWidth = false;
                vlg.spacing = 10;

                var csf = _container.AddComponent<ContentSizeFitter>();
                csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                csf.enabled = true;
            }
        }

        {
            _buttonTemplate.SetActive(false);
            _buttonTemplate.name = "ButtonTemplate";

            var csf = _buttonTemplate.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            csf.enabled = true;
        }

        {
            _buttonCancelTemplate.SetActive(false);
            _buttonCancelTemplate.name = "ButtonCancelTemplate";

            var csf = _buttonCancelTemplate.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            csf.enabled = true;
        }

        {
            var module = LazySingletonBehavior<UIManager>.Instance.GetOrCreatePopupModule<SimGameDifficultySettingsModule>();
            _toggleTemplate = Object.Instantiate(module.togglePrefab.gameObject, _layout.transform);
            module.Pool();

            _toggleTemplate.SetActive(false);
            _toggleTemplate.name = "ToggleTemplate";

            Object.DestroyImmediate(_toggleTemplate.transform.Find("warn-icon").gameObject);
            var sgdsToggle = _toggleTemplate.GetComponent<SGDSToggle>();
            Object.DestroyImmediate(sgdsToggle.careerScoreModText.gameObject);

            {
                var rect = sgdsToggle.GetComponent<RectTransform>();
                rect.pivot = new(0.5f, 0.5f);
                rect.anchoredPosition = new(0.5f, 0.5f);
                rect.anchorMin = new(0, 0);
                rect.anchorMax = new(1, 1);
                rect.sizeDelta = new(190, 50);
            }

            {
                var toggle = sgdsToggle.toggle;
                var rect = toggle.GetComponent<RectTransform>();
                rect.pivot = new(1, 0.5f);
                rect.anchoredPosition = new(-10, 0);
                rect.anchorMin = new(1, 0.5f);
                rect.anchorMax = new(1, 0.5f);
                rect.sizeDelta = new(32, 32);
            }

            {
                var header = sgdsToggle.header;
                header.SetText("HEADER");
                var rect = header.GetComponent<RectTransform>();
                rect.pivot = new(0, 0.5f);
                rect.anchoredPosition = new(10, 0);
                rect.anchorMin = new(0, 0.5f);
                rect.anchorMax = new(0, 0.5f);
                rect.sizeDelta = new(150, 40);
            }
        }

        {
            _filterPanelGroup = CreateFilterGroup("Actions");
            var hlg = _filterPanelGroup.GetComponent<HorizontalLayoutGroup>();
            // fix for issues with buttons
            hlg.spacing = 20;
            hlg.padding.bottom = 10;
        }

        {
            _filterPresetsGroup = CreateFilterGroup("'Quick' Load");
            var hlg = _filterPresetsGroup.GetComponent<HorizontalLayoutGroup>();
            // fix for issues with buttons
            hlg.spacing = 20;
            hlg.padding.bottom = 10;
        }

        AddButton(_buttonCancelTemplate, _filterPanelGroup, "Cancel", () => {});
        AddButton(_buttonTemplate, _filterPanelGroup, "Load", AssemblyFilterAndLoadCallback);
        {
            void AddPreset(TagManagerSettings.TagsFilterSet option)
            {
                var queries = new FilterQueries(option);
                AddButton($"{option.Label} ({queries.MechCount})", () => CheckFilteredCountAndLoadCallback(option));
            }

            var settings = TagManagerFeature.Shared.Settings;
            AddPreset(settings.SkirmishDefault);
            foreach (var preset in settings.SkirmishPresets)
            {
                AddPreset(preset);
            }

            foreach (var group in settings.SkirmishOptionsGroups)
            {
                AddOptionGroup(group);
            }
        }

        SetSearchText(null);
    }

    private void SetSearchText(string? text)
    {
        _searchText = text;
        RefreshCount();
    }

    private void RefreshCount()
    {
        var count = new FilterQueries(CreateFilterSet()).MechCount;
        _inputTitleText.SetText($"Filter by Tag ... {count} found");
        Control.Logger.Trace?.Log("Input Tag Search yielded {count} results: " + _searchText);
    }

    private string? _searchText;
    private Action<TagManagerSettings.TagsFilterSet> LoadCallback { get; }
    private void AssemblyFilterAndLoadCallback()
    {
        var filter = CreateFilterSet();
        CheckFilteredCountAndLoadCallback(filter);
    }

    private void CheckFilteredCountAndLoadCallback(TagManagerSettings.TagsFilterSet filter)
    {
        var count = new FilterQueries(filter).MechCount;
        var max = TagManagerFeature.Shared.Settings.SkirmishOverloadWarning;
        if (count > max)
        {
            GenericPopupBuilder
                .Create(GenericPopupType.Warning, $"You are about to load more than {max} 'Mechs ({count}), this will reduce the performance until the game is restarted.")
                .AddButton("Cancel")
                .AddButton("Confirm", () => LoadCallbackAndHide(filter))
                .Render();
        }
        else
        {
            LoadCallbackAndHide(filter);
        }
    }

    private void LoadCallbackAndHide(TagManagerSettings.TagsFilterSet filter)
    {
        LoadCallback(filter);
        Hide();
    }

    private TagManagerSettings.TagsFilterSet CreateFilterSet()
    {
        var settings = TagManagerFeature.Shared.Settings;
        var defaults = settings.SkirmishOptionsDefault;
        defaults.Mechs.OptionsSearch = string.IsNullOrEmpty(_searchText) ? null : _searchText;
        defaults.Mechs.OptionsGroups = settings.SkirmishOptionsGroups;
        return defaults;
    }

    private readonly GameObject _root;
    private readonly GameObject _background;
    private readonly GameObject _expanderViewport;
    private readonly GameObject _layout;
    private readonly GameObject _input;
    private readonly GameObject _inputTitle;
    private readonly LocalizableText _inputTitleText;
    private readonly GameObject _inputField;
    private readonly GameObject _container;
    private readonly GameObject _buttonCancelTemplate;
    private readonly GameObject _buttonTemplate;
    private readonly GameObject _toggleTemplate;
    private readonly GameObject _filterPanelGroup;
    private readonly GameObject _filterPresetsGroup;

    internal void Show()
    {
        _root.gameObject.SetActive(true);
    }

    private void Hide()
    {
        _root.gameObject.SetActive(false);
    }

    private void AddButton(string label, Action action)
    {
        AddButton(_buttonTemplate, _filterPresetsGroup, label, action);
    }

    private void AddButton(GameObject template, GameObject parent, string label, Action action)
    {
        var buttonGo = Object.Instantiate(template, parent.transform);
        var hbsButton = buttonGo.GetComponent<HBSButton>();
        hbsButton.SetText(label);
        hbsButton.OnClicked.AddListener(() =>
        {
            action();
        });
        hbsButton.SetState(ButtonState.Enabled);
        buttonGo.SetActive(true);
    }

    private void AddOptionGroup(TagManagerSettings.TagOptionsGroup group)
    {
        foreach (var option in group.Options)
        {
            AddOption(group, option);
        }
    }

    private void AddOption(TagManagerSettings.TagOptionsGroup group, TagManagerSettings.TagOption option)
    {
        var groupLabel = group.Label;
        var toggleLabel = option.Label;
        if (!filterGroups.TryGetValue(groupLabel, out var rowGo))
        {
            rowGo = CreateFilterGroup(groupLabel);
            filterGroups[groupLabel] = rowGo;
        }

        var cellGo = Object.Instantiate(_toggleTemplate, rowGo.transform);
        cellGo.name = "Filter Toggle " + toggleLabel;
        var toggle = cellGo.GetComponent<SGDSToggle>();
        toggle.header.SetText(toggleLabel);
        toggle.toggle.SetToggled(option.OptionActive);
        toggle.toggle.onStateChange.AddListener(s =>
        {
            if (s == ButtonState.Selected)
            {
                if (!option.OptionActive)
                {
                    option.OptionActive = true;
                    RefreshCount();
                }
            }
            else if (s == ButtonState.Enabled)
            {
                if (option.OptionActive)
                {
                    option.OptionActive = false;
                    RefreshCount();
                }
            }
        });
        cellGo.SetActive(true);
    }

    private readonly Dictionary<string, GameObject> filterGroups = new();

    private GameObject CreateFilterGroup(string label)
    {
        var group = new GameObject();
        group.name = "Filter Group " + label;

        var rect = group.AddComponent<RectTransform>();
        rect.pivot = new(0, 0.5f);
        rect.anchoredPosition = new(10, 0);
        rect.anchorMin = new(0, 0.5f);
        rect.anchorMax = new(0, 0.5f);

        var hlg = group.AddComponent<HorizontalLayoutGroup>();
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.enabled = true;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlHeight = false;
        hlg.childControlWidth = false;
        hlg.childForceExpandHeight = false;
        hlg.childForceExpandWidth = false;
        hlg.spacing = 10;

        var csf = group.AddComponent<ContentSizeFitter>();
        csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        csf.enabled = true;

        {
            var labelGo = Object.Instantiate(_toggleTemplate.GetComponent<SGDSToggle>().header.gameObject, group.transform);
            var lt = labelGo.GetComponent<LocalizableText>();
            lt.SetText(label);
            lt.alignment = TextAlignmentOptions.Right;
            // lt.fontSize = lt.fontSizeMax = lt.fontSizeMin = 20;
            lt.fontStyle = FontStyles.Bold;

            var rect2 = group.GetComponent<RectTransform>();
            rect.pivot = new(0, 0.5f);
            rect.anchoredPosition = new(10, 0);
            rect.anchorMin = new(0, 0.5f);
            rect.anchorMax = new(0, 0.5f);
            rect2.sizeDelta = new(130, 40);

            labelGo.SetActive(true);
        }

        group.transform.SetParent(_container.transform);
        group.SetActive(true);
        return group;
    }
}