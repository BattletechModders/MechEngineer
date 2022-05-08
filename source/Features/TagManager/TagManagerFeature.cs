using BattleTech;
using HBS.Collections;

namespace MechEngineer.Features.TagManager;

internal class TagManagerFeature : Feature<TagManagerSettings>
{
    internal static readonly TagManagerFeature Shared = new();

    internal override TagManagerSettings Settings => Control.Settings.TagManager;

    protected override void SetupFeatureLoaded()
    {
        Settings.Setup();
    }

    internal void ManageComponentTags(MechComponentDef def)
    {
        var tags = def.ComponentTags;

        if (Settings.LostechStockWeaponVariantFix
            && def is WeaponDef
            && !def.Description.Id.EndsWith("-STOCK")
            && tags.Contains(MechValidationRules.ComponentTag_LosTech))
        {
            Control.Logger.Debug?.Log($"LostechStockWeaponVariantFix {def.Description.Id}");

            tags.Remove(MechValidationRules.ComponentTag_Stock);
            tags.Add(MechValidationRules.ComponentTag_Variant);
        }

        if (Check(tags, Settings.WhitelistComponentTagSet))
        {
            Control.Logger.Debug?.Log($"WhitelistComponentTags {def.Description.Id}");
            tags.Remove(MechValidationRules.Tag_Blacklisted);
        }

        if (Check(tags, Settings.BlacklistComponentTagSet))
        {
            Control.Logger.Debug?.Log($"BlacklistComponentTags {def.Description.Id}");
            tags.Add(MechValidationRules.Tag_Blacklisted);
        }
    }

    internal void ManageMechTags(MechDef def)
    {
        var tags = def.MechTags;

        if (Check(tags, Settings.WhitelistMechTagSet))
        {
            Control.Logger.Debug?.Log($"WhitelistMechTags {def.Description.Id}");
            tags.Remove(MechValidationRules.Tag_Blacklisted);
        }

        if (Check(tags, Settings.BlacklistMechTagSet))
        {
            Control.Logger.Debug?.Log($"BlacklistMechTags {def.Description.Id}");
            tags.Add(MechValidationRules.Tag_Blacklisted);
        }
    }

    private bool Check(TagSet a, TagSet b)
    {
        if (a.Count < 1 || b.Count < 1)
        {
            return false;
        }
        return a.ContainsAny(b);
    }
}