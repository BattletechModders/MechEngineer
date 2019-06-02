namespace MechEngineer.Features.MechLabSlots
{
    internal class MechLabSlotsFeature : Feature
    {
        internal static MechLabSlotsFeature Shared = new MechLabSlotsFeature();

        internal override bool Enabled => settings?.Enabled ?? false;

        internal static Settings settings => Control.settings.MechLabSlots;

        public class Settings
        {
            public bool Enabled = true;

            public bool MechLabGeneralWidgetEnabled => MechLabGeneralSlots > 0;
            public int MechLabArmTopPadding = 120;
            public int MechLabGeneralSlots = 3;
        }
    }
}
