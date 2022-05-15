using System.Collections.Generic;
using System.Linq;
using BattleTech;
using CustomComponents;

namespace MechEngineer.Features.CriticalEffects;

[CustomComponent("StatusEffects")]
public class StatusEffectsCustom : SimpleCustomComponent, IListComponent<string>
{
    public void LoadList(IEnumerable<string> keyValues)
    {
        var effects = new List<EffectData>();
        foreach (var keyValue in keyValues)
        {
            var parts = keyValue.Split(',');
            var effectTemplateId = parts[0];
            var amount = parts.Length > 0 ? parts[1] : null;

            var effectTemplateData = CriticalEffectsFeature.GetEffectData(effectTemplateId);
            if (effectTemplateData == null)
            {
                continue;
            }

            var data = effectTemplateData.ToJSON()!;
            if (amount != null)
            {
                data = data.Replace("{amt}", amount);
            }
            var effect = new EffectData();
            effect.FromJSON(data);
            effects.Add(effect);
        }
        Def.statusEffects = Def.statusEffects.Union(effects).ToArray();
    }
}
