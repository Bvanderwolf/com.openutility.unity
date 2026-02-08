using System;
using System.Collections;
using OpenUtility.Data;
using OpenUtility.DelayedExecution;
using UnityEngine;
using UnityEngine.Events;

public abstract class MenuPage : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float _fadeDuration = 0.25f;

    [Header("References")]
    [SerializeField]
    private Optional<CanvasGroup> _canvasGroup;
    
    [Header("Events")]
    [SerializeField] 
    private UnityEvent _onPageOpened;
    
    [SerializeField]
    private UnityEvent _onPageClosed;
    
    public void Open()
    {
        if (_canvasGroup.HasValue)
        {
            CanvasGroup canvasGroup = _canvasGroup.Value;
            if (!gameObject.activeInHierarchy)
            {
                canvasGroup.alpha = 0.0f;
                gameObject.SetActive(true);
            }

            StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, _fadeDuration, startDelay: _fadeDuration, onComplete: OnPageOpened));
        }
        else
        {
            gameObject.SetActive(true);
            OnPageOpened();
        }
    }

    public void Close()
    {
        if (_canvasGroup.HasValue)
        {
            StartCoroutine(FadeCanvasGroup(_canvasGroup.Value, 0.0f, _fadeDuration, onComplete: OnPageClosed));
        }
        else
        {
            gameObject.SetActive(false);
            OnPageClosed();
        }
    }

    private void OnPageOpened()
    {
        Debug.Log($"Opened page: {gameObject.name}");
        
        _onPageOpened?.Invoke();
    }

    private void OnPageClosed()
    {
        Debug.Log($"Closed page: {gameObject.name}");
        
        _onPageClosed?.Invoke();
    }
    
    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha, float duration, float startDelay = 0f, Action onComplete = null)
    {
        if (startDelay > 0f)
            yield return WaitFor.Seconds(startDelay);

        float startAlpha = canvasGroup.alpha;
        if (duration <= 0f)
        {
            canvasGroup.alpha = targetAlpha;
            onComplete?.Invoke();
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        onComplete?.Invoke();
    }
}
