﻿namespace MechEngineer.Features.ShutdownInjuryProtection
{
    public class ShutdownInjuryProtectionSettings : ISettings
    {
        public bool Enabled { get; set; } = true;
        public string EnabledDescription => "Pilots receive injury when mech receives overheat damage or even has to shut down.";

        public bool HeatDamageInjuryEnabled = true;
        public bool ShutdownInjuryEnabled = true;
    }
}