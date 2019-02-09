using BattleTech;
using Localize;

namespace MechEngineer
{
    public static class MessageCenterUtils
    {
        public static void PublishMessage(this MechComponent component, Text message, FloatieMessage.MessageNature nature)
        {
            var actor = component.parent;
            if (actor == null)
            {
                return;
            }

            var stackMessage = new AddSequenceToStackMessage(new ShowActorInfoSequence(actor, message, nature, true));
            actor.Combat?.MessageCenter?.PublishMessage(stackMessage);
        }
    }
}
