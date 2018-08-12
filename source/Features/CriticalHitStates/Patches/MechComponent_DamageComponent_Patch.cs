using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechComponent), "DamageComponent")]
    public static class MechComponent_DamageComponent_Patch
    {
        public static bool Prefix(MechComponent __instance, CombatGameState ___combat, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            try
            {
                var messages = new List<MessageAddition>();
                void ClearMessageAndPublishAdditions()
                {
                    MechCheckForCritPatch.Message = null;
                    foreach (var message in messages)
                    {
                        var actor = __instance.parent;
                        var stackMessage = new AddSequenceToStackMessage(new ShowActorInfoSequence(actor, message.Text, message.Nature, true));
                        actor.Combat.MessageCenter.PublishMessage(stackMessage);
                    }
                }

                if (__instance.mechComponentRef.Def.IsIgnoreDamage())
                {
                    ClearMessageAndPublishAdditions();
                    return false;
                }

                if (!CirticalHitStatesHandler.Shared.ProcessWeaponHit(__instance, ___combat, hitInfo, damageLevel, applyEffects, messages))
                {
                    ClearMessageAndPublishAdditions();
                    return false;
                }

                if (!EngineCrits.ProcessWeaponHit(__instance, ___combat, hitInfo, damageLevel, applyEffects, messages))
                {
                    ClearMessageAndPublishAdditions();
                    return false;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }

            return true;
        }
    }

    public class MessageAddition
    {
        public string Text { get; set; }
        public FloatieMessage.MessageNature Nature { get; set; }
    }

    [HarmonyPatch(typeof(Mech), "CheckForCrit")]
    public static class MechCheckForCritPatch
    {
        public static MessageCenterMessage Message { get; set; }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions.MethodReplacer(
                AccessTools.Method(typeof(MessageCenter), "PublishMessage"),
                AccessTools.Method(typeof(MechCheckForCritPatch), "PublishMessage")
            );
        }

        public static void PublishMessage(this MessageCenter @this, MessageCenterMessage message)
        {
            Message = message;
        }

        public static void Postfix(Mech __instance)
        {
            try
            {
                if (Message != null)
                {
                    __instance.Combat.MessageCenter.PublishMessage(Message);
                    Message = null;
                }
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
        }
    }
}