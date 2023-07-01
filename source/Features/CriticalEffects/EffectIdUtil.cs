using System.Linq;
using BattleTech;
using MechEngineer.Features.CriticalEffects.Patches;
using MechEngineer.Features.PlaceholderEffects;

namespace MechEngineer.Features.CriticalEffects;

internal class EffectIdUtil
{
    private readonly string templateEffectId;
    private readonly string resolvedEffectId;
    private readonly MechComponent mechComponent;

    internal EffectIdUtil(string templateEffectId, string resolvedEffectId, MechComponent mechComponent)
    {
        this.templateEffectId = templateEffectId;
        this.resolvedEffectId = $"MECriticalHitEffect_{resolvedEffectId}_{mechComponent.parent.GUID}";
        this.mechComponent = mechComponent;
    }

    internal static void CreateEffect(MechComponent component, EffectData effectData, string effectId)
    {
        var actor = component.parent;

        Log.Main.Debug?.Log($"Creating id={effectId} statName={effectData.statisticData.statName}");

        EffectManager_GetTargetStatCollections_Patch.SetContext(component, effectData);
        try
        {
            actor.Combat.EffectManager.CreateEffect(effectData, effectId, -1, actor, actor, default, 0);
        }
        finally
        {
            EffectManager_GetTargetStatCollections_Patch.ClearContext();
        }
    }

    internal void CreateCriticalEffect()
    {
        var effectData = CriticalEffectsFeature.GetEffectData(templateEffectId);
        if (effectData == null)
        {
            return;
        }
        if (effectData.targetingData.effectTriggerType != EffectTriggerType.Passive) // we only support passive for now
        {
            Log.Main.Warning?.Log($"Effect templateEffectId={templateEffectId} is not passive");
            return;
        }

        PlaceholderEffectsFeature.ProcessLocationalEffectData(ref effectData, mechComponent);

        CreateEffect(mechComponent, effectData, resolvedEffectId);
    }

    internal void CancelCriticalEffect()
    {
        var actor = mechComponent.parent;
        var statusEffects = actor.Combat.EffectManager
            .GetAllEffectsWithID(resolvedEffectId)
            .Where(e => e.Target == actor);

        Log.Main.Debug?.Log($"Canceling id={resolvedEffectId}");
        foreach (var statusEffect in statusEffects)
        {
            Log.Main.Debug?.Log($"Canceling statName={statusEffect.EffectData.statisticData.statName}");
            actor.CancelEffect(statusEffect);
        }
    }
}