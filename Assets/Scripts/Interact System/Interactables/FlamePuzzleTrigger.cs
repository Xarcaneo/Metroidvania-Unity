using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlamePuzzleTrigger : Interactable
{
    [SerializeField] int m_puzzleID = 0;
    [SerializeField] int m_triggerID = 1;

    private void OnEnable()
    {
        InputManager.Instance.OnMenuReturn += OnReturnInput;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        InputManager.Instance.OnMenuReturn -= OnReturnInput;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Flame Puzzle" && mode == LoadSceneMode.Additive && isInteracting)
        {
            // The "Flame Puzzle" scene has been loaded; now you can access the manager
            FlamePuzzleManager puzzleManager = FindObjectOfType<FlamePuzzleManager>();

            if (puzzleManager != null)
            {
                puzzleManager.InstantiateObject(m_triggerID, m_puzzleID, "Flame Puzzle");
                puzzleManager.PuzzleCompleted += OnReturnInput;
            }
        }
    }

    private void OnReturnInput()
    {
        if (Menu.GameMenu.Instance.gameMode == Menu.GameMenu.GameMode.MINIGAME)
        {
            GameEvents.Instance.DeactivatePlayerInput(false);
            CallInteractionCompletedEvent();
            SceneManager.UnloadSceneAsync("Flame Puzzle");
        }
    }

    private void OnPuzzleCompleted()
    {
        canInteract = false;
        GameEvents.Instance.DeactivatePlayerInput(false);
        CallInteractionCompletedEvent();
        SceneManager.UnloadSceneAsync("Flame Puzzle");          
    }

    public override void Interact()
    {
        base.Interact();

        GameEvents.Instance.DeactivatePlayerInput(true);
        Menu.GameMenu.Instance.gameMode = Menu.GameMenu.GameMode.MINIGAME;
        SceneManager.LoadScene("Flame Puzzle", LoadSceneMode.Additive);
    }
}
