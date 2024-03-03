using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlamePuzzleTrigger : Interactable
{
    [SerializeField] private int m_puzzleID = 0;
    [SerializeField] private int m_triggerID = 1;
    private bool isCompleted = false;

    private Animator puzzleAnim;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    IEnumerator Start()
    {
        puzzleAnim = GetComponent<Animator>();

        yield return new WaitForEndOfFrame();
        var  triggerState = DialogueLua.GetVariable("FlamePuzzle." + m_triggerID).asBool;

        if (triggerState) SetCompleted();
    }

    private void SetCompleted()
    {
        canInteract = false;
        isCompleted = true;
        puzzleAnim.SetBool("isCompleted", true);
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
                FlamePuzzleManager.Instance.PuzzleCompleted += OnPuzzleCompleted;
            }
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == "Flame Puzzle")
        {
            GameEvents.Instance.DeactivatePlayerInput(false);

            if (FlamePuzzleManager.Instance)
                FlamePuzzleManager.Instance.PuzzleCompleted -= OnPuzzleCompleted;

            if (!isCompleted)
                CallInteractionCompletedEvent();
        }
    }

    private void OnPuzzleCompleted()
    {
        SetCompleted();
        DialogueLua.SetVariable("FlamePuzzle." + m_triggerID, true);
        GameEvents.Instance.DeactivatePlayerInput(false);
        GameEvents.Instance.PuzzleClose("Flame Puzzle");
    }

    public override void Interact()
    {
        base.Interact();

        GameEvents.Instance.DeactivatePlayerInput(true);
        GameEvents.Instance.PuzzleOpen("Flame Puzzle");
    }
}
