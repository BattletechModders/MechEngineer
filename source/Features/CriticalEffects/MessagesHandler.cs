using BattleTech;
using Localize;

namespace MechEngineer.Features.CriticalEffects;

public static class MessagesHandler
{
    public static void PublishComponentState(MechComponent mechComponent)
    {
        var ce = mechComponent.Criticals().Effects;
        if (mechComponent.DamageLevel == ComponentDamageLevel.Penalized)
        {
            var critMessage = new Text(CriticalEffectsFeature.settings.CritFloatieMessage, mechComponent.UIName);
            if (ce?.CritFloatieMessage != null)
            {
                if (ce.CritFloatieMessage == "")
                {
                    return;
                }
                critMessage = new Text(ce.CritFloatieMessage);
            }
            mechComponent.PublishMessage(
                critMessage,
                FloatieMessage.MessageNature.CriticalHit
            );
        }
        else if (mechComponent.DamageLevel == ComponentDamageLevel.Destroyed)
        {
            // dont show destroyed if a mech is known to be incapacitated
            var actor = mechComponent.parent;
            if (actor.IsDead || actor.IsFlaggedForDeath)
            {
                return;
            }

            // dont show destroyed if a whole section was destroyed, counters spam
            //if (actor is Mech mech)
            //{
            //    var location = mechComponent.mechComponentRef.MountedLocation;
            //    var mechLocationDestroyed = mech.IsLocationDestroyed(location);
            //    if (mechLocationDestroyed)
            //    {
            //        return;
            //    }
            //}

            var destroyedMessage = new Text(CriticalEffectsFeature.settings.DestroyedFloatieMessage, mechComponent.UIName);
            if (ce?.DestroyedFloatieMessage != null)
            {
                if (ce.DestroyedFloatieMessage == "")
                {
                    return;
                }
                destroyedMessage = new Text(ce.DestroyedFloatieMessage);
            }
            mechComponent.PublishMessage(
                destroyedMessage,
                FloatieMessage.MessageNature.ComponentDestroyed
            );
        }
    }

    private static void PublishMessage(this MechComponent component, Text message, FloatieMessage.MessageNature nature)
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