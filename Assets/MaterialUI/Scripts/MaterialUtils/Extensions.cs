//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// Static class with <see cref="Func{T}"/> extension methods.
    /// </summary>
    public static class FuncExtension
    {
        /// <summary>
        /// Invokes a Func if not null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">The function to invoke.</param>
        /// <returns></returns>
        public static T InvokeIfNotNull<T>(this Func<T> func)
        {
            if (func != null)
            {
                return func();
            }

            return default(T);
        }
    }

    /// <summary>
    /// Static class with <see cref="Transform"/> extension methods.
    /// </summary>
    public static class TransformExtension
    {
        /// <summary>
        /// Sets the parent and scale of a Transform.
        /// </summary>
        /// <param name="transform">The transform to modify.</param>
        /// <param name="parent">The new parent to set.</param>
        /// <param name="localScale">The local scale to set.</param>
        /// <param name="worldPositionStays">if set to <c>true</c> [world position stays].</param>
        public static void SetParentAndScale(this Transform transform, Transform parent, Vector3 localScale, bool worldPositionStays = false)
        {
            transform.SetParent(parent, worldPositionStays);
            transform.localScale = localScale;
        }

        /// <summary>
        /// Gets the root canvas from a transform.
        /// </summary>
        /// <param name="transform">The transform to use.</param>
        /// <returns>Returns root canvas if one found, otherwise returns null.</returns>
        public static Canvas GetRootCanvas(this Transform transform)
        {
            Canvas[] canvases = transform.GetComponentsInParent<Canvas>();

            if (canvases == null || canvases.Length == 0) return null;

            return canvases.Last();
        }
    }

    /// <summary>
    /// Static class with <see cref="Canvas"/> extension methods.
    /// </summary>
    public static class CanvasExtension
    {
        /// <summary>
        /// Copies a Canvas to another GameObject.
        /// </summary>
        /// <param name="canvas">The canvas to copy.</param>
        /// <param name="gameObjectToAddTo">The game object to add the new Canvas to.</param>
        /// <returns>The new Canvas instance.</returns>
        public static Canvas Copy(this Canvas canvas, GameObject gameObjectToAddTo)
        {
            Canvas dupCanvas = gameObjectToAddTo.GetAddComponent<Canvas>();

            RectTransform mainCanvasRectTransform = canvas.GetComponent<RectTransform>();
            RectTransform dropdownCanvasRectTransform = dupCanvas.GetComponent<RectTransform>();

            dropdownCanvasRectTransform.position = mainCanvasRectTransform.position;
            dropdownCanvasRectTransform.sizeDelta = mainCanvasRectTransform.sizeDelta;
            dropdownCanvasRectTransform.anchorMin = mainCanvasRectTransform.anchorMin;
            dropdownCanvasRectTransform.anchorMax = mainCanvasRectTransform.anchorMax;
            dropdownCanvasRectTransform.pivot = mainCanvasRectTransform.pivot;
            dropdownCanvasRectTransform.rotation = mainCanvasRectTransform.rotation;
            dropdownCanvasRectTransform.localScale = mainCanvasRectTransform.localScale;

            dupCanvas.gameObject.GetAddComponent<GraphicRaycaster>();
            CanvasScaler mainScaler = canvas.GetComponent<CanvasScaler>();
            if (mainScaler != null)
            {
                CanvasScaler scaler = dupCanvas.gameObject.GetAddComponent<CanvasScaler>();
                scaler.uiScaleMode = mainScaler.uiScaleMode;
                scaler.referenceResolution = mainScaler.referenceResolution;
                scaler.screenMatchMode = mainScaler.screenMatchMode;
                scaler.matchWidthOrHeight = mainScaler.matchWidthOrHeight;
                scaler.referencePixelsPerUnit = mainScaler.referencePixelsPerUnit;
            }
            dupCanvas.gameObject.GetAddComponent<MaterialUIScaler>();
            dupCanvas.renderMode = canvas.renderMode;

            return dupCanvas;
        }
        /// <summary>
        /// Copies the settings to other canvas.
        /// </summary>
        /// <param name="canvas">The canvas to copy from.</param>
        /// <param name="otherCanvas">The canvas to copy to.</param>
        public static void CopySettingsToOtherCanvas(this Canvas canvas, Canvas otherCanvas)
        {
            RectTransform mainCanvasRectTransform = canvas.GetComponent<RectTransform>();
            RectTransform dropdownCanvasRectTransform = otherCanvas.GetComponent<RectTransform>();

            dropdownCanvasRectTransform.position = mainCanvasRectTransform.position;
            dropdownCanvasRectTransform.sizeDelta = mainCanvasRectTransform.sizeDelta;
            dropdownCanvasRectTransform.anchorMin = mainCanvasRectTransform.anchorMin;
            dropdownCanvasRectTransform.anchorMax = mainCanvasRectTransform.anchorMax;
            dropdownCanvasRectTransform.pivot = mainCanvasRectTransform.pivot;
            dropdownCanvasRectTransform.rotation = mainCanvasRectTransform.rotation;
            dropdownCanvasRectTransform.localScale = mainCanvasRectTransform.localScale;

            otherCanvas.gameObject.GetAddComponent<GraphicRaycaster>();
            CanvasScaler mainScaler = canvas.GetComponent<CanvasScaler>();
            if (mainScaler != null)
            {
                CanvasScaler scaler = otherCanvas.gameObject.GetAddComponent<CanvasScaler>();
                scaler.uiScaleMode = mainScaler.uiScaleMode;
                scaler.referenceResolution = mainScaler.referenceResolution;
                scaler.screenMatchMode = mainScaler.screenMatchMode;
                scaler.matchWidthOrHeight = mainScaler.matchWidthOrHeight;
                scaler.referencePixelsPerUnit = mainScaler.referencePixelsPerUnit;
            }
            otherCanvas.gameObject.GetAddComponent<MaterialUIScaler>();
            otherCanvas.renderMode = canvas.renderMode;
        }
    }

    /// <summary>
    /// Static class with <see cref="Action"/> extension methods.
    /// </summary>
    public static class ActionExtension
    {
        /// <summary>
        /// Invokes an <see cref="Action"/> if not null.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        public static void InvokeIfNotNull(this Action action)
        {
            if (action != null)
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// Invokes an <see cref="Action{T}"/> if not null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <param name="parameter">The parameter.</param>
        public static void InvokeIfNotNull<T>(this Action<T> action, T parameter)
        {
            if (action != null)
            {
                action.Invoke(parameter);
            }
        }
    }

    /// <summary>
    /// Static class with <see cref="UnityEvent"/> extension methods.
    /// </summary>
    public static class UnityEventExtension
    {
        /// <summary>
        /// Invokes a <see cref="UnityEvent"/> if not null.
        /// </summary>
        /// <param name="unityEvent">The UnityEvent to invoke.</param>
        public static void InvokeIfNotNull(this UnityEvent unityEvent)
        {
            if (unityEvent != null)
            {
                unityEvent.Invoke();
            }
        }

        /// <summary>
        /// Invokes a <see cref="UnityEvent{T}"/> if not null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unityEvent">The UnityEvent to invoke.</param>
        /// <param name="parameter">The argument used in the invocation.</param>
        public static void InvokeIfNotNull<T>(this UnityEvent<T> unityEvent, T parameter)
        {
            if (unityEvent != null)
            {
                unityEvent.Invoke(parameter);
            }
        }
    }

    /// <summary>
    /// Static class with <see cref="GameObject"/> extension methods.
    /// </summary>
    public static class GameObjectExtension
    {
        /// <summary>
        /// Gets a Component on a GameObject if it exists, otherwise add one.
        /// </summary>
        /// <typeparam name="T">The type of Component to add.</typeparam>
        /// <param name="gameObject">The game object to check/add to.</param>
        /// <returns>The Component instance.</returns>
        public static T GetAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.GetComponent<T>() != null)
            {
                return gameObject.GetComponent<T>();
            }
            else
            {
                return gameObject.AddComponent<T>();
            }

        }

        /// <summary>
        /// Gets a child Component by name and type.
        /// </summary>
        /// <typeparam name="T">The type of Component.</typeparam>
        /// <param name="gameObject">The game object.</param>
        /// <param name="name">The name to search.</param>
        /// <returns>The Component found, otherwise null.</returns>
        public static T GetChildByName<T>(this GameObject gameObject, string name) where T : Component
        {
            T[] items = gameObject.GetComponentsInChildren<T>(true);

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].name == name)
                {
                    return items[i];
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Static class with <see cref="MonoBehaviour"/> extension methods.
    /// </summary>
    public static class MonoBehaviourExtension
    {
        /// <summary>
        /// Gets a Component on a GameObject if it exists, otherwise add one.
        /// </summary>
        /// <typeparam name="T">The type of Component to add.</typeparam>
        /// <returns>The Component instance.</returns>
        public static T GetAddComponent<T>(this MonoBehaviour monoBehaviour) where T : Component
        {
            if (monoBehaviour.GetComponent<T>() != null)
            {
                return monoBehaviour.GetComponent<T>();
            }

            return monoBehaviour.gameObject.AddComponent<T>();
        }

        /// <summary>
        /// Gets a child Component by name and type.
        /// </summary>
        /// <typeparam name="T">The type of Component.</typeparam>
        /// <param name="monoBehaviour">The MonoBehaviour.</param>
        /// <param name="name">The name to search.</param>
        /// <returns>The Component found, otherwise null.</returns>
        public static T GetChildByName<T>(this MonoBehaviour monoBehaviour, string name) where T : Component
        {
            return monoBehaviour.gameObject.GetChildByName<T>(name);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ComponentExtension
    {
        /// <summary>
        /// Gets the name of the child by.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component">The component.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static T GetChildByName<T>(this Component component, string name) where T : Component
        {
            return component.gameObject.GetChildByName<T>(name);
        }
    }

    /// <summary>
    /// Static class with <see cref="Color"/> extension methods.
    /// </summary>
    public static class ColorExtension
    {
        /// <summary>
        /// Gets a color with a specified alpha level.
        /// </summary>
        /// <param name="color">The color to get.</param>
        /// <param name="alpha">The desired alpha level.</param>
        /// <returns>A Color with 'rgb' values from color argument, and 'a' value from alpha argument.</returns>
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        /// <summary>
        /// Uses <see cref="Mathf.Approximately"/> on the color level values of two colors to compare them.
        /// </summary>
        /// <param name="thisColor">The first Color to compare.</param>
        /// <param name="otherColor">The second Color to compare.</param>
        /// <param name="compareAlpha">Should the alpha levels also be compared?</param>
        /// <returns>True if the first Color is approximately the second Color, otherwise false.</returns>
        public static bool Approximately(this Color thisColor, Color otherColor, bool compareAlpha = false)
        {
            if (!Mathf.Approximately(thisColor.r, otherColor.r)) return false;
            if (!Mathf.Approximately(thisColor.g, otherColor.g)) return false;
            if (!Mathf.Approximately(thisColor.b, otherColor.b)) return false;
            if (!compareAlpha) return true;
            return Mathf.Approximately(thisColor.a, otherColor.a);
        }
    }

    /// <summary>
    /// Static class with <see cref="RectTransform"/> extension methods.
    /// </summary>
    public static class RectTransformExtension
    {
        /// <summary>Sometimes sizeDelta works, sometimes rect works, sometimes neither work and you need to get the layout properties.
        ///	This method provides a simple way to get the size of a RectTransform, no matter what's driving it or what the anchor values are.
        /// </summary>
        /// <param name="rectTransform">The rect transform to check.</param>
        /// <returns>The proper size of the RectTransform.</returns>
        public static Vector2 GetProperSize(this RectTransform rectTransform) //, bool attemptToRefreshLayout = false)
        {
            Vector2 size = new Vector2(rectTransform.rect.width, rectTransform.rect.height);

            if (size.x == 0 && size.y == 0)
            {
                LayoutElement layoutElement = rectTransform.GetComponent<LayoutElement>();

                if (layoutElement != null)
                {
                    size.x = layoutElement.preferredWidth;
                    size.y = layoutElement.preferredHeight;
                }
            }
            if (size.x == 0 && size.y == 0)
            {
                LayoutGroup layoutGroup = rectTransform.GetComponent<LayoutGroup>();

                if (layoutGroup != null)
                {
                    size.x = layoutGroup.preferredWidth;
                    size.y = layoutGroup.preferredHeight;
                }
            }

            if (size.x == 0 && size.y == 0)
            {
                size.x = LayoutUtility.GetPreferredWidth(rectTransform);
                size.y = LayoutUtility.GetPreferredHeight(rectTransform);
            }

            return size;
        }

        /// <summary>
        /// Gets the position regardless of pivot.
        /// </summary>
        /// <param name="rectTransform">The rect transform.</param>
        /// <returns>The position in world space.</returns>
        public static Vector3 GetPositionRegardlessOfPivot(this RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            return (corners[0] + corners[2]) / 2;
        }

        /// <summary>
        /// Gets the local position regardless of pivot.
        /// </summary>
        /// <param name="rectTransform">The rect transform.</param>
        /// <returns>The position in local space.</returns>
        public static Vector3 GetLocalPositionRegardlessOfPivot(this RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetLocalCorners(corners);
            return (corners[0] + corners[2]) / 2;
        }

        /// <summary>
        /// Sets the x value of a RectTransform's anchor.
        /// </summary>
        /// <param name="rectTransform">The rect transform.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        public static void SetAnchorX(this RectTransform rectTransform, float min, float max)
        {
            rectTransform.anchorMin = new Vector2(min, rectTransform.anchorMin.y);
            rectTransform.anchorMax = new Vector2(max, rectTransform.anchorMax.y);
        }

        /// <summary>
        /// Sets the y value of a RectTransform's anchor
        /// </summary>
        /// <param name="rectTransform">The rect transform.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        public static void SetAnchorY(this RectTransform rectTransform, float min, float max)
        {
            rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, min);
            rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, max);
        }
    }

    /// <summary>
    /// Static class with <see cref="Graphic"/> extension methods.
    /// </summary>
    public static class GraphicExtension
    {
        /// <summary>
        /// Determines whether a Graphic is of type Image or VectorImage.
        /// </summary>
        /// <param name="graphic">The graphic to check.</param>
        /// <returns>True if the Graphic is of type Image or VectorImage, otherwise false.</returns>
        public static bool IsSpriteOrVectorImage(this Graphic graphic)
        {
            return (graphic is Image || graphic is VectorImage);
        }

        /// <summary>
        /// Sets the image of a Graphic (must be of type Image).
        /// </summary>
        /// <param name="graphic">The graphic to modify.</param>
        /// <param name="sprite">The sprite to set.</param>
        public static void SetImage(this Graphic graphic, Sprite sprite)
        {
            Image imageToSet = graphic as Image;

            if (imageToSet != null)
            {
                imageToSet.sprite = sprite;
            }
        }
        /// <summary>
        /// Sets the image of a Graphic (must be of type VectorImage).
        /// </summary>
        /// <param name="graphic">The graphic to modify.</param>
        /// <param name="vectorImageData">The vector image data to set.</param>
        public static void SetImage(this Graphic graphic, VectorImageData vectorImageData)
        {
            VectorImage imageToSet = graphic as VectorImage;

            if (imageToSet != null)
            {
                imageToSet.vectorImageData = vectorImageData;
            }
        }
        /// <summary>
        /// Sets the image of a Graphic (must be of type Image if imageData has type Sprite, or VectorImage if imageData has type VectorImageData).
        /// </summary>
        /// <param name="graphic">The graphic to modify.</param>
        /// <param name="imageData">The image data to set.</param>
        public static void SetImage(this Graphic graphic, ImageData imageData)
        {
            VectorImage vectorImage = graphic as VectorImage;

            if (vectorImage != null && imageData != null)
            {
                if (imageData.imageDataType == ImageDataType.VectorImage)
                {
                    vectorImage.vectorImageData = imageData.vectorImageData;
                }
                return;
            }

            Image spriteImage = graphic as Image;

            if (spriteImage != null && imageData != null)
            {
                if (imageData.imageDataType == ImageDataType.Sprite)
                {
                    spriteImage.sprite = imageData.sprite;
                }
            }
        }

        /// <summary>
        /// Gets the sprite image.
        /// </summary>
        /// <param name="graphic">The graphic to check.</param>
        /// <returns>The Sprite of the Graphic, if applicable and one exists.</returns>
        public static Sprite GetSpriteImage(this Graphic graphic)
        {
            Image imageToGet = graphic as Image;

            if (imageToGet != null)
            {
                return imageToGet.sprite;
            }

            return null;
        }

        /// <summary>
        /// Gets the vector image.
        /// </summary>
        /// <param name="graphic">The graphic to check.</param>
        /// <returns>The VectorImageData of the Graphic, if applicable and one exists.</returns>
        public static VectorImageData GetVectorImage(this Graphic graphic)
        {
            VectorImage imageToGet = graphic as VectorImage;

            if (imageToGet != null)
            {
                return imageToGet.vectorImageData;
            }

            return null;
        }

        /// <summary>
        /// Gets the image data.
        /// </summary>
        /// <param name="graphic">The graphic to check.</param>
        /// <returns>The ImageData, if applicable and one exists.</returns>
        public static ImageData GetImageData(this Graphic graphic)
        {
            Sprite sprite = graphic.GetSpriteImage();

            if (sprite != null)
            {
                return new ImageData(sprite);
            }

            VectorImageData vectorImageData = graphic.GetVectorImage();

            if (vectorImageData != null)
            {
                return new ImageData(vectorImageData);
            }

            return null;
        }
    }
}