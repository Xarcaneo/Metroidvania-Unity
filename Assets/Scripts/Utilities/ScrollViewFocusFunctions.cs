using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Provides extension methods for ScrollRect to focus on specific points or items within the scroll view.
/// Includes functions for both immediate positioning and smooth scrolling animations.
/// </summary>
public static class ScrollViewFocusFunctions
{
    /// <summary>
    /// Calculates the scroll position required to focus on a specific point within the content.
    /// </summary>
    /// <param name="scrollView">The ScrollRect to adjust.</param>
    /// <param name="focusPoint">The point to focus on, in content coordinates.</param>
    /// <returns>A Vector2 representing the normalized scroll position to focus on the specified point.</returns>
    public static Vector2 CalculateFocusedScrollPosition(this ScrollRect scrollView, Vector2 focusPoint)
    {
        Vector2 contentSize = scrollView.content.rect.size;
        Vector2 viewportSize = ((RectTransform)scrollView.content.parent).rect.size;
        Vector2 contentScale = scrollView.content.localScale;

        contentSize.Scale(contentScale);
        focusPoint.Scale(contentScale);

        Vector2 scrollPosition = scrollView.normalizedPosition;
        if (scrollView.horizontal && contentSize.x > viewportSize.x)
        {
            scrollPosition.x = Mathf.Clamp01((focusPoint.x - viewportSize.x * 0.5f) / (contentSize.x - viewportSize.x));
        }
        if (scrollView.vertical && contentSize.y > viewportSize.y)
        {
            scrollPosition.y = Mathf.Clamp01((focusPoint.y - viewportSize.y * 0.5f) / (contentSize.y - viewportSize.y));
        }

        return scrollPosition;
    }

    /// <summary>
    /// Calculates the scroll position required to focus on a specific item within the content.
    /// </summary>
    /// <param name="scrollView">The ScrollRect to adjust.</param>
    /// <param name="item">The RectTransform of the item to focus on.</param>
    /// <returns>A Vector2 representing the normalized scroll position to focus on the specified item.</returns>
    public static Vector2 CalculateFocusedScrollPosition(this ScrollRect scrollView, RectTransform item)
    {
        Vector2 itemCenterPoint = scrollView.content.InverseTransformPoint(item.transform.TransformPoint(item.rect.center));

        Vector2 contentSizeOffset = scrollView.content.rect.size;
        contentSizeOffset.Scale(scrollView.content.pivot);

        return scrollView.CalculateFocusedScrollPosition(itemCenterPoint + contentSizeOffset);
    }

    /// <summary>
    /// Immediately sets the scroll position to focus on a specific point within the content.
    /// </summary>
    /// <param name="scrollView">The ScrollRect to adjust.</param>
    /// <param name="focusPoint">The point to focus on, in content coordinates.</param>
    public static void FocusAtPoint(this ScrollRect scrollView, Vector2 focusPoint)
    {
        scrollView.normalizedPosition = scrollView.CalculateFocusedScrollPosition(focusPoint);
    }

    /// <summary>
    /// Immediately sets the scroll position to focus on a specific item within the content.
    /// </summary>
    /// <param name="scrollView">The ScrollRect to adjust.</param>
    /// <param name="item">The RectTransform of the item to focus on.</param>
    public static void FocusOnItem(this ScrollRect scrollView, RectTransform item)
    {
        scrollView.normalizedPosition = scrollView.CalculateFocusedScrollPosition(item);
    }

    /// <summary>
    /// Smoothly scrolls to a target normalized position over time using an easing curve.
    /// </summary>
    /// <param name="scrollView">The ScrollRect to adjust.</param>
    /// <param name="targetNormalizedPos">The target normalized position to scroll to.</param>
    /// <param name="speed">The speed of the scrolling animation.</param>
    /// <returns>An IEnumerator for coroutine usage.</returns>
    private static IEnumerator LerpToScrollPositionCoroutine(this ScrollRect scrollView, Vector2 targetNormalizedPos, float speed)
    {
        Vector2 initialNormalizedPos = scrollView.normalizedPosition;

        float t = 0f;
        while (t < 1f)
        {
            scrollView.normalizedPosition = Vector2.LerpUnclamped(initialNormalizedPos, targetNormalizedPos, 1f - (1f - t) * (1f - t));
            yield return null;
            t += speed * Time.unscaledDeltaTime;
        }

        scrollView.normalizedPosition = targetNormalizedPos;
    }

    /// <summary>
    /// Smoothly scrolls to focus on a specific point within the content.
    /// </summary>
    /// <param name="scrollView">The ScrollRect to adjust.</param>
    /// <param name="focusPoint">The point to focus on, in content coordinates.</param>
    /// <param name="speed">The speed of the scrolling animation.</param>
    /// <returns>An IEnumerator for coroutine usage.</returns>
    public static IEnumerator FocusAtPointCoroutine(this ScrollRect scrollView, Vector2 focusPoint, float speed)
    {
        yield return scrollView.LerpToScrollPositionCoroutine(scrollView.CalculateFocusedScrollPosition(focusPoint), speed);
    }

    /// <summary>
    /// Smoothly scrolls to focus on a specific item within the content.
    /// </summary>
    /// <param name="scrollView">The ScrollRect to adjust.</param>
    /// <param name="item">The RectTransform of the item to focus on.</param>
    /// <param name="speed">The speed of the scrolling animation.</param>
    /// <returns>An IEnumerator for coroutine usage.</returns>
    public static IEnumerator FocusOnItemCoroutine(this ScrollRect scrollView, RectTransform item, float speed)
    {
        yield return scrollView.LerpToScrollPositionCoroutine(scrollView.CalculateFocusedScrollPosition(item), speed);
    }
}
