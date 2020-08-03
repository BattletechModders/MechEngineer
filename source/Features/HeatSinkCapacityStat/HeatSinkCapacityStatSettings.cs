namespace MechEngineer.Features.HeatSinkCapacityStat
{
    public class HeatSinkCapacityStatSettings : ISettings
    {
        public bool Enabled { get; set; } = true;
        public string EnabledDescription => "Required by the engine feature to work. Disabled the mech.GetHeatSinkDissipation method.";
    }
}