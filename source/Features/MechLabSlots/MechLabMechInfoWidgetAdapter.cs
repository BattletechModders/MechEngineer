using BattleTech.UI;

namespace MechEngineer.Features.MechLabSlots
{
    internal class MechLabMechInfoWidgetAdapter : Adapter<MechLabMechInfoWidget>
    {
        internal MechLabMechInfoWidgetAdapter(MechLabMechInfoWidget instance) : base(instance)
        {
        }
        internal MechLabHardpointElement[] hardpoints => traverse.Field("hardpoints").GetValue<MechLabHardpointElement[]>();
    }
}