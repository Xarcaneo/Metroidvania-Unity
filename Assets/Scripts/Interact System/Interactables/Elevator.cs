using PixelCrushers;
using UnityEngine;

public class Elevator : Interactable
{
    [SerializeField] private ScenePortal scenePortal;

    private Animator entityAnim; // Reference to the Animator component

    const string idleOnParam = "IdleOn";
    const string turningOnParam = "TurningOn";

    private void Start()
    {
        entityAnim = GetComponent<Animator>();
    }

    void OnDestroy()
    {
        InputManager.Instance.isInputActive = true;
    }

    public override void Interact()
    {
        InputManager.Instance.isInputActive = false;

        // Start the elevator animation
        entityAnim.SetBool(turningOnParam, true);

        // Disable player and move them to the elevator position
        GameEvents.Instance.DeactivatePlayerInput(true);
        Player.Instance.gameObject.transform.position =
            new Vector3(this.transform.position.x, Player.Instance.gameObject.transform.position.y, 0.0f);
        Player.Instance.gameObject.SetActive(false);
    }

    void OnAnimationTrigger()
    {
        // Reset the animation parameters
        entityAnim.SetBool(turningOnParam, false);
        entityAnim.SetBool(idleOnParam, true);

        // Enable player and reset the elevator movement flag
        if (Player.Instance.Core.GetCoreComponent<Movement>().FacingDirection != this.transform.localScale.x)
            Player.Instance.Core.GetCoreComponent<Movement>().Flip();

        Player.Instance.gameObject.SetActive(true);
        scenePortal.gameObject.SetActive(true);
    }
}
