using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using HBS;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
            _root = Object.Instantiate(module.gameObject, UIManager.Instance.uiNode.nodeTransform);
            {
                var popup = _root.GetComponent<GenericPopup>();
                popup.OnPooled();
                popup.ShowInput(new GenericPopupInputSettings
                {
                    SampleText = "Search by Tag ... use comma to enter multiple terms and prepend ! to negate a term.",
                    InputFieldName = "",
                });
                _inputTitleText = popup._inputTitleText;
                popup._inputField.onValueChanged.AddListener(SetSearchText);
                Object.DestroyImmediate(popup);
            }
            _background = _root.transform.Find("Representation/secondLayerBackfill-CONDITIONAL").gameObject;
            _expanderViewport = _root.transform.Find("Representation/ExpanderViewport").gameObject;
            _layout = _expanderViewport.transform.Find("popupContainerLayout").gameObject;
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
            _filterPanelGroup = CreateFilterGroup("Actions", false);
            _filterPanelGroup.transform.SetParent(_layout.transform);
            var hlg = _filterPanelGroup.GetComponent<HorizontalLayoutGroup>();
            // fix for issues with buttons
            hlg.spacing = 20;
            hlg.padding.bottom = 10;

            AddButton(_buttonCancelTemplate, _filterPanelGroup, "Cancel", Hide);
            AddButton(_buttonTemplate, _filterPanelGroup, "Load", AssemblyFilterAndLoadCallback);
        }

        {
            var settings = TagManagerFeature.Shared.Settings;
            if (settings.SkirmishOptionsPresets != null)
            {
                var presetsGroupGo = CreateFilterGroup("'Quick' Load");
                presetsGroupGo.transform.SetParent(_layout.transform);
                presetsGroupGo.transform.SetSiblingIndex(1);
                var hlg = presetsGroupGo.GetComponent<HorizontalLayoutGroup>();
                // fix for issues with buttons
                hlg.spacing = 20;
                hlg.padding.bottom = 10;
                foreach (var preset in settings.SkirmishOptionsPresets)
                {
                    var queries = new FilterQueries(preset);
                    AddButton(
                        _buttonTemplate,
                        presetsGroupGo,
                        $"{preset.Label} ({queries.MechCount})",
                        () => CheckFilteredCountAndLoadCallback(preset)
                    );
                }
            }

            if (settings.SkirmishOptionsComponentGroup != null)
            {
                AddOptionsGroup(settings.SkirmishOptionsComponentGroup);
            }

            if (settings.SkirmishOptionsMechGroups != null)
            {
                foreach (var group in settings.SkirmishOptionsMechGroups)
                {
                    AddOptionsGroup(group);
                }
            }
        }

        _container.transform.SetAsLastSibling();

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
        var warningSuffix = count > TagManagerFeature.Shared.Settings.SkirmishOverloadWarningCount
            ? "<color=#F06248FF>Warning too many units!</color>"
            : "";
        _inputTitleText.SetText($"{warningSuffix}\r\n{count} results");
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
        var max = TagManagerFeature.Shared.Settings.SkirmishOverloadWarningCount;
        if (count > max)
        {
            GenericPopupBuilder
                .Create(GenericPopupType.Warning, $"You are about to load more than {max} 'Mechs ({count}), this will reduce the performance until the game is restarted.")
                .AddFader()
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

    private static string[]? CombineOrDefault(IEnumerable<string[]?> all)
    {
        HashSet<string>? list = null;
        foreach (var each in all)
        {
            if (each == null)
            {
                continue;
            }
            list ??= new();
            foreach (var term in each)
            {
                list.Add(term);
            }
        }
        return list?.ToArray();
    }

    private TagManagerSettings.TagsFilterSet CreateFilterSet()
    {
        var settings = TagManagerFeature.Shared.Settings;
        var defaults = settings.SkirmishOptionsDefault;

        TagManagerSettings.TagsFilter componentsFilter;
        if (settings.SkirmishOptionsComponentGroup != null)
        {
            componentsFilter = new()
            {
                ContainsAny = CombineOrDefault(
                    settings.SkirmishOptionsComponentGroup.Options
                        .Where(x => x.OptionActive)
                        .Select(x => x.ContainsAny)
                ),
                NotContainsAny = CombineOrDefault(
                    settings.SkirmishOptionsComponentGroup.Options
                        .Where(x => x.OptionActive)
                        .Select(x => x.NotContainsAny)
                ),
            };

            if (componentsFilter.ContainsAny != null && componentsFilter.NotContainsAny != null)
            {
                componentsFilter.NotContainsAny = componentsFilter.NotContainsAny.Except(componentsFilter.ContainsAny).ToArray();
            }

            if (componentsFilter.ContainsAny == null)
            {
                componentsFilter.ContainsAny = defaults.Components.ContainsAny;
            }
            else if (defaults.Components.ContainsAny != null)
            {
                componentsFilter.ContainsAny =
                    componentsFilter.ContainsAny
                        .Concat(defaults.Components.ContainsAny)
                        .ToArray();
            }

            if (componentsFilter.NotContainsAny == null)
            {
                componentsFilter.NotContainsAny = defaults.Components.NotContainsAny;
            }
            else if (defaults.Components.NotContainsAny != null)
            {
                componentsFilter.NotContainsAny =
                    componentsFilter.NotContainsAny
                        .Concat(defaults.Components.NotContainsAny)
                        .ToArray();
            }
        }
        else
        {
            componentsFilter = defaults.Components;
        }

        var filterSet = new TagManagerSettings.TagsFilterSet
        {
            Components = componentsFilter,
            Mechs = new()
            {
                ContainsAny = defaults.Mechs.ContainsAny,
                NotContainsAny = defaults.Mechs.NotContainsAny,
                OptionsSearch = string.IsNullOrEmpty(_searchText) ? null : _searchText,
                OptionsGroups = settings.SkirmishOptionsMechGroups
            },
            Pilots = defaults.Pilots,
            Lances = defaults.Lances
        };
        return filterSet;
    }

    private readonly GameObject _root;
    private readonly GameObject _background;
    private readonly GameObject _expanderViewport;
    private readonly GameObject _layout;
    private readonly LocalizableText _inputTitleText;
    private readonly GameObject _container;
    private readonly GameObject _buttonCancelTemplate;
    private readonly GameObject _buttonTemplate;
    private readonly GameObject _toggleTemplate;
    private readonly GameObject _filterPanelGroup;

    internal void Show()
    {
        _root.transform.SetParent(UIManager.Instance.uiNode.nodeTransform);
        _root.gameObject.SetActive(true);
    }

    private void Hide()
    {
        _root.gameObject.SetActive(false);
        _root.transform.SetParent(UnityGameInstance.BattleTechGame.DataManager.GameObjectPool.inactivePooledGameObjectRoot);
    }

    private void AddButton(GameObject template, GameObject parent, string label, UnityAction action)
    {
        var buttonGo = Object.Instantiate(template, parent.transform);
        var hbsButton = buttonGo.GetComponent<HBSButton>();
        hbsButton.SetText(label);
        hbsButton.OnClicked.AddListener(action);
        hbsButton.SetState(ButtonState.Enabled);
        buttonGo.SetActive(true);
    }

    private void AddOptionsGroup(TagManagerSettings.TagOptionsGroup group)
    {
        Transform rowTransform;
        {
            var rowGo = CreateFilterGroup(group.Label);
            rowTransform = rowGo.transform;
            rowTransform.SetParent(_container.transform);
        }

        foreach (var option in group.Options)
        {
            if (option.OptionBreakLineBefore)
            {
                var rowGo = CreateFilterGroup("");
                rowTransform = rowGo.transform;
                rowTransform.SetParent(_container.transform);
            }
            AddOption(option, rowTransform);
        }
    }

    private void AddOption(TagManagerSettings.TagOption option, Transform parent)
    {
        var cellGo = Object.Instantiate(_toggleTemplate, parent);
        cellGo.name = "Filter Toggle " + option.Label;
        var toggle = cellGo.GetComponent<SGDSToggle>();
        toggle.header.SetText(option.Label);
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

    private GameObject CreateFilterGroup(string label, bool showLabel = true)
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

        if (showLabel)
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

        group.SetActive(true);
        return group;
    }
}