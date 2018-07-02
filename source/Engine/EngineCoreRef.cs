using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BattleTech;
using BattleTech.Data;

namespace MechEngineer
{
    internal class EngineCoreRef
    {
        private static readonly Regex Regex = new Regex(@"^(?:([^/]*))(?:/([^/]+))?$", RegexOptions.Singleline | RegexOptions.Compiled);

        internal readonly MechComponentRef ComponentRef;
        internal readonly EngineCoreDef CoreDef;
        
        internal EngineHeatSinkDef HeatSinkDef;
        
        private readonly Dictionary<EngineHeatSinkDef, int> additionalHSCounts = new Dictionary<EngineHeatSinkDef, int>();
        
        internal HSQuery Query(EngineHeatSinkDef type)
        {
            return new HSQuery(this, type);
        }

        internal int AdditionalHeatSinkCount => additionalHSCounts.Values.Sum();

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
            }
            else
            {
                var match = Regex.Match(text);
                if (match.Success)
                {
                    UUID = string.IsNullOrEmpty(match.Groups[1].Value) ? null : match.Groups[1].Value;
                    Properties = match.Groups[2].Value;
                }
            }

            if (HeatSinkDef == null)
            {
                HeatSinkDef = DataManager.GetStandardHeatSinkDef();
            }
        }

        internal DataManager DataManager => ComponentRef.DataManager;

        private string Properties
        {
            set
            {
                var dictionary = DictionarySerializer.FromString(value);
                if (dictionary.TryGetValue("ihstype", out var defId))
                {
                    dictionary.Remove("ihstype");
                    HeatSinkDef = DataManager.GetEngineHeatSinkDef(defId);
                    Control.mod.Logger.LogError("ihstype - can't find EngineHeatSinkDef with id=" + defId);
                }

                foreach (var keyvalue in dictionary)
                {
                    if (!(DataManager.Get(BattleTechResourceType.HeatSinkDef, keyvalue.Key) is EngineHeatSinkDef type))
                    {
                        Control.mod.Logger.LogError("can't find EngineHeatSinkDef with id=" + keyvalue.Key);
                        continue;
                    }
                    Query(type).AdditionalCount = int.Parse(keyvalue.Value);
                }
            }
            get
            {
                var dictionary = new Dictionary<string, string>();
                if (HeatSinkDef != DataManager.GetStandardHeatSinkDef())
                {
                    dictionary["ihstype"] = HeatSinkDef.Description.Id;
                }

                foreach (var type in additionalHSCounts.Keys)
                {
                    var count = Query(type).AdditionalCount;
                    if (count > 0)
                    {
                        dictionary[type.Description.Id] = count.ToString();
                    }
                }

                return DictionarySerializer.ToString(dictionary);
            }
        }

        internal float EngineHeatDissipation
        {
            get
            {
                float dissipation = 0;
                foreach (var keyvalue in additionalHSCounts)
                {
                    dissipation += keyvalue.Key.DissipationCapacity * keyvalue.Value;
                }
                dissipation += HeatSinkDef.DissipationCapacity * CoreDef.MinHeatSinks;

                // can't enforce heatsinkdef earlier as apparently in same cases the Def is a generic one and does not derive from HeatSinkDef (Tooltips)
                dissipation += CoreDef.DissipationCapacity;

                //Control.mod.Logger.LogDebug("GetHeatDissipation rating=" + engineDef.Rating + " minHeatSinks=" + minHeatSinks + " additionalHeatSinks=" + engineProps.AdditionalHeatSinkCount + " dissipation=" + dissipation);

                return dissipation;
            }
        }

        internal string BonusValueA => $"- {EngineHeatDissipation} Heat";

        internal string BonusValueB
        {
            get
            {
                if (CoreDef.MaxAdditionalHeatSinks > 0)
                {
                    return $"{HeatSinkDef.Abbreviation} {CoreDef.MinHeatSinks + AdditionalHeatSinkCount} / {CoreDef.MaxHeatSinks}";
                }
                else
                {
                    return $"{HeatSinkDef.Abbreviation} {CoreDef.MinHeatSinks}";
                }
            }
        }

        internal float HeatSinkTonnage => AdditionalHeatSinkCount * 1;

        internal string GetNewSimGameUID()
        {
            return (string.IsNullOrEmpty(UUID) ? "" : UUID) + "/" + Properties;
        }

        internal IEnumerable<string> GetInternalComponents()
        {
            var kit = DataManager.GetAllEngineHeatSinkKitDefDefs().FirstOrDefault(c => c.HeatSinkDef == HeatSinkDef);
            if (kit != null)
            {
                yield return kit.Description.Id;
            }

            foreach (var type in additionalHSCounts.Keys)
            {
                for (var i = 0; i < Query(type).AdditionalCount; i++)
                {
                    yield return type.Description.Id;
                }
            }
        }
        
        internal class HSQuery
        {
            private readonly EngineCoreRef coreRef;
            private readonly EngineHeatSinkDef heatSinkDef;

            internal HSQuery(EngineCoreRef coreRef, EngineHeatSinkDef heatSinkDef)
            {
                this.coreRef = coreRef;
                this.heatSinkDef = heatSinkDef;
            }

            internal bool IsType => coreRef.HeatSinkDef == heatSinkDef;

            internal int InternalCount => IsType ? coreRef.CoreDef.MinHeatSinks : 0;

            internal int AdditionalCount
            {
                set
                {
                    if (value > 0)
                    {
                        coreRef.additionalHSCounts[heatSinkDef] = value;
                    }
                    else
                    {
                        coreRef.additionalHSCounts.Remove(heatSinkDef);
                    } 
                }
                get => coreRef.additionalHSCounts.TryGetValue(heatSinkDef, out var value) ? value : 0;
            }

            public int Count => InternalCount + AdditionalCount;
        }
    }
}