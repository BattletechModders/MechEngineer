using System.Collections.Generic;
using CustomComponents;

namespace MechEngineer.Features.AutoFix
{
    internal class SensorsAHandler : IPreProcessor
    {
        internal static MELazy<SensorsAHandler> Lazy = new MELazy<SensorsAHandler>();
        internal static SensorsAHandler Shared => Lazy.Value;
        
        private readonly IdentityHelper identity;

        public SensorsAHandler()
        {
            identity = AutoFixerFeature.settings.SensorsACategorizer;
        }

        public void PreProcess(object target, Dictionary<string, object> values)
        {
            identity?.PreProcess(target, values);
        }
    }
}