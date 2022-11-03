﻿using BattleTech;
using fastJSON;
using HBS.Collections;

namespace MechEngineer.Features.TagManager;

public class TagManagerSettings : ISettings
{
    public bool Enabled { get; set; }
    public string EnabledDescription => $"Manipulates Tags on Components, Mechs, Pilots and Lances by adding ({nameof(TagsFilter.Blacklist)}) or removing ({nameof(TagsFilter.Whitelist)}) {MechValidationRules.Tag_Blacklisted}.";

    public int SimGameItemsMinCount;
    public string SimGameItemsMinCountDescription = $"Set the owned minimum count of each mech component in SimGame.";

    public bool LostechStockWeaponVariantFix { get; set; } = true;
    public string LostechStockWeaponVariantDescription => "Fixes lostech variant weapon tagging by checking if id ends with -STOCK.";

    public TagsFilter Components = new()
    {
        SkirmishAllow = new [] { MechValidationRules.ComponentTag_Stock },
        SkirmishBlock = new [] { MechValidationRules.Tag_Blacklisted },
        Whitelist = new [] { MechValidationRules.ComponentTag_Stock, MechValidationRules.ComponentTag_Variant, MechValidationRules.ComponentTag_LosTech },
        Blacklist = new[] { MechValidationRules.ComponentTag_Debug },
    };

    public TagsFilter Mechs = new()
    {
        SkirmishAllow = new[] { MechValidationRules.MechTag_Released },
        // TODO fix so custom does not have to be blocked anymore
        SkirmishBlock = new [] { MechValidationRules.Tag_Blacklisted, MechValidationRules.MechTag_Unlocked, MechValidationRules.MechTag_Custom },
        Whitelist = new[] { MechValidationRules.MechTag_Released },
    };

    public TagsFilter Pilots = new()
    {
        SkirmishAllow = new[] { MechValidationRules.PilotTag_Released },
    };

    public TagsFilter Lances = new()
    {
        SkirmishAllow = new[] { MechValidationRules.LanceTag_Skirmish },
        // TODO fix so custom does not have to be blocked anymore
        SkirmishBlock = new [] { MechValidationRules.LanceTag_Custom },
        Whitelist = new[] { MechValidationRules.LanceTag_Released },
    };

    public class TagsFilter
    {
        public string[] SkirmishAllow = { };
        public string[] SkirmishBlock = { };

        public string[] Whitelist = { };
        public string[] Blacklist = { };

        public bool? SkirmishForce;

        [JsonIgnore]
        internal TagSet SkirmishAllowTagSet = null!;
        [JsonIgnore]
        internal TagSet SkirmishBlockTagSet = null!;

        [JsonIgnore]
        internal TagSet WhitelistTagSet = null!;
        [JsonIgnore]
        internal TagSet BlacklistTagSet = null!;

        internal void Complete()
        {
            SkirmishAllowTagSet = new(SkirmishAllow);
            SkirmishBlockTagSet = new(SkirmishBlock);
            WhitelistTagSet = new(Whitelist);
            BlacklistTagSet = new(Blacklist);
        }
    }

    internal void Complete()
    {
        Components.Complete();
        Mechs.Complete();
        Pilots.Complete();
        Lances.Complete();
    }
}