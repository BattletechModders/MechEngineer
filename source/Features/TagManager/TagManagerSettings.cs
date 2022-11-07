using System;
using BattleTech;
using fastJSON;

namespace MechEngineer.Features.TagManager;

public class TagManagerSettings : ISettings
{
    public bool Enabled { get; set; }

    public string EnabledDescription =>
        $"Manipulates tags on Components, Mechs, Pilots and Lances by adding or removing {MechValidationRules.Tag_Blacklisted}:" +
        $" {nameof(TagsTransformer.Blacklist)} is applied after {nameof(TagsTransformer.Whitelist)}." +
        $" Also allows filtering by tags when entering the skirmish mech lab: {nameof(TagsFilter.BlockAny)} has precedence over {nameof(TagsFilter.AllowAny)}. Empty allow can be used to match nothing, while null means to allow everything.";

    public int SimGameItemsMinCount = 0;
    public const string SimGameItemsMinCountDescription = $"Set the owned minimum count of each mech component in SimGame.";

    public bool LostechStockWeaponVariantFix = true;
    public const string LostechStockWeaponVariantDescription = "Fixes lostech variant weapon tagging by checking if id ends with -STOCK.";

    public int SkirmishOverloadWarningCount = 500;
    public const string SkirmishOverloadWarningDescription = "Warn the user before loading into the SkirmishMechBay if too many 'Mech will be loaded.";

    public TagsFilterSet SkirmishDefault = new()
    {
        Label = "Default",
        Components = new()
        {
            AllowAny = new[] { MechValidationRules.ComponentTag_Stock, MechValidationRules.ComponentTag_Variant, MechValidationRules.ComponentTag_LosTech },
            BlockAny = new[] { MechValidationRules.Tag_Blacklisted }
        },
        Mechs = new()
        {
            AllowAny = new[] { MechValidationRules.MechTag_Released },
            BlockAny = new[] { MechValidationRules.Tag_Blacklisted, MechValidationRules.MechTag_Unlocked }
        },
        Pilots = new()
        {
            AllowAny = new[] { MechValidationRules.PilotTag_Released }
        },
        Lances = new()
        {
            AllowAny = new[] { MechValidationRules.LanceTag_Skirmish }
        }
    };
    public const string SkirmishDefaultDescription = "The default settings used when no options panel is shown. Can be shown as a preset in the options panel.";

    public TagsFilterSet[] SkirmishPresets =
    {
        new()
        {
            Label = "Stock",
            Components = new()
            {
                AllowAny = new[] { MechValidationRules.ComponentTag_Stock },
                BlockAny = new[] { MechValidationRules.Tag_Blacklisted }
            },
            Mechs = new()
            {
                AllowAny = new[] { MechValidationRules.MechTag_Released },
                BlockAny = new[] { MechValidationRules.Tag_Blacklisted, MechValidationRules.MechTag_Unlocked }
            },
            Pilots = new()
            {
                AllowAny = new[] { MechValidationRules.PilotTag_Released }
            },
            Lances = new()
            {
                AllowAny = new[] { MechValidationRules.LanceTag_Skirmish }
            }
        },
    };
    public const string SkirmishPresetsDescription = "If empty, no options panel is shown. Otherwise these are presets to quickly select a filter-combination.";

    public TagsFilterSet SkirmishOptionsDefault = new()
    {
        Components = new()
        {
            AllowAny = new[] { MechValidationRules.ComponentTag_Stock, MechValidationRules.ComponentTag_Variant, MechValidationRules.ComponentTag_LosTech },
            BlockAny = new[] { MechValidationRules.Tag_Blacklisted }
        },
        Mechs = new()
        {
            AllowAny = null,
            BlockAny = new[] { MechValidationRules.Tag_Blacklisted, MechValidationRules.MechTag_Unlocked, "VanillaOverride", "unit_override" }
        },
        Pilots = new()
        {
            AllowAny = new[] { MechValidationRules.PilotTag_Released }
        },
        Lances = new()
        {
            AllowAny = new[] { MechValidationRules.LanceTag_Skirmish }
        }
    };
    public const string SkirmishOptionsDefaultDescription = "(Alpha) The options panel uses these defaults when dynamically combining filters or using the search filter.";

