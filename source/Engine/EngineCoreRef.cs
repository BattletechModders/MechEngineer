using System.Collections.Generic;
using System.Text.RegularExpressions;
using BattleTech;
using Newtonsoft.Json;

namespace MechEngineer
{
    internal partial class EngineCoreRef
    {
        internal bool IsDHS;
        internal bool IsSHS
        {
            get { return !IsDHS; }
        }

        internal int InternalSHSCount
        {
            get { return IsSHS ? CoreDef.MinHeatSinks : 0; }
        }
        internal int InternalDHSCount
        {
            get { return IsDHS ? CoreDef.MinHeatSinks : 0; }
        }

        internal int AdditionalDHSCount;
        internal int AdditionalSHSCount;
        internal int AdditionalHeatSinkCount
        {
            get { return AdditionalSHSCount + AdditionalDHSCount; }
        }

        internal EngineType Type;

        internal string UUID;

        internal readonly MechComponentRef ComponentRef;
        internal readonly EngineCoreDef CoreDef;

        private static readonly Regex Regex = new Regex(@"^(?:([^/]*))(?:/([^/]+))?$", RegexOptions.Singleline | RegexOptions.Compiled);

        internal EngineCoreRef(MechComponentRef componentRef, EngineCoreDef coreDef, EngineType engineType)
        {
            ComponentRef = componentRef;
            CoreDef = coreDef;
            Type = engineType;

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

        private string Properties
        {
            set
            {
                var dictionary = DictionarySerializer.FromString(value);
                IsDHS = dictionary.ContainsKey("ihstype") && dictionary["ihstype"] == "dhs";
                AdditionalSHSCount = dictionary.ContainsKey("ashs") ? int.Parse(dictionary["ashs"]) : 0;
                AdditionalDHSCount = dictionary.ContainsKey("adhs") ? int.Parse(dictionary["adhs"]) : 0;
            }
            get
            {
                var dictionary = new Dictionary<string, string>();
                if (IsDHS)
                {
                    dictionary["ihstype"] = "dhs";
                }

                if (AdditionalSHSCount > 0)
                {
                    dictionary["ashs"] = AdditionalSHSCount.ToString();
                }

                if (AdditionalDHSCount > 0)
                {
                    dictionary["adhs"] = AdditionalDHSCount.ToString();
                }

                return DictionarySerializer.ToString(dictionary);
            }
        }

        internal string GetNewSimGameUID()
        {
            return (string.IsNullOrEmpty(UUID) ? "" : UUID) + "/" + Properties;
        }

        internal IEnumerable<string> GetInternalComponents()
        {
            if (IsDHS)
            {
                yield return Control.settings.EngineKitDHS;
            }

            for (var i = 0; i < AdditionalSHSCount; i++)
            {
                yield return Control.settings.GearHeatSinkStandard;
            }

            for (var i = 0; i < AdditionalDHSCount; i++)
            {
                yield return Control.settings.GearHeatSinkDouble;
            }
        }

        internal float EngineHeatDissipation
        {
            get
            {
                var dissipation = AdditionalDHSCount * Control.Combat.Heat.DefaultHeatSinkDissipationCapacity * 2;
                dissipation += AdditionalSHSCount * Control.Combat.Heat.DefaultHeatSinkDissipationCapacity;
                dissipation += (IsDHS ? 2 : 1) * CoreDef.MinHeatSinks * Control.Combat.Heat.DefaultHeatSinkDissipationCapacity;

                // can't enforce heatsinkdef earlier as apparently in same cases the Def is a generic one and does not derive from HeatSinkDef (Tooltips)
                var heatSinkDef = CoreDef.Def as HeatSinkDef;
                if (heatSinkDef != null)
                {
                    dissipation += heatSinkDef.DissipationCapacity;
                }

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
                var bonusText = IsDHS ? "DHS" : "SHS";
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

        internal float EngineTonnage
        {
            get { return (CoreDef.StandardEngineTonnage * (Type == null ? 1.0f : Type.WeightMultiplier)).RoundStandard();}
        }

        internal float Tonnage
        {
            get { return HeatSinkTonnage + EngineTonnage + CoreDef.GyroTonnage; }
        }

        public float TonnageChanges
        {
            get { return - CoreDef.Def.Tonnage + Tonnage; }
        }
    }
}