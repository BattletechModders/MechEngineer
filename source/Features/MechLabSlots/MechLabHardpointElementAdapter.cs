using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using SVGImporter;

namespace MechEngineer.Features.MechLabSlots
{
    internal class MechLabHardpointElementAdapter : Adapter<MechLabHardpointElement>
    {
        internal MechLabHardpointElementAdapter(MechLabHardpointElement instance) : base(instance)
        {
        }

        internal LocalizableText hardpointText => traverse.Field("hardpointText").GetValue<LocalizableText>();

        internal SVGImage hardpointIcon => traverse.Field("hardpointIcon").GetValue<SVGImage>();
    }
}