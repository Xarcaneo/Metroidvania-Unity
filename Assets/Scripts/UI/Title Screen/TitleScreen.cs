using UnityEngine;

/// <summary>
/// Manages the title screen behavior and transition to the main menu.
/// </summary>
[RequireComponent(typeof(Animator))]
public class TitleScreen : MonoBehaviour
{
    private Animator animator;
    private static readonly int ExitParam = Animator.StringToHash("exit");
    
    /// <summary>
    /// Indicates whether the transition animation has finished playing.
    /// </summary>
    public bool TransitionFinished { get; private set; }

    /// <summary>
    /// Initializes required components.
    /// </summary>
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Checks for any key press to trigger the exit animation.
    /// </summary>
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            animator.SetBool(ExitParam, true);
        }
    }

    /// <summary>
    /// Called by the animation event when the exit animation finishes.
    /// Transitions to the main menu and deactivates the title screen.
    /// </summary>
    public void AnimationFinished()
    {
        TransitionFinished = true;
        Menu.MainMenu.Instance.OnOpenMenu();
        gameObject.SetActive(false);
    }
}
