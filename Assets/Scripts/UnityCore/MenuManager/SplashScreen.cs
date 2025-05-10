using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Handles the behavior of the splash screen, showing a sequence of splash screens with fade animations.
/// </summary>
public class SplashScreen : MonoBehaviour
{
    [Header("Splash Screen Settings")]
    [Tooltip("Canvas groups to display in sequence")]
    [SerializeField] private List<CanvasGroup> splashScreens = new List<CanvasGroup>();
    
    [Tooltip("How long each splash screen is fully visible")]
    [SerializeField] private float displayDuration = 2.0f;
    
    [Tooltip("Duration of fade in animation")]
    [SerializeField] private float fadeInDuration = 1.0f;
    
    [Tooltip("Duration of fade out animation")]
    [SerializeField] private float fadeOutDuration = 1.0f;

    [Header("Scene Transition")]
    [Tooltip("Name of the scene to load after all splash screens")]
    [SerializeField] private string nextSceneName = "MainMenu";

    // Tracks the currently active splash screen index
    private int currentSplashIndex = -1;
    private bool isTransitioning = false;

    /// <summary>
    /// Initialize and start the splash screen sequence.
    /// </summary>
    private void Start()
    {
        // Ensure all splash screens are initially invisible
        foreach (var screen in splashScreens)
        {
            if (screen != null)
            {
                screen.alpha = 0f;
                screen.gameObject.SetActive(true);
            }
        }

        // Start the splash screen sequence
        StartCoroutine(PlaySplashSequence());
    }

    /// <summary>
    /// Coroutine that plays through all splash screens in sequence.
    /// </summary>
    private IEnumerator PlaySplashSequence()
    {
        // Wait a frame to ensure everything is initialized
        yield return null;

        // Play each splash screen in sequence
        for (int i = 0; i < splashScreens.Count; i++)
        {
            currentSplashIndex = i;
            CanvasGroup currentScreen = splashScreens[i];
            
            if (currentScreen == null) continue;

            // Fade in
            LeanTween.alphaCanvas(currentScreen, 1f, fadeInDuration).setEase(LeanTweenType.easeInOutSine);
            yield return new WaitForSeconds(fadeInDuration);
            
            // Hold on screen
            yield return new WaitForSeconds(displayDuration);
            
            // Fade out
            LeanTween.alphaCanvas(currentScreen, 0f, fadeOutDuration).setEase(LeanTweenType.easeInOutSine);
            yield return new WaitForSeconds(fadeOutDuration);
        }

        // Transition to the next scene
        TransitionToNextScene();
    }

    /// <summary>
    /// Loads the next scene after all splash screens have been shown.
    /// </summary>
    private void TransitionToNextScene()
    {
        if (!isTransitioning)
        {
            isTransitioning = true;
            SaveSystem.LoadScene(nextSceneName);
        }
    }
}
