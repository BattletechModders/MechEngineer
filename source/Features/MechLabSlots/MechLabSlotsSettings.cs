using fastJSON;

namespace MechEngineer.Features.MechLabSlots
{
    internal class MechLabSlotsSettings : BaseSettings
    {
        [JsonIgnore]
        public bool MechLabGeneralWidgetEnabled => MechLabGeneralSlots > 0;
        public int MechLabArmTopPadding = 120;
        public int MechLabGeneralSlots = 3;
    }
}