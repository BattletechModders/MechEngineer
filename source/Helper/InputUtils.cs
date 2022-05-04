using UnityEngine;

namespace MechEngineer.Helper;

internal static class InputUtils
{
    internal static bool ShiftModifierPressed => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    internal static bool ControlModifierPressed => Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
    internal static bool AltModifierPressed => Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
}
