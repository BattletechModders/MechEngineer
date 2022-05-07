using System.Collections.Generic;
using BattleTech;
using CustomComponents;
using MechEngineer.Features.OverrideTonnage;

namespace MechEngineer.Features.Engines.Helper;

internal static class EngineSearcher
{
    internal static Result SearchInventory(IList<MechComponentRef> componentRefs)
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

            if (componentDef.Is<EngineCoreDef>(out var coreDef))
            {
                result.CoreDef = coreDef;
            }

            if (componentDef.Is<EngineHeatBlockDef>(out var blockDef))
            {
                result.HeatBlockDef = blockDef;
            }
        }

        result.WeightFactors = WeightsUtils.GetWeightFactorsFromInventory(componentRefs);

        return result;
    }

    internal class Result
    {
        internal CoolingDef? CoolingDef;
        internal EngineHeatBlockDef? HeatBlockDef;
        internal EngineCoreDef? CoreDef;
        internal WeightFactors WeightFactors = new();
        internal readonly List<MechComponentRef> HeatSinks = new();
    }
}