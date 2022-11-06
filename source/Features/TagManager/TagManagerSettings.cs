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
            AllowAny = new[] { MechValidationRules.LanceTag_Skirmish },
            BlockAny = new[] { MechValidationRules.LanceTag_Custom }
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
                AllowAny = new[] { MechValidationRules.LanceTag_Skirmish },
                BlockAny = new[] { MechValidationRules.LanceTag_Custom }
            }
        },
    };
    public const string SkirmishPresetsDescription = "If empty, no options panel is shown. Otherwise these are presets to quickly select a filter-combination.";

    public TagsFilterSet SkirmishOptionsDefault = new()
    {
        Components = new()
        {
            AllowAny = null,
            BlockAny = new[] { MechValidationRules.Tag_Blacklisted }
        },
        Mechs = new()
        {
            AllowAny = null,
            BlockAny = new[] { MechValidationRules.Tag_Blacklisted, MechValidationRules.MechTag_Unlocked, "VanillaOverride", "unit_override" }
        },
        Pilots = new()
        {
            AllowAny = Array.Empty<string>(),
        },
        Lances = new()
        {
            AllowAny = Array.Empty<string>(),
        }
    };
    public const string SkirmishOptionsDefaultDescription = "(Alpha) The options panel uses these defaults when dynamically combining filters or using the search filter.";

    public TagOptionsGroup[] SkirmishOptionsGroups =
    {
        new()
        {
            Label = "Tech Base",
            Options = new TagOption[]
            {
                new()
                {
                    Label = "Inner Sphere",
                    ExcludeAny = new[]
                    {
                        "unit_clan",
                        "unit_omni",
                        "unit_primitive",
                        "unit_prototype"
                    }
                },
                new()
                {
                    Label = "Clan",
                    IncludeAny = new[] { "unit_clan" }
                },
                new()
                {
                    Label = "Omni",
                    IncludeAny = new[] { "unit_omni" }
                },
                new()
                {
                    Label = "Primitive",
                    IncludeAny = new[] { "unit_primitive" }
                },
                new()
                {
                    Label = "Prototype",
                    IncludeAny = new[] { "unit_prototype" }
                }
            }
        },
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
                new()
                {
                    Label = "Super Heavy",
                    IncludeAny = new[] { "unit_superheavy" }
                },
                new()
                {
                    Label = "Other",
                    ExcludeAny = new[]
                    {
                        "unit_light",
                        "unit_medium",
                        "unit_heavy",
                        "unit_assault",
                        "unit_superheavy"
                    }
                },
            }
        },
        new()
        {
            Label = "'Mech Type",
            Options = new TagOption[]
            {
                new()
                {
                    Label = "BattleMech",
                    ExcludeAny = new[]
                    {
                        "unit_industrial",
                        "unit_protomech",
                        "unit_quad",
                        "unit_LAM",
                        "unit_vehicle",
                        "unit_powerarmor",
                    }
                },
                new()
                {
                    Label = "IndustrialMech",
                    IncludeAny = new[] { "unit_industrial" }
                },
                new()
                {
                    Label = "ProtoMech",
                    IncludeAny = new[] { "unit_protomech" }
                },
                new()
                {
                    Label = "Quadrupedal",
                    IncludeAny = new[] { "unit_quad" }
                },
                new()
                {
                    Label = "LAM",
                    IncludeAny = new[] { "unit_LAM" }
                }
            }
        },
        new()
        {
            Label = "Unit Type",
            Options = new TagOption[]
            {
                new()
                {
                    Label = "'Mech",
                    ExcludeAny = new[]
                    {
                        "unit_vehicle",
                        "unit_powerarmor",
                    }
                },
                new()
                {
                    Label = "Vehicle",
                    IncludeAny = new[] { "unit_vehicle" }
                },
                new()
                {
                    Label = "Battle Armor",
                    IncludeAny = new[] { "unit_powerarmor" }
                }
            }
        },
        new()
        {
            Label = "Rarity",
            Options = new TagOption[]
            {
                new()
                {
                    Label = "Normal",
                    ExcludeAny = new[]
                    {
                        "unit_elite",
                        "unit_hero",
                        "unit_legendary"
                    }
                },
                new()
                {
                    Label = "Elite",
                    IncludeAny = new[] { "unit_elite" }
                },
                new()
                {
                    Label = "Hero",
                    IncludeAny = new[] { "unit_hero" }
                },
                new()
                {
                    Label = "Legendary",
                    IncludeAny = new[] { "unit_legendary" }
                }
            }
        },
        new()
        {
            Label = "Era",
            Options = new TagOption[]
            {
                new()
                {
                    Label = "Clan Invasion",
                    IncludeAny = new[] { "unit_era_clan_invasion" }
                },
                new()
                {
                    Label = "Civil War",
                    IncludeAny = new[] { "unit_era_civil_war" }
                },
                new()
                {
                    Label = "Jihad",
                    IncludeAny = new[] { "unit_era_jihad" }
                },
                new()
                {
                    Label = "Republic",
                    IncludeAny = new[] { "unit_era_republic" }
                },
                new()
                {
                    Label = "Dark Age",
                    IncludeAny = new[] { "unit_era_dark_ages" }
                },
                new()
                {
                    Label = "Unknown",
                    ExcludeAny = new[]
                    {
                        "unit_era_clan_invasion",
                        "unit_era_civil_war",
                        "unit_era_jihad",
                        "unit_era_republic",
                        "unit_era_dark_ages"
                    }
                }
            }
        }
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
        [JsonIgnore]
        internal bool OptionActive = false;
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