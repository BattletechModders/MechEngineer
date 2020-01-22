using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;
using fastJSON;
using Localize;
using Newtonsoft.Json;

namespace MechEngineer.Features.OverrideDescriptions {
  public enum BonusDescriptionType { Traits, CriticalEffects };
  public class BonusDescriptionContent {
    public string template { get; set; }
    public string elements { get; set; }
    public BonusDescriptionContent(string t, string e) { template = t; elements = e; }
  }
  public class ExtendedDescription {
    public List<BonusDescriptionContent> Traits { get; set; }
    public Dictionary<UnitType, List<BonusDescriptionContent>> CriticalEffects { get; set; }
    public string Description { get; private set; }
    public ExtendedDescription(string original) {
      CriticalEffects = new Dictionary<UnitType, List<BonusDescriptionContent>>();
      Traits = new List<BonusDescriptionContent>();
      Description = original;
    }
  }
  public static class DescriptionsHelper {
    private static Dictionary<string, ExtendedDescription> extendedDescriptions = new Dictionary<string, ExtendedDescription>();
    public static void AddBonusDescription(this DescriptionDef def, BonusDescriptionType type, UnitType ut, string template, string elements, string originalDescription) {
      //Control.mod.Logger.LogDebug("AddBonusDescription: "+def.Id+" type:"+type+" unit:"+ut+" template:"+template+" elements:"+elements+" original:"+originalDescription);
      if (extendedDescriptions.ContainsKey(def.Id) == false) { extendedDescriptions.Add(def.Id, new ExtendedDescription(originalDescription)); };
      ExtendedDescription descr = extendedDescriptions[def.Id];
      switch (type) {
        case BonusDescriptionType.Traits: descr.Traits.Add(new BonusDescriptionContent(template,elements)); break;
        case BonusDescriptionType.CriticalEffects:
          if (descr.CriticalEffects.ContainsKey(ut) == false) { descr.CriticalEffects.Add(ut, new List<BonusDescriptionContent>()); };
          descr.CriticalEffects[ut].Add(new BonusDescriptionContent(template, elements));
          break;
      }
    }
    public static string GetExtendedDescription(string defId) {
      //Control.mod.Logger.LogDebug("GetExtendedDescription: " + defId);
      if (extendedDescriptions.TryGetValue(defId,out ExtendedDescription descr)) {
        return JsonConvert.SerializeObject(descr);
      }
      return string.Empty;
    }
  }
  [CustomComponent("BonusDescriptions")]
  public class BonusDescriptions : SimpleCustomComponent, IAdjustTooltip, IAdjustInventoryElement, IAfterLoad {
    public string[] Bonuses { get; set; }

    public void AdjustTooltip(TooltipPrefab_Equipment tooltip, MechComponentDef componentDef) {
      var adapter = new TooltipPrefab_EquipmentAdapter(tooltip);
      //GUILogUtils.LogHierarchy(tooltip.transform);
      adapter.ShowBonuses = false;
    }

    public void AdjustInventoryElement(ListElementController_BASE_NotListView element) {
      var count = 0;
      foreach (var description in descriptions.Select(x => x.Long).Where(x => x != null).Take(2)) {
        if (count == 0) {
          element.ItemWidget.gearBonusText.text = description;
          count++;
        } else if (count == 1) {
          element.ItemWidget.gearBonusTextB.text = description;
        }
      }
    }

    public void OnLoaded(Dictionary<string, object> values) {
      if (Bonuses.Length < 1) {
        return;
      }

      foreach (var bonus in Bonuses) {
        var split = bonus.Split(new[] { ':' }, 2);
        var bonusKey = split[0].Trim();

        if (!OverrideDescriptionsFeature.Resources.TryGetValue(bonusKey, out var settings)) {
          Control.mod.Logger.LogError($"Could not find bonus description \"{bonusKey}\" used by {Def.Description.Id}");
          continue;
        }

        var args = split.Length >= 2 ? split[1].Split(',').Select(c => c.Trim()).ToArray() : new string[0];

        var description = new BonusDescription(settings, args);
        descriptions.Add(description);
      }

      {
        var adapter = new MechComponentDefAdapter(Def);
        var count = 0;
        foreach (var description in descriptions.Select(x => x.Short).Where(x => x != null).Take(2)) {
          if (count == 0) {
            adapter.BonusValueA = description;
            count++;
          } else if (count == 1) {
            adapter.BonusValueB = description;
          }
        }
      }

      AddBonusDescriptions(
          Def.Description,
          descriptions.Select(x => x.Full),
          OverrideDescriptionsFeature.settings.BonusDescriptionsElementTemplate,
          OverrideDescriptionsFeature.settings.BonusDescriptionsDescriptionTemplate,
          BonusDescriptionType.Traits,UnitType.UNDEFINED
      );
    }

    internal static void AddBonusDescriptions(
        DescriptionDef descriptionDef,
        IEnumerable<string> elements,
        string elementTemplate,
        string descriptionTemplate, BonusDescriptionType bonusType, UnitType unitType) {
      var adapter = new DescriptionDefAdapter(descriptionDef);
      var bonuses = string.Join("", elements.Where(x => x != null).Select(x => elementTemplate.Replace("{{element}}", x)).ToArray());
      descriptionDef.AddBonusDescription(bonusType, unitType, descriptionTemplate, bonuses, adapter.Details);
      adapter.Details = descriptionTemplate.Replace("{{elements}}", bonuses).Replace("{{originalDescription}}", adapter.Details);
    }

    [JsonIgnoreAttribute]
    private readonly List<BonusDescription> descriptions = new List<BonusDescription>();

    private class BonusDescription {
      internal BonusDescription(BonusDescriptionSettings settings, string[] values) {
        Short = Process(settings.Short, values);
        Long = Process(settings.Long, values);
        Full = Process(settings.Full, values);
      }

      public string Short { get; }
      public string Long { get; }
      public string Full { get; }

      private string Process(string format, string[] values) {
        try {
          return string.IsNullOrEmpty(format) ? null : new Text(format, values).ToString();
        } catch (Exception e) {
          var message = $"Can't process '{format}'";
          Control.mod.Logger.LogError(message, e);
          return message;
        }
      }
    }
  }
}
