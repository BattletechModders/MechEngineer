using System.Linq;
using TMPro;
using UnityEngine;

namespace MechEngineer.Helper;

internal static class Fonts
{
    internal static TMP_FontAsset MediumFont => GetFontByName("UnitedSansReg-Medium SDF");
    internal static TMP_FontAsset BlackFont => GetFontByName("UnitedSansReg-Black SDF");

    private static TMP_FontAsset GetFontByName(string name)
    {
        return Resources.FindObjectsOfTypeAll(typeof(TMP_FontAsset)).Cast<TMP_FontAsset>().First(f => f.name == name);
    }
}