using BattleTech;

namespace MechEngineer.Features.HeatSinkCapacityStat;

public class HeatSinkCapacityStatSettings : ISettings
{
    public bool Enabled { get; set; } = true;
    public string EnabledDescription => "Required by the engine feature to work. Disabled the mech.GetHeatSinkDissipation method.";

    public ComponentType[] ShutdownStatusEffectsExcludedComponentTypes = {ComponentType.HeatSink};
    public string ShutdownStatusEffectsExcludedComponentTypesDescription => "By default in CBT, heat sinks are still effective even when a mech is shut down. Heat Sink has to be part of the array or the engine will bug out once a mech is shut down.";

    public string[] ShutdownStatusEffectsExcludedComponentTags = {"ignore_shutdown"};
    public string ShutdownStatusEffectsExcludedComponentTagsDescription => "Don't shut down status effects for the components having the listed tags.";
}