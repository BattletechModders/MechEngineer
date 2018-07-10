using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BattleTech;
using BattleTech.Data;
using CustomComponents;

namespace MechEngineer
{
    internal class EngineCoreRef
    {
        private static readonly Regex Regex = new Regex(@"^(?:([^/]*))?(?:/([^/]+))?$", RegexOptions.Singleline | RegexOptions.Compiled);

        internal readonly MechComponentRef ComponentRef;
        internal readonly EngineCoreDef CoreDef;
        
        internal EngineHeatSink HeatSinkDef;
        
        private readonly Dictionary<EngineHeatSink, int> additionalHSCounts = new Dictionary<EngineHeatSink, int>();
        
        internal HSQuery Query(EngineHeatSink type)
        {
            return new HSQuery(this, type);
        }

        internal int AdditionalHeatSinkCount => additionalHSCounts.Values.Sum();

        internal string UUID;

        internal EngineCoreRef(EngineHeatSink heatSinkDef, EngineCoreDef coreDef) // only used for autofix, not fully supported
        {
            CoreDef = coreDef;
            HeatSinkDef = heatSinkDef;
        }

        internal EngineCoreRef(MechComponentRef componentRef, EngineCoreDef coreDef)
        {
            CoreDef = coreDef;
            ComponentRef = componentRef;

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
                HeatSinkDef = DataManager.GetDefaultEngineHeatSinkDef();
            }
        }

        internal DataManager DataManager => ComponentRef.DataManager;

        private string Properties
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                var dictionary = DictionarySerializer.FromString(value);
                if (dictionary.TryGetValue("ihstype", out var defId))
                {
                    dictionary.Remove("ihstype");
                    HeatSinkDef = DataManager.GetEngineHeatSinkDef(defId);
                    if (HeatSinkDef == null)
                    {
                        Control.mod.Logger.LogError("ihstype - can't find EngineHeatSink with id=" + defId);
                    }
                }

                foreach (var keyvalue in dictionary)
                {
                    var heatSinkDef = DataManager.Get(BattleTechResourceType.HeatSinkDef, keyvalue.Key) as HeatSinkDef;
                    var type = heatSinkDef?.GetComponent<EngineHeatSink>();
                    if (type == null)
                    {
                        Control.mod.Logger.LogError("can't find EngineHeatSink with id=" + keyvalue.Key);
                        continue;
                    }
                    Query(type).AdditionalCount = int.Parse(keyvalue.Value);
                }
            }
            get
            {
                var dictionary = new Dictionary<string, string>();
                if (HeatSinkDef != DataManager.GetDefaultEngineHeatSinkDef())
                {
                    dictionary["ihstype"] = HeatSinkDef.Def.Description.Id;
                }

                foreach (var type in additionalHSCounts.Keys)
                {
                    var count = Query(type).AdditionalCount;
                    if (count > 0)
                    {
                        dictionary[type.Def.Description.Id] = count.ToString();
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
                    dissipation += keyvalue.Key.HeatSinkDef.DissipationCapacity * keyvalue.Value;
                }
                dissipation += HeatSinkDef.HeatSinkDef.DissipationCapacity * CoreDef.InternalHeatSinks;

                // can't enforce heatsinkdef earlier as apparently in same cases the Def is a generic one and does not derive from HeatSinkDef (Tooltips)
                dissipation += CoreDef.HeatSinkDef.DissipationCapacity;

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
                    return $"{HeatSinkDef.Abbreviation} {CoreDef.InternalHeatSinks + AdditionalHeatSinkCount} / {CoreDef.MaxInternalHeatSinks}";
                }
                else
                {
                    return $"{HeatSinkDef.Abbreviation} {CoreDef.InternalHeatSinks}";
                }
            }
        }

        internal float InternalHeatSinkTonnage => AdditionalHeatSinkCount * HeatSinkDef.HeatSinkDef.Tonnage;

        internal string GetNewSimGameUID()
        {
            var props = Properties;
            if (!string.IsNullOrEmpty(props))
            {
                props = "/" + props;
            }
            return UUID + props;
        }

        internal IEnumerable<string> GetInternalComponents()
        {
            var heatSinkDefId = HeatSinkDef.Def.Description.Id;
            var kit = DataManager.GetAllEngineHeatSinkKitDefs().FirstOrDefault(c => c.HeatSinkDefId == heatSinkDefId);
            if (kit != null)
            {
                yield return kit.Def.Description.Id;
            }

            foreach (var type in additionalHSCounts.Keys)
            {
                for (var i = 0; i < Query(type).AdditionalCount; i++)
                {
                    yield return type.Def.Description.Id;
                }
            }
        }
        
        internal class HSQuery
        {
            private readonly EngineCoreRef coreRef;
            private readonly EngineHeatSink heatSinkDef;

            internal HSQuery(EngineCoreRef coreRef, EngineHeatSink heatSinkDef)
            {
                this.coreRef = coreRef;
                this.heatSinkDef = heatSinkDef;
            }

            internal bool IsType => coreRef.HeatSinkDef == heatSinkDef;

            internal int InternalCount => IsType ? coreRef.CoreDef.InternalHeatSinks : 0;

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