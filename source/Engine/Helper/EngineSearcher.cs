using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    internal class EngineSearcher
    {
        internal static Result SearchInventory(IEnumerable<MechComponentRef> componentRefs)
        {
            var result = new Result();
            foreach (var componentRef in componentRefs)
            {
                var componentDef = componentRef.Def;

                var heatsink = componentDef.GetComponent<EngineHeatSink>();
                if (heatsink != null)
                {
                    result.ExternalHeatSinkTonnage += componentDef.Tonnage;
                    continue;
                }

                var enginePart = componentDef.GetComponent<EnginePart>();
                if (enginePart == null)
                {
                    continue;
                }

                result.Parts.Add(componentRef);

                if (result.CoreRef == null && componentDef.GetComponent<EngineCoreDef>() != null)
                {
                    result.CoreRef = componentRef.GetEngineCoreRef();
                    continue;
                }

                if (result.Type == null)
                {
                    result.Type = componentDef.GetComponent<EngineType>();
                }
            }

            return result;
        }

        internal class Result
        {
            internal EngineCoreRef CoreRef;
            internal List<MechComponentRef> Parts = new List<MechComponentRef>();
            internal EngineType Type;
            internal float ExternalHeatSinkTonnage;
        }
    }
}