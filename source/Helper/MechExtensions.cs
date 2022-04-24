using BattleTech;

namespace MechEngineer.Helper;

internal static class MechExtensions
{
    internal static void PublishFloatieMessage(this AbstractActor actor, string text, FloatieMessage.MessageNature nature = FloatieMessage.MessageNature.CriticalHit)
    {
        actor.Combat.MessageCenter.PublishMessage(new FloatieMessage(actor.GUID, actor.GUID, text, nature));
    }
}