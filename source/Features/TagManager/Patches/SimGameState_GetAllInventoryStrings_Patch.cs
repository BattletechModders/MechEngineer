using System;
using BattleTech;
using Harmony;
using HBS.Data;
using MechEngineer.Misc;

namespace MechEngineer.Features.TagManager.Patches;

[HarmonyPatch(typeof(SimGameState), nameof(SimGameState.GetAllInventoryStrings))]
public static class SimGameState_GetAllInventoryStrings_Patch
{
    [UsedByHarmony]
    public static bool Prepare()
    {
        return TagManagerFeature.Shared.Settings.SimGameItemsMinCount > 0;
    }

    [HarmonyPrefix]
    public static void Prefix(SimGameState __instance)
    {
        try
        {
            var feature = TagManagerFeature.Shared;
            var state = __instance;
            var minCount = feature.Settings.SimGameItemsMinCount;

            void AddApplicable<T>(DictionaryStore<T> store) where T : MechComponentDef, new()
            {
                foreach (var def in store.items.Values)
                {
                    if (!feature.ComponentIsValidForSkirmish(def, false))
                    {
                        continue;
                    }

                    var id = state.GetItemStatID(def.Description.Id, SimGameState.GetTypeFromComponent(def.ComponentType));
                    if (state.companyStats.ContainsStatistic(id))
                    {
                        var count = state.companyStats.GetValue<int>(id);
                        if (count < minCount)
                        {
                            state.companyStats.ModifyStat("SimGameState", 0, id, StatCollection.StatOperation.Set, minCount);
                        }
                    }
                    else
                    {
                        state.companyStats.AddStatistic(id, minCount);
                    }
                }
            }

            var dataManager = UnityGameInstance.BattleTechGame.DataManager;
            AddApplicable(dataManager.ammoBoxDefs);
            AddApplicable(dataManager.heatSinkDefs);
            AddApplicable(dataManager.jumpJetDefs);
            AddApplicable(dataManager.upgradeDefs);
            AddApplicable(dataManager.weaponDefs);
        }
        catch (Exception e)
        {
            Control.Logger.Error.Log(e);
        }
    }
}