using BattleTech.UI;

namespace MechEngineer.Helper
{
    internal class MechLabMechInfoWidgetAdapter : Adapter<MechLabMechInfoWidget>
    {
        internal MechLabMechInfoWidgetAdapter(MechLabMechInfoWidget instance) : base(instance)
        {
        }
        internal MechLabHardpointElement[] hardpoints => traverse.Field("hardpoints").GetValue<MechLabHardpointElement[]>();
    }
}