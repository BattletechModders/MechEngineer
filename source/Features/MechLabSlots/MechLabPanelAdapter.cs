using BattleTech.UI;

namespace MechEngineer.Features.MechLabSlots
{
    internal class MechLabPanelAdapter : Adapter<MechLabPanel>
    {
        internal MechLabPanelAdapter(MechLabPanel instance) : base(instance)
        {
        }

        internal HBSDOTweenToggle btn_mechViewerButton => traverse.Field("btn_mechViewerButton").GetValue<HBSDOTweenToggle>();
        internal MechLabMechInfoWidget mechInfoWidget => traverse.Field("mechInfoWidget").GetValue<MechLabMechInfoWidget>();
    }
}