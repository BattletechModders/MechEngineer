using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using SVGImporter;

namespace MechEngineer.Features.DynamicSlots.Patches;

[HarmonyPatch(typeof(MechLabItemSlotElement), nameof(MechLabItemSlotElement.SetIconAndText))]
public static class MechLabItemSlotElement_SetIconAndText_Patch
{
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(
        MechComponentRef ___componentRef,
        SVGImage ___icon,
        LocalizableText ___nameText,
        LocalizableText ___bonusTextA,
        LocalizableText ___bonusTextB
        )
    {
        // support no icon
        ___icon.gameObject.SetActive(!string.IsNullOrEmpty(___componentRef.Def.Description.Icon));

        // support no name
        ___nameText.gameObject.SetActive(!string.IsNullOrEmpty(___componentRef.Def.Description.UIName));

        // support null besides empty string
        ___bonusTextA.gameObject.SetActive(!string.IsNullOrEmpty(___componentRef.Def.BonusValueA));
        ___bonusTextB.gameObject.SetActive(!string.IsNullOrEmpty(___componentRef.Def.BonusValueB));
    }
}
