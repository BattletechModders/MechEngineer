using System;
using BattleTech;
using fastJSON;

namespace MechEngineer.Features.TagManager;

public class TagManagerSettings : ISettings
{
    public string EnabledDescription =>
        $"Manipulates tags on Components, Mechs, Pilots and Lances by adding or removing {MechValidationRules.Tag_Blacklisted}:" +
        $" {nameof(TagsTransformer.Blacklist)} is applied after {nameof(TagsTransformer.Whitelist)}." +
        $" Also allows filtering by tags when entering the skirmish mech lab:" +
        $" {nameof(TagsFilter.NotContainsAny)} has precedence over {nameof(TagsFilter.ContainsAny)}." +
        $" Empty allow can be used to match nothing, while null means to allow everything.";
    public bool Enabled { get; set; }

    public const string SimGameItemsMinCountDescription = $"Set the owned minimum count of each mech component in SimGame.";
    public int SimGameItemsMinCount = 0;

    public const string LostechStockWeaponVariantDescription = "Fixes lostech variant weapon tagging by checking if id ends with -STOCK.";
    public bool LostechStockWeaponVariantFix = true;

    public const string SkirmishOverloadWarningDescription = "Warn the user before loading into the SkirmishMechBay if too many 'Mech will be loaded.";
    public int SkirmishOverloadWarningCount = 500;

    public const string SkirmishDefaultDescription = "The default settings used when no options panel is shown and the user enters the skirmish 'Mech bay directly.";
    public TagsFilterSet SkirmishDefault = new()
    {
        Label = "Default",
        Components = new()
        {
            ContainsAny = new[] { MechValidationRules.ComponentTag_Stock, MechValidationRules.ComponentTag_Variant, MechValidationRules.ComponentTag_LosTech },
            NotContainsAny = new[] { MechValidationRules.Tag_Blacklisted }
        },
        Mechs = new()
        {
            ContainsAny = new[] { MechValidationRules.MechTag_Released },
            NotContainsAny = new[] { MechValidationRules.Tag_Blacklisted, MechValidationRules.MechTag_Unlocked }
        },
        Pilots = new()
        {
            ContainsAny = new[] { MechValidationRules.PilotTag_Released }
        },
        Lances = new()
        {
            ContainsAny = new[] { MechValidationRules.LanceTag_Skirmish }
        }
    };

    public const string SkirmishOptionsShowDescription = "Shows or hides the skirmish options panel before entering the skirmish 'Mech bay.";
    public bool SkirmishOptionsShow = false;

    public const string SkirmishPresetsDescription = "Presets allow to quickly select a custom filter-combination.";
    public TagsFilterSet[]? SkirmishOptionsPresets =
    {
        new()
        {
            Label = "Stock",
            Components = new()
            {
                ContainsAny = new[] { MechValidationRules.ComponentTag_Stock },
                NotContainsAny = new[] { MechValidationRules.Tag_Blacklisted }
            },
            Mechs = new()
            {
                ContainsAny = new[] { MechValidationRules.MechTag_Released },
                NotContainsAny = new[] { MechValidationRules.Tag_Blacklisted, MechValidationRules.MechTag_Unlocked }
            },
            Pilots = new()
            {
                ContainsAny = new[] { MechValidationRules.PilotTag_Released }
            },
            Lances = new()
            {
                ContainsAny = new[] { MechValidationRules.LanceTag_Skirmish }
            }
        },
        new()
        {
            Label = "DEBUG Screenshots",
            Components = new()
            {
                ContainsAny = new string[] { },
                NotContainsAny = new[] { MechValidationRules.Tag_Blacklisted }
            },
            Mechs = new()
            {
                ContainsAny = new[] { MechValidationRules.MechTag_Released },
                NotContainsAny = new[] { MechValidationRules.Tag_Blacklisted, MechValidationRules.MechTag_Unlocked }
            },
            Pilots = new()
            {
                ContainsAny = new string[] { }
            },
            Lances = new()
            {
                ContainsAny = new string[] { }
            }
        }
    };