    public TagOptionsGroup[] SkirmishOptionsGroups =
    {
        new()
        {
            Label = "Tonnage",
            Options = new TagOption[]
            {
                new()
                {
                    Label = "Light",
                    IncludeAny = new[] { "unit_light" }
                },
                new()
                {
                    Label = "Medium",
                    IncludeAny = new[] { "unit_medium" }
                },
                new()
                {
                    Label = "Heavy",
                    IncludeAny = new[] { "unit_heavy" }
                },
                new()
                {
                    Label = "Assault",
                    IncludeAny = new[] { "unit_assault" }
                },
            }
        },
        new()
        {
            Label = "Release",
            Options = new TagOption[]
            {
                new()
                {
                    Label = "Skirmish",
                    IncludeAny = new[] { "unit_release" },
                    OptionActive = true
                },
                new()
                {
                    Label = "Other",
                    ExcludeAny = new[] { "unit_release" }
                },
            }
        },
        new()
        {
            Label = "DLC",
            Options = new TagOption[]
            {
                new()
                {
                    Label = "DLC",
                    IncludeAny = new[] { "unit_dlc" }
                },
                new()
                {
                    Label = "Other",
                    ExcludeAny = new[] { "unit_dlc" }
                },
            }
        },
        new()
        {
            Label = "Common",
            Options = new TagOption[]
            {
                new()
                {
                    Label = "Common",
                    IncludeAny = new[] { "unit_common" }
                },
                new()
                {
                    Label = "Other",
                    ExcludeAny = new[] { "unit_common" }
                },
            }
        },
        new()
        {
            Label = "Speed",
            Options = new TagOption[]
            {
                new()
                {
                    Label = "High",
                    IncludeAny = new[] { "unit_speed_high" }
                },
                new()
                {
                    Label = "Low",
                    IncludeAny = new[] { "unit_speed_low" }
                },
                new()
                {
                    Label = "Other",
                    ExcludeAny = new[]
                    {
                        "unit_speed_high",
                        "unit_speed_low"
                    }
                },
            }
        },
        new()
        {
            Label = "Armor",
            Options = new TagOption[]
            {
                new()
                {
                    Label = "High",
                    IncludeAny = new[] { "unit_armor_high" }
                },
                new()
                {
                    Label = "Low",
                    IncludeAny = new[] { "unit_armor_low" }
                },
                new()
                {
                    Label = "Other",
                    ExcludeAny = new[]
                    {
                        "unit_armor_high",
                        "unit_armor_low"
                    }
                },
            }
        },
        new()
        {
            Label = "Range",
            Options = new TagOption[]
            {
                new()
                {
                    Label = "Long",
                    IncludeAny = new[] { "unit_range_long" }
                },
                new()
                {
                    Label = "Medium",
                    IncludeAny = new[] { "unit_range_medium" }
                },
                new()
                {
                    Label = "Short",
                    IncludeAny = new[] { "unit_range_short" }
                },
                new()
                {
                    Label = "Other",
                    ExcludeAny = new[]
                    {
                        "unit_range_long",
                        "unit_range_medium",
                        "unit_range_short"
                    }
                },
            }
        },
        new()
        {
            Label = "Misc",
            Options = new TagOption[]
            {
                new()
                {
                    Label = "Indirect Fire",
                    IncludeAny = new[] { "unit_indirectFire" }
                },
                new()
                {
                    Label = "Jump",
                    IncludeAny = new[] { "unit_jumpOK" }
                },
                new()
                {
                    Label = "Hot",
                    IncludeAny = new[] { "unit_hot" }
                },
                new()
                {
                    Label = "Other",
                    ExcludeAny = new[]
                    {
                        "unit_indirectFire",
                        "unit_jumpOK",
                        "unit_hot"
                    }
                },
            }
        },
        new()
        {
            Label = "Role",
            Options = new TagOption[]
            {
                new()
                {
                    Label = "Brawler",
                    IncludeAny = new[] { "unit_role_brawler" }
                },
                new()
                {
                    Label = "Flanker",
                    IncludeAny = new[] { "unit_role_flanker" }
                },
                new()
                {
                    Label = "Scout",
                    IncludeAny = new[] { "unit_role_scout" }
                },
                new()
                {
                    Label = "Sniper",
                    IncludeAny = new[] { "unit_role_sniper" }
                },
                new()
                {
                    Label = "AP & ECM",
                    IncludeAny = new[]
                    {
                        "unit_role_activeprobe",
                        "unit_role_ecmcarrier",
                        "unit_role_ewe"
                    }
                },
                new()
                {
                    Label = "Other",
                    ExcludeAny = new[]
                    {
                        "unit_role_brawler",
                        "unit_role_flanker",
                        "unit_role_scout",
                        "unit_role_sniper",
                        "unit_role_activeprobe",
                        "unit_role_ecmcarrier",
                        "unit_role_ewe"
                    }
                }
            }
        },
        new()
        {
            Label = "Lance",
            Options = new TagOption[]
            {
                new()
                {
                    Label = "Assassin",
                    IncludeAny = new[] { "unit_lance_assassin" }
                },
                new()
                {
                    Label = "Support",
                    IncludeAny = new[] { "unit_lance_support" }
                },
                new()
                {
                    Label = "Tank",
                    IncludeAny = new[] { "unit_lance_tank" }
                },
                new()
                {
                    Label = "Vanguard",
                    IncludeAny = new[] { "unit_lance_vanguard" }
                },
                new()
                {
                    Label = "Other",
                    ExcludeAny = new[]
                    {
                        "unit_lance_assassin",
                        "unit_lance_support",
                        "unit_lance_tank",
                        "unit_lance_vanguard"
                    }
                }
            }
        },
    };

    public class TagOptionsGroup
    {
        public string Label = "<null>";
        public TagOption[] Options = Array.Empty<TagOption>();
    }

    public class TagOption
    {
        public string Label = "<null>";
        public string[]? IncludeAny;
        public string[]? ExcludeAny;
        public bool OptionActive = false;
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
        public string[]? AllowAny;
        public string[]? BlockAny;
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