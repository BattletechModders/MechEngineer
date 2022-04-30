using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;
using MechEngineer.Features.Engines.Helper;
using MechEngineer.Features.OverrideTonnage;
using UnityEngine;

namespace MechEngineer.Features.Engines;

// Public due necessity to have access from BattleValue module -- bhtrail
public class Engine
{
    // Declared public to have access from BattleValue module -- bhtrail
    public static Engine GetEngine(ChassisDef chassisDef, IList<MechComponentRef> componentRefs)
    {
        var result = EngineSearcher.SearchInventory(componentRefs);
        if (result.CoolingDef == null || result.CoreDef == null || result.HeatBlockDef == null)
        {
            return null;
        }

        if (chassisDef.ChassisTags.Contains(EngineFeature.settings.ProtoMechEngineTag))
        {
            return new ProtoMechEngine(result);
        }

        return new Engine(result);
    }

    // Declared internal to have access from BattleValue module -- bhtrail
    internal Engine(EngineSearcher.Result result) : this(result.CoolingDef, result.HeatBlockDef, result.CoreDef, result.WeightFactors, result.HeatSinks)
    {
    }

    // should be private but used during autofixer, rename EngineSearcher.Result to .Builder and apply new semantics
    internal Engine(
        CoolingDef coolingDef,
        EngineHeatBlockDef heatBlockDef,
        EngineCoreDef coreDef,
        WeightFactors weightFactors,
        List<MechComponentRef> heatSinksExternal,
        bool calculate = true)
    {
        HeatSinksExternal = heatSinksExternal;
        HeatBlockDef = heatBlockDef;
        CoreDef = coreDef;
        WeightFactors = weightFactors;
        CoolingDef = coolingDef;
        if (calculate)
        {
            CalculateStats();
        }
    }

    private static int MatchingCount(IEnumerable<MechComponentRef> heatSinks, HeatSinkDef heatSinkDef)
    {
        return heatSinks.Select(r => r.Def).Count(d => d == heatSinkDef);
    }

    private CoolingDef _coolingDef;
    // Declared public to have access from BattleValue module -- bhtrail
    public CoolingDef CoolingDef
    {
        get => _coolingDef;
        set
        {
            _coolingDef = value;
            var id = _coolingDef.HeatSinkDefId;
            var def = UnityGameInstance.BattleTechGame.DataManager.HeatSinkDefs.Get(id);
            HeatSinkDef = def.GetComponent<EngineHeatSinkDef>();
        }
    }

    internal void CalculateStats()
    {
        HeatSinkExternalCount = MatchingCount(HeatSinksExternal, HeatSinkDef.Def);
    }

    // Declared public to have access from BattleValue module -- bhtrail
    public List<MechComponentRef> HeatSinksExternal { get; set; }
    private int HeatSinkExternalCount { get; set; }

    // Declared public to have access from BattleValue module -- bhtrail
    public EngineCoreDef CoreDef { get; set; }
    internal WeightFactors WeightFactors { get; set; }
    // Declared public to have access from BattleValue module -- bhtrail
    public EngineHeatBlockDef HeatBlockDef { get; set; } // amount of internal heat sinks
    // Declared public to have access from BattleValue module -- bhtrail
    public EngineHeatSinkDef HeatSinkDef { get; set; } // type of internal heat sinks and compatible external heat sinks

    // Declared public to have access from BattleValue module -- bhtrail
    public float EngineHeatDissipation
    {
        get
        {
            var dissipation = HeatSinkDef.Def.DissipationCapacity * (HeatSinkInternalFreeMaxCount + HeatBlockDef.HeatSinkCount);
            dissipation += CoreDef.Def.DissipationCapacity;
            dissipation += CoolingDef.Def.DissipationCapacity;
            return dissipation;
        }
    }

    #region heat sink counting

    internal int HeatSinkExternalFreeCount => Mathf.Min(HeatSinkExternalCount, HeatSinkExternalFreeMaxCount);
    internal int HeatSinkExternalAdditionalCount => HeatSinkExternalCount - HeatSinkExternalFreeCount;

    private int HeatSinkTotalCount => HeatSinkInternalCount + HeatSinkExternalCount;
    internal int HeatSinkInternalCount => HeatSinkInternalFreeMaxCount + HeatBlockDef.HeatSinkCount;

    private int HeatSinksFreeMaxCount => EngineFeature.settings.MinimumHeatSinksOnMech;
    private int HeatSinksInternalMaxCount => CoreDef.Rating / 25;

    protected virtual int HeatSinkTotalMinCount => HeatSinksFreeMaxCount;

    internal virtual int HeatSinkInternalFreeMaxCount => Mathf.Min(HeatSinksFreeMaxCount, HeatSinksInternalMaxCount);
    internal virtual int HeatSinkInternalAdditionalMaxCount => Mathf.Max(0, HeatSinksInternalMaxCount - HeatSinksFreeMaxCount);
    internal virtual int HeatSinkExternalFreeMaxCount => HeatSinksFreeMaxCount - HeatSinkInternalFreeMaxCount;

    internal bool IsMissingHeatSinks(out int min, out int count)
    {
        min = HeatSinkTotalMinCount;
        count = HeatSinkTotalCount;
        return count < min;
    }

    #endregion

    #region weights

    internal float HeatSinkExternalFreeTonnage => HeatSinkExternalFreeCount * HeatSinkDef.Def.Tonnage;
    internal float GyroTonnage => PrecisionUtils.RoundUp(StandardGyroTonnage * WeightFactors.GyroFactor, WeightPrecision);
    internal float EngineTonnage => PrecisionUtils.RoundUp(StandardEngineTonnage * WeightFactors.EngineFactor, WeightPrecision);
    internal float HeatSinkTonnage => -HeatSinkExternalFreeTonnage;
    internal float TotalTonnage => HeatSinkTonnage + EngineTonnage + GyroTonnage;

    internal virtual float StandardGyroTonnage => PrecisionUtils.RoundUp(CoreDef.Rating / 100f, 1f);
    internal virtual float StandardEngineTonnage => CoreDef.Def.Tonnage - StandardGyroTonnage;

    internal virtual float WeightPrecision => OverrideTonnageFeature.settings.TonnageStandardPrecision;

    #endregion
}

internal class ProtoMechEngine : Engine
{
    internal ProtoMechEngine(EngineSearcher.Result result) : base(result)
    {
    }

    protected override int HeatSinkTotalMinCount => 0;

    internal override int HeatSinkInternalFreeMaxCount => 0;
    internal override int HeatSinkInternalAdditionalMaxCount => 0;
    internal override int HeatSinkExternalFreeMaxCount => 0;

    internal override float StandardGyroTonnage => 0;
    internal override float StandardEngineTonnage => CoreDef.Def.Tonnage;

    internal override float WeightPrecision => OverrideTonnageFeature.settings.KilogramStandardPrecision;
}