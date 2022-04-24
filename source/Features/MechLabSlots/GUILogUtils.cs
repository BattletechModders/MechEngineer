using System.Collections.Generic;
using System.Linq;
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

    public static Transform GetChild(this Transform @this, string name, int index = 0)
    {
        return @this.GetChildren().Where(x => x.name == name).Skip(index).FirstOrDefault();
    }
}