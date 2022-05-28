using BattleTech;
using fastJSON;
using HBS.Collections;

namespace MechEngineer.Features.TagManager;

public class TagManagerSettings : ISettings
{
    public bool Enabled { get; set; }
    public string EnabledDescription => "Manipulates MechDef and ComponentDef Tags";

    public int? SimGameItemsMinCount;
    public string SimGameItemsMinCountDescription = $"Set the owned minimum count of each mech component in SimGame.";

    public string[] SkirmishWhitelistTags = {MechValidationRules.ComponentTag_Stock};
    public string SkirmishWhitelistTagsDescription = "Components with these tags will appear in the skirmish mechlab, blacklisted tagged items never appear.";

    public string[] WhitelistComponentTags = {MechValidationRules.ComponentTag_Stock, MechValidationRules.ComponentTag_Variant, MechValidationRules.ComponentTag_LosTech};
    public string WhitelistComponentTagsDescription = "Whitelists components with specified tags.";

    public string[] BlacklistComponentTags = { };
    public string BlacklistComponentTagsDescription = "Blacklists components with specified tags, has precedence over whitelisting.";

    public string[] WhitelistMechTags = {MechValidationRules.MechTag_Released};
    public string WhitelistMechTagsDescription = "Similar to WhitelistComponentTags but with MechTags";

    public string[] BlacklistMechTags = { };
    public string BlacklistMechTagsDescription = "Similar to BlacklistComponentTags but with MechTags";

    public bool LostechStockWeaponVariantFix { get; set; } = true;
    public string LostechStockWeaponVariantDescription => "Fixes lostech variant weapon tagging by checking if id ends with -STOCK.";

    internal void Setup()
    {
        SkirmishWhitelistTagSet = new(SkirmishWhitelistTags);
        WhitelistComponentTagSet = new(WhitelistComponentTags);
        BlacklistComponentTagSet = new(BlacklistComponentTags);
        WhitelistMechTagSet = new(WhitelistMechTags);
        BlacklistMechTagSet = new(BlacklistMechTags);
    }

    [JsonIgnore]
    public TagSet SkirmishWhitelistTagSet = null!;

    [JsonIgnore]
    public TagSet WhitelistComponentTagSet = null!;

    [JsonIgnore]
    public TagSet BlacklistComponentTagSet = null!;

    [JsonIgnore]
    public TagSet WhitelistMechTagSet = null!;

    [JsonIgnore]
    public TagSet BlacklistMechTagSet = null!;
}