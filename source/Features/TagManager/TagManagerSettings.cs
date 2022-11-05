using BattleTech;
using fastJSON;
using HBS.Collections;

namespace MechEngineer.Features.TagManager;

public class TagManagerSettings : ISettings
{
    public bool Enabled { get; set; }

    public string EnabledDescription =>
        $"Manipulates tags on Components, Mechs, Pilots and Lances by adding or removing {MechValidationRules.Tag_Blacklisted}:" +
        $" {nameof(TagsTransformer.Blacklist)} is applied after {nameof(TagsTransformer.Whitelist)}." +
        $" Also allows filtering by tags when entering the skirmish mech lab: {nameof(TagsFilter.Block)} has precedence over {nameof(TagsFilter.Allow)}";

    public int SimGameItemsMinCount;
    public string SimGameItemsMinCountDescription = $"Set the owned minimum count of each mech component in SimGame.";

    public bool LostechStockWeaponVariantFix { get; set; } = true;
    public string LostechStockWeaponVariantDescription => "Fixes lostech variant weapon tagging by checking if id ends with -STOCK.";

    public string SkirmishOptionsTitle { get; set; } = "Selection";

    public TagsFilterSet SkirmishDefault = new()
    {
        Label = "Default",
        Components = new()
        {
            Allow = new[] { MechValidationRules.ComponentTag_Stock, MechValidationRules.ComponentTag_Variant, MechValidationRules.ComponentTag_LosTech },
            Block = new[] { MechValidationRules.Tag_Blacklisted }
        },
        Mechs = new()
        {
            Allow = new[] { MechValidationRules.MechTag_Released },
            Block = new[] { MechValidationRules.Tag_Blacklisted, MechValidationRules.MechTag_Unlocked, MechValidationRules.MechTag_Custom }
        },
        Pilots = new()
        {
            Allow = new[] { MechValidationRules.PilotTag_Released }
        },
        Lances = new()
        {
            Allow = new[] { MechValidationRules.LanceTag_Skirmish },
            Block = new[] { MechValidationRules.LanceTag_Custom }
        }
    };

    public TagsFilterSet[] SkirmishOptions =
    {
        new()
        {
            Label = "Stock",
            Components = new()
            {
                Allow = new[] { MechValidationRules.ComponentTag_Stock },
                Block = new[] { MechValidationRules.Tag_Blacklisted }
            },
            Mechs = new()
            {
                Allow = new[] { MechValidationRules.MechTag_Released },
                Block = new[] { MechValidationRules.Tag_Blacklisted, MechValidationRules.MechTag_Unlocked, MechValidationRules.MechTag_Custom }
            },
            Pilots = new()
            {
                Allow = new[] { MechValidationRules.PilotTag_Released }
            },
            Lances = new()
            {
                Allow = new[] { MechValidationRules.LanceTag_Skirmish },
                Block = new[] { MechValidationRules.LanceTag_Custom }
            }
        },
        new()
        {
            Label = "RT all",
            Components = new()
            {
                AllowByDefault = true,
                Block = new[] { MechValidationRules.Tag_Blacklisted, "VanillaOverride" }
            },
            Mechs = new()
            {
                AllowByDefault = true,
                Block = new[] { MechValidationRules.Tag_Blacklisted, MechValidationRules.MechTag_Custom, "unit_vehicle" }
            },
            Pilots = new(),
            Lances = new()
        },
        new()
        {
            Label = "RT SLDF",
            Components = new()
            {
                AllowByDefault = true,
                Block = new[] { MechValidationRules.Tag_Blacklisted, "VanillaOverride" }
            },
            Mechs = new()
            {
                Allow = new[] { "unit_sldf" },
                Block = new[] { MechValidationRules.Tag_Blacklisted, MechValidationRules.MechTag_Custom, "unit_vehicle" }
            },
            Pilots = new(),
            Lances = new()
        }
    };

    public class TagsFilterSet
    {
        public string Label = "<null>";
        public bool Hide;
        public TagsFilter Components = new();
        public TagsFilter Mechs = new();
        public TagsFilter Pilots = new();
        public TagsFilter Lances = new();
    }
    public class TagsFilter
    {
        public string[] Allow = { };
        public string[] Block = { };
        public bool AllowByDefault = false;
    }

    public TagsTransformer Components = new()
    {
        Whitelist = new[] { MechValidationRules.ComponentTag_Stock, MechValidationRules.ComponentTag_Variant, MechValidationRules.ComponentTag_LosTech },
        Blacklist = new[] { MechValidationRules.ComponentTag_Debug }
    };
    public TagsTransformer Mechs = new()
    {
        Whitelist = new[] { MechValidationRules.MechTag_Released }
    };
    public TagsTransformer Pilots = new();
    public TagsTransformer Lances = new()
    {
        Whitelist = new[] { MechValidationRules.LanceTag_Released }
    };
    public class TagsTransformer
    {
        public string[] Whitelist = { };
        public string[] Blacklist = { };
    }
}