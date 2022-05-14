using System.Collections.Generic;
using UnityEngine;

namespace MechEngineer.Features.MechLabSlots;

public static class GUILogUtils
{
    public static IEnumerable<Transform> GetChildren(this Transform @this)
    {
        foreach (Transform current in @this)
        {
            yield return current;
        }
    }
}
