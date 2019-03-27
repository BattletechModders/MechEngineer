using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BattleTech;
using CustomComponents;
using Localize;

namespace MechEngineer
{
    public static class MessagesHandler
    {
        public static bool CompressFloatieMessages(FloatieMessage incoming, Queue<FloatieMessage> queue)
        {
            var incomingString = incoming.text.ToString();
            Control.mod.Logger.LogDebug("Floatie " + DateTime.Now.ToString("hh.mm.ss.ffffff") + " " + incomingString);
            foreach (var message in queue)
            {
                // quick preliminary check
                if (!message.text.ToString().StartsWith(incomingString))
                {
                    continue;
                }

                var times = 1;
                
                // parse and remove multiplier from the end of the message
                var lastPart = message.text.m_parts.Last();
                var m = MultiplierRegex.Match(lastPart.text);
                if (m.Success)
                {
                    times = int.Parse(m.Groups[1].Value);
                    var parts = message.text.m_parts;
                    parts.RemoveAt(parts.Count - 1);
                }

                // actual check if the message contents are the same, so we can combine messages
                if (message.text.ToString() != incomingString)
                {
                    if (m.Success)
                    {
                        message.text.m_parts.Add(lastPart); // undo multiplier removal
                    }
                    continue;
                }

                // adding new multiplier
                message.text.Append(" x " + ++times);
                return true;
            }

            return false;
        }
        private static readonly Regex MultiplierRegex = new Regex("^ x (\\d+)$", RegexOptions.Compiled);

        public static void PublishComponentState(MechComponent mechComponent)
        {
            if (mechComponent.DamageLevel == ComponentDamageLevel.Penalized)
            {
                var critMessage = new Text("{0} CRIT", mechComponent.UIName);
                if (mechComponent.componentDef.Is<CriticalEffects>(out var ce))
                {
                    if (ce.CritFloatieMessage != null)
                    {
                        if (ce.CritFloatieMessage == "")
                        {
                            return;
                        }
                        critMessage = new Text(ce.CritFloatieMessage);
                    }
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

                var destroyedMessage = new Text("{0} DESTROYED", mechComponent.UIName);
                if (mechComponent.componentDef.Is<CriticalEffects>(out var ce))
                {
                    if (ce.DestroyedFloatieMessage != null)
                    {
                        if (ce.DestroyedFloatieMessage == "")
                        {
                            return;
                        }
                        destroyedMessage = new Text(ce.DestroyedFloatieMessage);
                    }
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
}
