using BattleTech;
using HBS.Collections;

namespace MechEngineer.Features.TagManager;

internal class TagManagerFeature : Feature<TagManagerSettings>
{
    internal static readonly TagManagerFeature Shared = new();

    internal override TagManagerSettings Settings => Control.Settings.TagManager;

    protected override void SetupFeatureLoaded()
    {
        Settings.Complete();
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
        return IsValidForSkirmish(def.ComponentTags, Settings.Components);
    }

    internal void ManageMechTags(MechDef def)
    {
        ApplyFilter(def.MechTags, Settings.Mechs);
    }

    internal bool MechIsValidForSkirmish(MechDef def, bool includeCustomMechs)
    {
        return IsValidForSkirmish(def.MechTags, Settings.Mechs);
    }

    public void ManagePilotTags(PilotDef def)
    {
        ApplyFilter(def.PilotTags, Settings.Pilots);
    }

    internal bool PilotIsValidForSkirmish(PilotDef def)
    {
        return IsValidForSkirmish(def.PilotTags, Settings.Pilots);
    }

    internal void ManageLanceTags(LanceDef def)
    {
        ApplyFilter(def.LanceTags, Settings.Lances);
    }

    internal bool LanceIsValidForSkirmish(LanceDef def, bool requireFullLance, bool includeCustomLances)
    {
        return IsValidForSkirmish(def.LanceTags, Settings.Lances);
    }

    private bool IsValidForSkirmish(TagSet tags, TagManagerSettings.TagsFilter filter)
    {
        if (filter.SkirmishForce.HasValue)
        {
            return filter.SkirmishForce.Value;
        }

        if (ContainsAny(tags, filter.SkirmishBlockTagSet))
        {
            return false;
        }
        if (ContainsAny(tags, filter.SkirmishAllowTagSet))
        {
            return true;
        }
        return false;
    }

    private void ApplyFilter(TagSet tags, TagManagerSettings.TagsFilter filter)
    {
        if (ContainsAny(tags, filter.WhitelistTagSet))
        {
            tags.Remove(MechValidationRules.Tag_Blacklisted);
        }
        if (ContainsAny(tags, filter.BlacklistTagSet))
        {
            tags.Add(MechValidationRules.Tag_Blacklisted);
        }
    }

    private bool ContainsAny(TagSet a, TagSet b)
    {
        if (a.Count < 1 || b.Count < 1)
        {
            return false;
        }
        return a.ContainsAny(b);
    }
}