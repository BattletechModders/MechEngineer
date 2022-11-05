using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;
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

    internal void ShowOptions(MainMenu menu)
    {
        var builder = GenericPopupBuilder.Create(Settings.SkirmishOptionsTitle, "")
            .AddFader();

        void AddOption(TagManagerSettings.TagsFilterSet option)
        {
            if (!option.Hide)
            {
                builder = builder.AddButton(option.Label, () =>
                {
                    _currentSkirmishFilter = option;
                    OpenSkirmishMechBay(menu);
                });
            }
        }

        AddOption(Settings.SkirmishDefault);
        foreach (var option in Settings.SkirmishOptions)
        {
            AddOption(option);
        }

        builder.Render();
    }

    private void OpenSkirmishMechBay(MainMenu menu)
    {
        LazySingletonBehavior<UIManager>.Instance.GetOrCreateUIModule<SkirmishMechBayPanel>().SetData();
        menu.Pool();
    }

    internal void RequestResources(SkirmishMechBayPanel panel)
    {
        var loadRequest = panel.dataManager.CreateLoadRequest(panel.LanceConfiguratorDataLoaded);

        var mdd = MetadataDatabase.Instance;

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
        foreach (var id in QueryItems("PilotDefID", "PilotDef", _currentSkirmishFilter.Pilots))
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
        foreach (var id in QueryItems("UnitDefID", "UnitDef", _currentSkirmishFilter.Mechs))
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
        foreach (var id in QueryItems("LanceDefID", "LanceDef", _currentSkirmishFilter.Lances))
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

    private static List<string> QueryItems(string idColumn, string tableName, TagManagerSettings.TagsFilter filter)
    {
        var queryString =
            $"SELECT {idColumn} FROM {tableName} d LEFT JOIN TagSetTag tst ON d.TagSetID = tst.TagSetID" +
            " WHERE tst.TagName NOT IN @Exclude";
        if (!filter.AllowByDefault)
        {
            queryString += " AND tst.TagName IN @Include";
        }

        return MetadataDatabase.Instance
            .Query<string>(queryString, new { Include = filter.Allow, Exclude = filter.Block })
            .ToList();
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
        return IsValidForSkirmish(def.LanceTags, _currentSkirmishFilter.Lances);
    }

    private bool IsValidForSkirmish(TagSet tags, TagManagerSettings.TagsFilter filter)
    {
        if (ContainsAny(tags, filter.Block))
        {
            return false;
        }
        if (ContainsAny(tags, filter.Allow))
        {
            return true;
        }
        return filter.AllowByDefault;
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
        if (tagSet.Count < 1 || tags.Length < 1)
        {
            return false;
        }
        tagSet.UpdateSet();
        return tags.Any(tag => tagSet.set.Contains(tag));
    }
}