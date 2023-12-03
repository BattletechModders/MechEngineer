using UnityEngine;

namespace MechEngineer.Helper;

internal static class SharedGameObjects
{
    internal static Transform ContainerTransform { get; }
    static SharedGameObjects()
    {
        if (ContainerTransform == null)
        {
            var containerGo = new GameObject(nameof(MechEngineer));
            containerGo.SetActive(false);
            Object.DontDestroyOnLoad(containerGo);
            ContainerTransform = containerGo.transform;
        }
    }
}