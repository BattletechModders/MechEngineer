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
                    result.HeatSinks.Add(componentRef);
                    continue;
                }

                if (componentDef.Is<Weights>(out var weightSavings))
                {
                    result.Weights.Combine(weightSavings);
                }
                
                if (!componentDef.IsEnginePart())
                {
                    continue;
                }

                result.Parts.Add(componentRef);

                if (result.CoreRef == null && componentDef.GetComponent<EngineCoreDef>() != null)
                {
                    result.CoreRef = componentRef.GetEngineCoreRef();
                }
            }

            return result;
        }

        internal class Result
        {
            internal EngineCoreRef CoreRef;
            internal List<MechComponentRef> Parts = new List<MechComponentRef>();
            internal Weights Weights = new Weights();
            internal List<MechComponentRef> HeatSinks = new List<MechComponentRef>();
        }
    }
}