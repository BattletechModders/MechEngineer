using System.Collections.Generic;
using BattleTech;
using CustomComponents;
using MechEngineer.Features.OverrideTonnage;

namespace MechEngineer.Features.Engines.Helper
{
    internal class EngineSearcher
    {
        internal static Result SearchInventory(IEnumerable<MechComponentRef> componentRefs)
        {
            var result = new Result();
            foreach (var componentRef in componentRefs)
            {
                var componentDef = componentRef.Def;
                
                if (componentDef.Is<EngineHeatSinkDef>())
                {
                    result.HeatSinks.Add(componentRef);
                }

                if (componentDef.Is<CoolingDef>(out var coolingDef))
                {
                    result.CoolingDef = coolingDef;
                }

                if (componentDef.Is<Weights>(out var weightSavings))
                {
                    result.Weights.Combine(weightSavings);
                }

                if (componentDef.Is<EngineCoreDef>(out var coreDef))
                {
                    result.CoreDef = coreDef;
                }

                if (componentDef.Is<EngineHeatBlockDef>(out var blockDef))
                {
                    result.HeatBlockDef = blockDef;
                }
            }

            return result;
        }

        internal class Result
        {
            internal CoolingDef CoolingDef;
            internal EngineHeatBlockDef HeatBlockDef;
            internal EngineCoreDef CoreDef;
            internal Weights Weights = new Weights();
            internal List<MechComponentRef> HeatSinks = new List<MechComponentRef>();
        }
    }
}