using System.Collections.Generic;
using System.Linq;
using BattleTech.Save.SaveGameStructure;
using TMPro;
using UnityEngine;

namespace MechEngineer
{
    public static class GUILogUtils
    {
        public static void AdjustHeight(this Transform transform, int changedHeight)
        {
            var rect = transform.GetComponent<RectTransform>();
            if (rect == null)
            {
                return;
            }

            var vector = rect.sizeDelta;
            vector.y += changedHeight;
            rect.sizeDelta = vector;
        }

        public static RectTransform Rect(this Transform transform)
        {
            return transform.GetComponent<RectTransform>();
        }

        public static float Top(this Transform transform)
        {
            return transform.localPosition.y;
        }

        public static void SetTop(this Transform transform, float y)
        {
            var position = transform.localPosition;
            position.y = y;
            transform.localPosition = position;
        }

        public static float Bottom(this Transform transform)
        {
            var rect = transform.GetComponent<RectTransform>();
            return transform.localPosition.y - rect.sizeDelta.y;
        }

        public static void SetBottom(this Transform transform, float y)
        {
            var rect = transform.GetComponent<RectTransform>();
            var position = transform.localPosition;
            position.y = y + rect.sizeDelta.y;
            transform.localPosition = position;
        }

        public static float Right(this Transform transform)
        {
            var rect = transform.GetComponent<RectTransform>();
            return transform.localPosition.x + rect.sizeDelta.x;
        }

        public static void SetRight(this Transform transform, float x)
        {
            var rect = transform.GetComponent<RectTransform>();
            var position = transform.localPosition;
            position.x = x - rect.sizeDelta.x;
            transform.localPosition = position;
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

        public static void LogTransform(Transform transform)
        {
            Control.mod.Logger.LogDebug("");
            Control.mod.Logger.LogDebug("name=" + transform.name);
            Control.mod.Logger.LogDebug("parent=" + transform.parent);
            Control.mod.Logger.LogDebug("position=" + transform.position);
            Control.mod.Logger.LogDebug("localPosition=" + transform.localPosition);
            var rect = transform.GetComponent<RectTransform>();
            Control.mod.Logger.LogDebug("rect.anchoredPosition=" + rect.anchoredPosition);
            //Control.mod.Logger.LogDebug("rect.anchorMax=" + rect.anchorMax);
            //Control.mod.Logger.LogDebug("rect.anchorMin=" + rect.anchorMin);
            //Control.mod.Logger.LogDebug("rect.offsetMax=" + rect.offsetMax);
            //Control.mod.Logger.LogDebug("rect.offsetMin=" + rect.offsetMin);
            Control.mod.Logger.LogDebug("rect.pivot=" + rect.pivot);
            Control.mod.Logger.LogDebug("rect.rect=" + rect.rect);
        }

        public static void LogHierarchy(Transform transform, int level = 0)
        {
            var text = "";
            for (var i = 0; i < level; i++)
            {
                text += "  ";
            }

            var rectText = "";
            {
                var rect = transform.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rectText = " rect=" + rect.rect + " ancho=" + rect.anchoredPosition;
                }
            }

            var textText = "";
            {
                var textComponent = transform.GetComponent<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    //textText = "OverflowMode=" + textComponent.OverflowMode + " fontSize=" + textComponent.fontSize + " fontName=" + textComponent.font.name + " text=" + textComponent.text;
                    textText = " color=" + textComponent.color + " fontName=" + textComponent.font.name + " text=" + textComponent.text;
                }
            }

            Control.mod.Logger.LogDebug(text + transform.gameObject.name + " world=" + transform.position + " local=" + transform.localPosition + rectText + textText);
            level++;
            foreach (Transform current in transform)
            {
                LogHierarchy(current, level);
            }
        }
    }
}