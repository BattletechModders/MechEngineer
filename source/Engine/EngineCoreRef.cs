using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BattleTech;

namespace MechEngineer
{
    internal enum HeatSinkType
    {
        SHS, DHS, CDHS
    }

    internal static class HeatSinkTypeExtensions
    {
        internal static string Abbrev(this HeatSinkType @this)
        {
            return @this.ToString();
        }

        internal static string AAbbrev(this HeatSinkType @this)
        {
            return "a" + @this.ToString().ToLowerInvariant();
        }

        internal static string Full(this HeatSinkType @this)
        {
            switch (@this)
            {
                case HeatSinkType.DHS:
                    return "Double Heat Sink";
                case HeatSinkType.CDHS:
                    return "Clan Double Heat Sink";
                default:
                    return "Standard Heat Sink";
            }
        }

        internal static IEnumerable<HeatSinkType> Types()
        {
            return Enum.GetValues(typeof(HeatSinkType)).Cast<HeatSinkType>();
        }
    }

    internal class EngineCoreRef
    {
        internal class HeatSinkQuery
        {
            private readonly EngineCoreRef CoreRef;
            private readonly HeatSinkType Key;

            internal HeatSinkQuery(EngineCoreRef coreRef, HeatSinkType key)
            {
                CoreRef = coreRef;
                Key = key;
            }

            internal bool IsType => CoreRef. HSType == Key;

            internal int InternalCount => IsType ? CoreRef.CoreDef.MinHeatSinks : 0;

            internal int AdditionalCount
            {
                set => CoreRef.AdditionalHSCounts[Key] = value;
                get => CoreRef.AdditionalHSCounts.TryGetValue(Key, out var value) ? value : 0;
            }
        }

        private static readonly Regex Regex = new Regex(@"^(?:([^/]*))(?:/([^/]+))?$", RegexOptions.Singleline | RegexOptions.Compiled);

        internal readonly MechComponentRef ComponentRef;
        internal readonly EngineCoreDef CoreDef;

        internal HeatSinkType HSType;
        
        private readonly Dictionary<HeatSinkType, int> AdditionalHSCounts = new Dictionary<HeatSinkType, int>();

        internal HeatSinkQuery Query(HeatSinkType type)
        {
            return new HeatSinkQuery(this, type);
        }

        internal int AdditionalHeatSinkCount => AdditionalHSCounts.Values.Sum();

        internal string UUID;

        internal EngineCoreRef(MechComponentRef componentRef, EngineCoreDef coreDef)
        {
            ComponentRef = componentRef;
            CoreDef = coreDef;

            var text = componentRef.SimGameUID;

            if (string.IsNullOrEmpty(text))
            {
                if (text != null)
                {
                    componentRef.SetSimGameUID(null);
                }

                return;
            }

            var match = Regex.Match(text);
            if (!match.Success)
            {
                return;
            }

            UUID = string.IsNullOrEmpty(match.Groups[1].Value) ? null : match.Groups[1].Value;
            Properties = match.Groups[2].Value;
        }

        internal bool Is(HeatSinkType type)
        {
            return HSType == type;
        }

        private string Properties
        {
            set
            {
                var dictionary = DictionarySerializer.FromString(value);
                if (dictionary.ContainsKey("ihstype"))
                {
                    var typeString = dictionary["ihstype"];
                    try
                    {
                        HSType = (HeatSinkType) Enum.Parse(typeof(HeatSinkType), typeString, true);
                    }
                    catch (ArgumentException e)
                    {
                        Control.mod.Logger.LogError("can't parse heatsink type", e);
                        HSType = HeatSinkType.SHS;
                    }
                }

                foreach (var type in HeatSinkTypeExtensions.Types())
                {
                    var aname = type.AAbbrev();
                    Query(type).AdditionalCount = dictionary.ContainsKey(aname) ? int.Parse(dictionary[aname]) : 0;
                }
            }
            get
            {
                var dictionary = new Dictionary<string, string>();
                if (!Is(HeatSinkType.SHS))
                {
                    dictionary["ihstype"] = HSType.Abbrev().ToLowerInvariant();
                }

                foreach (var type in HeatSinkTypeExtensions.Types())
                {
                    var count = Query(type).AdditionalCount;
                    if (count > 0)
                    {
                        var aname = type.AAbbrev();
                        dictionary[aname] = count.ToString();
                    }
                }

                return DictionarySerializer.ToString(dictionary);
            }
        }

        internal float EngineHeatDissipation
        {
            get
            {
                var dissipation = Query(HeatSinkType.SHS).AdditionalCount * Control.Combat.Heat.DefaultHeatSinkDissipationCapacity;
                dissipation += Query(HeatSinkType.DHS).AdditionalCount * Control.Combat.Heat.DefaultHeatSinkDissipationCapacity * 2;
                dissipation += Query(HeatSinkType.CDHS).AdditionalCount * Control.Combat.Heat.DefaultHeatSinkDissipationCapacity * 2;
                dissipation += (Is(HeatSinkType.SHS) ? 1 : 2) * CoreDef.MinHeatSinks * Control.Combat.Heat.DefaultHeatSinkDissipationCapacity;

                // can't enforce heatsinkdef earlier as apparently in same cases the Def is a generic one and does not derive from HeatSinkDef (Tooltips)
                dissipation += CoreDef.DissipationCapacity;

                //Control.mod.Logger.LogDebug("GetHeatDissipation rating=" + engineDef.Rating + " minHeatSinks=" + minHeatSinks + " additionalHeatSinks=" + engineProps.AdditionalHeatSinkCount + " dissipation=" + dissipation);

                return dissipation;
            }
        }

        internal string BonusValueA
        {
            get { return string.Format("- {0} Heat", EngineHeatDissipation); }
        }

        internal string BonusValueB
        {
            get
            {
                var bonusText = HSType.Abbrev();
                if (CoreDef.MaxAdditionalHeatSinks > 0)
                {
                    bonusText += string.Format(" {0} / {1}", CoreDef.MinHeatSinks + AdditionalHeatSinkCount, CoreDef.MaxHeatSinks);
                }
                else
                {
                    bonusText += string.Format(" {0}", CoreDef.MinHeatSinks);
                }

                return bonusText;
            }
        }

        internal float HeatSinkTonnage
        {
            get { return AdditionalHeatSinkCount * 1; }
        }

        internal string GetNewSimGameUID()
        {
            return (string.IsNullOrEmpty(UUID) ? "" : UUID) + "/" + Properties;
        }

        internal IEnumerable<string> GetInternalComponents()
        {
            if (Is(HeatSinkType.DHS))
            {
                yield return Control.settings.EngineKitDHS;
            }

            if (Is(HeatSinkType.CDHS))
            {
                yield return Control.settings.EngineKitCDHS;
            }

            for (var i = 0; i < Query(HeatSinkType.SHS).AdditionalCount; i++)
            {
                yield return Control.settings.GearHeatSinkStandard;
            }

            for (var i = 0; i < Query(HeatSinkType.DHS).AdditionalCount; i++)
            {
                yield return Control.settings.GearHeatSinkDouble;
            }

            for (var i = 0; i < Query(HeatSinkType.CDHS).AdditionalCount; i++)
            {
                yield return Control.settings.GearHeatSinkDoubleClan;
            }
        }
    }
}