using System;
using OpenUtility.Data;
using OpenUtility.DelayedExecution;
using UnityEngine;
using UnityEngine.UI;

namespace OpenUtility.UI
{
    public struct ScrollOptions
    {
        /// <summary>
        /// The normalized position to scroll to.
        /// </summary>
        public Vector2 position;

        /// <summary>
        /// The speed of the scroll animation. 
        /// </summary>
        public float speed;
        
        /// <summary>
        /// The easing function to use for the scroll animation.
        /// </summary>
        public EasingFunction easingFunction;
    }
    
    /// <summary>
    /// Provides utility methods for working with ScrollRect components.
    /// </summary>
    public static class ScrollViewUtility
    {
        /// <summary>
        /// Calculates the normalized scroll position required to focus on a specific point within the scroll view's content.
        /// </summary>
        public static Vector2 CalculateFocusedScrollPosition(this ScrollRect scrollView, Vector2 screenPoint)
        {
            Vector2 contentSize = scrollView.content.rect.size;
            Vector2 viewportSize = ((RectTransform)scrollView.content.parent).rect.size;
            Vector2 contentScale = scrollView.content.localScale;

            contentSize.Scale(contentScale);
            screenPoint.Scale(contentScale);

            Vector2 scrollPosition = scrollView.normalizedPosition;
            if (scrollView.horizontal && contentSize.x > viewportSize.x)
                scrollPosition.x = Mathf.Clamp01((screenPoint.x - viewportSize.x * 0.5f) / (contentSize.x - viewportSize.x));
            if (scrollView.vertical && contentSize.y > viewportSize.y)
                scrollPosition.y = Mathf.Clamp01((screenPoint.y - viewportSize.y * 0.5f) / (contentSize.y - viewportSize.y));

            return scrollPosition;
        }

        /// <summary>
        /// Calculates the normalized scroll position required to focus on a specific RectTransform within the scroll view's content.
        /// </summary>
        public static Vector2 CalculateFocusedScrollPosition(this ScrollRect scrollView, RectTransform transform)
        {
            Vector3 position = transform.TransformPoint(transform.rect.center);
            Vector2 itemCenterPoint = scrollView.content.InverseTransformPoint(position);

            Vector2 contentSizeOffset = scrollView.content.rect.size;
            contentSizeOffset.Scale(scrollView.content.pivot);

            return scrollView.CalculateFocusedScrollPosition(itemCenterPoint + contentSizeOffset);
        }

        /// <summary>
        /// Scrolls the ScrollRect to the specified normalized position using the provided options. If no options
        /// are provided, the scroll view will scroll towards the (0,0) position with default speed and no easing.
        /// </summary>
        public static void Scroll(this ScrollRect scrollRect, ScrollOptions options = default, Action action = null)
        {
            WaitFor.Scroll(scrollRect, options, action);
        }

        /// <summary>
        /// Scrolls the ScrollRect towards the specified screen point at the given speed, using an optional easing function.
        /// </summary>
        public static void ScrollTowardsPoint(this ScrollRect scrollView, Vector2 screenPoint, float speed, EasingFunction function = null, Action action = null)
        {
            Vector3 position = scrollView.CalculateFocusedScrollPosition(screenPoint);
            ScrollOptions options = new ScrollOptions
            {
                position = position,
                speed = speed,
                easingFunction = function
            };

            WaitFor.Scroll(scrollView, options, action);
        }

        /// <summary>
        /// Scrolls the ScrollRect towards the specified RectTransform at the given speed, using an optional easing function.
        /// </summary>
        public static void ScrollTowardsTransform(this ScrollRect scrollView, RectTransform transform, float speed, EasingFunction function = null, Action action = null)
        {
            Vector3 position = scrollView.CalculateFocusedScrollPosition(transform);
            ScrollOptions options = new ScrollOptions
            {
                position = position,
                speed = speed,
                easingFunction = function
            };

            WaitFor.Scroll(scrollView, options, action);
        }
    }
}
