using System.Linq;
using BattleTech;
using fastJSON;

namespace MechEngineer.Features.CustomCapacities;

public class CustomCapacitiesSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Enables some carry rules.";

    public float CarryLeftOverTopOff;
    public string CarryLeftOverTopOffDescription =>
        "Non CBT mechanism. The value is multiplied by the chassis tonnage. Carry in hand might have left overs, `top off` - `carry in hand usage` = `carry left over capacity`." +
        $" Modifiable via custom capacity collection '{CustomCapacitiesFeature.CarryLeftOverTopOffCollectionId}'.";

    public string CarryHandErrorOneFreeHand = "OVERWEIGHT: 'Mechs handheld carry weight requires one free hand.";

    public void Complete()
    {
        AllCapacities = Enumerable.Empty<CustomCapacity>()
            .Append(CarryWeight)
            .Append(HeatSink)
            .Append(HeatSinkEngineAdditional)
            .Concat(Capacities)
            .Where(x => x !=null)
            .Select(x => x!)
            .OrderBy(x => x.Description.Id)
            .ToArray();
    }

#nullable disable
    [JsonIgnore]
    public CustomCapacity[] AllCapacities;
#nullable restore

    public CustomCapacity? CarryWeight = new()
    {
        Description = new()
        {
            Id = CustomCapacitiesFeature.CarryInHandCollectionId,
            Name = "Carry Weight",
            Details = "Carry weight represents the total carry capacity of a 'Mech on top of the normal chassis weight internal capacity." +
                      " Each hand actuator allows to carry an equivalent of up to 5% chassis maximum tonnage." +
                      " If a melee weapon is too heavy for a single arm, it can be held two-handed by combining both hands carry capacities.",
            Icon = "UixSvgIcon_specialEquip_Melee"
        },
        Format = "{0:0.#} / {1:0.#}",
        ErrorOverweight = "OVERWEIGHT: This 'Mechs total carry weight exceeds maximum",
        HideIfNoUsageAndCapacity = true
    };

    public CustomCapacity? HeatSink = new()
    {
        Description = new()
        {
            Id = CustomCapacitiesFeature.HeatSinkCollectionId,
            Name = "Heat Sinks",
            Details = "A 'Mechs engine requires at least 10 heat sinks, which are usually provided by the engine itself." +
                      " Engine cores with a rating lower than 250 need supplementary heat sinks installed elsewhere on the 'Mech.",
            Icon = "uixSvgIcon_equipment_Heatsink"
        },
        Format = "{0:0} / {1:0}",
        ErrorOverweight = "HEAT SINKS: This 'Mech does not have the minimum amount of heat sinks",
        HideIfNoUsageAndCapacity = false
    };

    public CustomCapacity? HeatSinkEngineAdditional = new()
    {
        Description = new()
        {
            Id = CustomCapacitiesFeature.HeatSinkEngineAdditionalCollectionId,
            Name = "Additional Engine Heat Sinks",
            Details = "If a 'Mechs engine rating is at or above 275, additional heat sinks can directly be installed within the engine without using up criticals on the 'Mech.",
            Icon = "uixSvgIcon_equipment_Heatsink"
        },
        Format = "E {0:0} / {1:0}",
        ErrorOverweight = "HEAT SINKS: This 'Mech has exceeded the amount of heat sinks the engine can be fitted with",
        HideIfNoUsageAndCapacity = true
    };

    public CustomCapacity[] Capacities = { };

    public class CustomCapacity
    {
        public BaseDescriptionDef Description { get; set; } = null!;

        public string Format { get; set; } = null!;
        public string ErrorOverweight { get; set; } = null!;

        public bool HideIfNoUsageAndCapacity { get; set; }
    }
}
