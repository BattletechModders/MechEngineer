using System;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using HBS;
using HBS.Collections;

namespace MechEngineer.Features.TagManager;

internal class TagManagerFeature : Feature<TagManagerSettings>
{
    internal static readonly TagManagerFeature Shared = new();

    internal override TagManagerSettings Settings => Control.Settings.TagManager;

    private TagManagerSettings.TagsFilterSet _currentSkirmishFilter = null!;
    protected override void SetupFeatureLoaded()
    {
        _currentSkirmishFilter = Settings.SkirmishDefault;
    }

    private OptionsPanel? _optionsPanel;
    private MainMenu? _mainMenu;

    internal void ShowOptions(MainMenu menu)
    {
        _mainMenu = menu;
        _optionsPanel ??= new(filter =>
        {
            _currentSkirmishFilter = filter;
            OpenSkirmishMechBay();
        });
        _optionsPanel.Show();
    }

    private void OpenSkirmishMechBay()
    {
        LazySingletonBehavior<UIManager>.Instance.GetOrCreateUIModule<SkirmishMechBayPanel>().SetData();
        _mainMenu!.Pool();
    }

    internal void RequestResources(SkirmishMechBayPanel panel)
    {
        var loadRequest = panel.dataManager.CreateLoadRequest(panel.LanceConfiguratorDataLoaded);
        var queries = new FilterQueries(_currentSkirmishFilter);

        // vanilla
        // loadRequest.AddAllOfTypeBlindLoadRequest(BattleTechResourceType.ChassisDef, true);
        loadRequest.AddAllOfTypeBlindLoadRequest(BattleTechResourceType.BaseDescriptionDef, true);

        // always load all components, required by CustomComponents
        loadRequest.AddAllOfTypeBlindLoadRequest(BattleTechResourceType.AmmunitionBoxDef, true);
        loadRequest.AddAllOfTypeBlindLoadRequest(BattleTechResourceType.HeatSinkDef, true);
        loadRequest.AddAllOfTypeBlindLoadRequest(BattleTechResourceType.JumpJetDef, true);
        loadRequest.AddAllOfTypeBlindLoadRequest(BattleTechResourceType.UpgradeDef, true);
        loadRequest.AddAllOfTypeBlindLoadRequest(BattleTechResourceType.WeaponDef, true);

        panel.allPilots = new();
        panel.allPilotDefs = new();
        foreach (var id in queries.PilotIds())
        {
            loadRequest.AddLoadRequest(BattleTechResourceType.PilotDef, id, delegate(string _, PilotDef? def)
            {
                if (MechValidationRules.PilotIsValidForSkirmish(def))
                {
                    panel.allPilotDefs.Add(def);
                    var pilot = new Pilot(def, $"Pilot_{id}", true);
                    panel.allPilots.Add(pilot);
                }
                else
                {
                    Control.Logger.Warning?.Log($"Invalid pilot {id} for skirmish was loaded");
                }
            }, true);
        }

        panel.stockMechs = new();
        foreach (var id in queries.MechIds())
        {
            loadRequest.AddLoadRequest(BattleTechResourceType.MechDef, id, delegate(string _, MechDef? def)
            {
                try
                {
                    def?.Refresh();
                }
                catch (Exception e)
                {
                    Control.Logger.Warning?.Log($"Mech {id} could not be refreshed", e);
                    return;
                }

                if (MechValidationRules.MechIsValidForSkirmish(def, false))
                {
                    panel.stockMechs.Add(def);
                }
                else
                {
                    Control.Logger.Warning?.Log($"Invalid mech {id} for skirmish was loaded");
                }
            }, true);
        }

        panel.stockLances = new();
        foreach (var id in queries.LanceIds())
        {
            loadRequest.AddLoadRequest(BattleTechResourceType.LanceDef, id, delegate(string _, LanceDef? def)
            {
                if (MechValidationRules.LanceIsValidForSkirmish(def, false, false))
                {
                    panel.stockLances.Add(def);
                }
                else
                {
                    Control.Logger.Warning?.Log($"Invalid lance {id} for skirmish was loaded");
                }
            }, true);
        }

        loadRequest.ProcessRequests();
    }

    internal void ManageComponentTags(MechComponentDef def)
    {
        var tags = def.ComponentTags;

        if (Settings.LostechStockWeaponVariantFix
            && def is WeaponDef
            && !def.Description.Id.EndsWith("-STOCK")
            && tags.Contains(MechValidationRules.ComponentTag_LosTech))
        {
            Control.Logger.Trace?.Log($"LostechStockWeaponVariantFix {def.Description.Id}");

            tags.Remove(MechValidationRules.ComponentTag_Stock);
            tags.Add(MechValidationRules.ComponentTag_Variant);
        }

        ApplyFilter(tags, Settings.Components);
    }

    internal bool ComponentIsValidForSkirmish(MechComponentDef def, bool isDebugLab)
    {
        return IsValidForSkirmish(def.ComponentTags, _currentSkirmishFilter.Components);
    }

    internal void ManageMechTags(MechDef def)
    {
        ApplyFilter(def.MechTags, Settings.Mechs);
    }

    internal bool MechIsValidForSkirmish(MechDef def, bool includeCustomMechs)
    {
        if (!includeCustomMechs && def.MechTags.Contains(MechValidationRules.MechTag_Custom))
        {
            return false;
        }
        return IsValidForSkirmish(def.MechTags, _currentSkirmishFilter.Mechs);
    }

    public void ManagePilotTags(PilotDef def)
    {
        ApplyFilter(def.PilotTags, Settings.Pilots);
    }

    internal bool PilotIsValidForSkirmish(PilotDef def)
    {
        return IsValidForSkirmish(def.PilotTags, _currentSkirmishFilter.Pilots);
    }

    internal void ManageLanceTags(LanceDef def)
    {
        ApplyFilter(def.LanceTags, Settings.Lances);
    }

    internal bool LanceIsValidForSkirmish(LanceDef def, bool requireFullLance, bool includeCustomLances)
    {
        if (!includeCustomLances && def.LanceTags.Contains(MechValidationRules.LanceTag_Custom))
        {
            return false;
        }
        return IsValidForSkirmish(def.LanceTags, _currentSkirmishFilter.Lances);
    }

    private bool IsValidForSkirmish(TagSet tags, TagManagerSettings.TagsFilter filter)
    {
        if (filter.NotContainsAny != null && ContainsAny(tags, filter.NotContainsAny))
        {
            return false;
        }
        if (filter.ContainsAny != null && !ContainsAny(tags, filter.ContainsAny))
        {
            return false;
        }
        return true;
    }

    private void ApplyFilter(TagSet tags, TagManagerSettings.TagsTransformer transformer)
    {
        if (ContainsAny(tags, transformer.Whitelist))
        {
            tags.Remove(MechValidationRules.Tag_Blacklisted);
        }
        if (ContainsAny(tags, transformer.Blacklist))
        {
            tags.Add(MechValidationRules.Tag_Blacklisted);
        }
    }

    private bool ContainsAny(TagSet tagSet, string[] tags)
    {
        if (tags.Length < 1 || tagSet.Count < 1)
        {
            return false;
        }
        tagSet.UpdateSet();
        return tags.Any(tag => tagSet.set.Contains(tag));
    }
}