using BattleTech;

namespace MechEngineer
{
    internal static class MechExtensions
    {
        internal static void PublishFloatieMessage(this Mech mech, string text, FloatieMessage.MessageNature nature = FloatieMessage.MessageNature.CriticalHit)
        {
            mech.Combat.MessageCenter.PublishMessage(new FloatieMessage(mech.GUID, mech.GUID, text, nature));
        }
    }
}