    public const string SkirmishOptionsDefaultDescription = "Filters that are always active regardless of what the user selects in the options panel.";
    public TagsFilterSet SkirmishOptionsDefault = new()
    {
        Components = new()
        {
            ContainsAny = new string[] { },
            NotContainsAny = new[] { MechValidationRules.Tag_Blacklisted }
        },
        Mechs = new()
        {
            ContainsAny = new[] { MechValidationRules.MechTag_Released },
            NotContainsAny = new[] { MechValidationRules.Tag_Blacklisted, MechValidationRules.MechTag_Unlocked }
        },
        Pilots = new()
        {
            ContainsAny = new[] { MechValidationRules.PilotTag_Released }
        },
        Lances = new()
        {
            ContainsAny = new[] { MechValidationRules.LanceTag_Skirmish }
        }
    };

    public const string SkirmishOptionsComponentDescription =
        "Component options that can be selected in the options panel, they don't influence what is loaded into the 'Mech bay only what is shown later on in the 'Mech lab inventory." +
        " (Alpha) All active ContainsAny are combined with an OR, all active NotContainsAny are combined with an OR." +
        " Afterwards NotContainsAny has anything removed that exists in ContainsAny." +
        " Then anything from the defaults are added to each." +
        " Then the ContainsAny and NotContainsAny are combined with an AND.";
    public TagOptionsGroup? SkirmishOptionsComponentGroup = new()
    {
        Label = "Components",
        Options = new TagOption[]
        {
            new()
            {
                Label = "Stock",
                ContainsAny = new[] { MechValidationRules.ComponentTag_Stock },
                NotContainsAny = new[] { MechValidationRules.ComponentTag_Variant, MechValidationRules.ComponentTag_LosTech },
                OptionActive = true
            },
            new()
            {
                Label = "Variants & LosTech",
                ContainsAny = new[] { MechValidationRules.ComponentTag_Variant, MechValidationRules.ComponentTag_LosTech },
                NotContainsAny = new[] { MechValidationRules.ComponentTag_Stock },
                OptionActive = true
            }
        }
    };

    public const string SkirmishOptionsMechGroupsDescription =
        "Filter which 'Mech get loaded. Each group is combined with an AND, each toggle within a group is combined with an OR.";
    public TagOptionsGroup[]? SkirmishOptionsMechGroups =
    {
        new()
        {
            Label = "Tonnage",
            Options = new TagOption[]
            {
                new()
                {
                    Label = "Light",
                    ContainsAny = new[] { "unit_light" },
                    OptionActive = true
                },
                new()
                {
                    Label = "Medium",
                    ContainsAny = new[] { "unit_medium" },
                    OptionActive = true
                },
                new()
                {
                    Label = "Heavy",
                    ContainsAny = new[] { "unit_heavy" },
                    OptionActive = true,
                    OptionBreakLineBefore = true
                },
                new()
                {
                    Label = "Assault",
                    ContainsAny = new[] { "unit_assault" },
                    OptionActive = true
                }
            }
        },
        new()
        {
            Label = "Source",
            Options = new TagOption[]
            {
                new()
                {
                    Label = "Base",
                    NotContainsAny = new[] { "unit_dlc" },
                    OptionActive = true
                },
                new()
                {
                    Label = "DLC",
                    ContainsAny = new[] { "unit_dlc" },
                    OptionActive = true
                }
            }
        }
/* TODO add description explaining custom tags to search for
unit_common
unit_speed_high
unit_speed_low
unit_armor_high
unit_armor_low
unit_range_long
unit_range_medium
unit_range_short
unit_indirectFire
unit_jumpOK
unit_hot
unit_role_brawler
unit_role_flanker
unit_role_scout
unit_role_sniper
unit_role_activeprobe
unit_role_ecmcarrier
unit_role_ewe
unit_lance_assassin
unit_lance_support
unit_lance_tank
unit_lance_vanguard
*/
    };

    public class TagOptionsGroup
    {
        public string Label = "<null>";
        public TagOption[] Options = Array.Empty<TagOption>();
    }

    public class TagOption
    {
        public string Label = "<null>";
        public string[]? ContainsAny;
        public string[]? NotContainsAny;
        public bool OptionActive = false;
        public bool OptionBreakLineBefore = false;
    }

    public class TagsFilterSet
    {
        public string Label = "<null>";
        public TagsFilter Components = new();
        public TagsFilter Mechs = new();
        public TagsFilter Pilots = new();
        public TagsFilter Lances = new();
    }
    public class TagsFilter
    {
        public string[]? ContainsAny;
        public string[]? NotContainsAny;
        [JsonIgnore]
        internal string? OptionsSearch;
        [JsonIgnore]
        internal TagOptionsGroup[]? OptionsGroups;
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