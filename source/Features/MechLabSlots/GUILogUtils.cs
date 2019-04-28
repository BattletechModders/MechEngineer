using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MechEngineer.Features.MechLabSlots
{
    public static class GUILogUtils
    {
        public static void EnableLayout(this GameObject gameObject)
        {
            {
                var component = gameObject.GetComponent<LayoutElement>();
                if (component == null)
                {
                    component = gameObject.AddComponent<LayoutElement>();
                }
                component.ignoreLayout = false;
                component.enabled = true;
            }

            {
                var component = gameObject.GetComponent<HorizontalLayoutGroup>();
                if (component != null)
                {
                    component.childForceExpandHeight = false;
                    component.childAlignment = TextAnchor.UpperCenter;
                    component.enabled = true;
                }
            }

            {
                var component = gameObject.GetComponent<VerticalLayoutGroup>();
                if (component != null)
                {
                    component.enabled = true;
                }
            }

            {
                var component = gameObject.GetComponent<GridLayoutGroup>();
                if (component != null)
                {
                    component.enabled = true;
                }
            }

            {
                var component = gameObject.GetComponent<ContentSizeFitter>();
                if (component != null)
                {
                    component.enabled = true;
                }
            }
        }

        public static RectTransform Rect(this Transform transform)
        {
            return transform.GetComponent<RectTransform>();
        }

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
}