using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public Animator animator;

    public bool transitionFinished = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            // Change the "exit" parameter in the Animator to true
            animator.SetBool("exit", true);
        }
    }

    public void AnimationFinished()
    {
        transitionFinished = true;
        Menu.MainMenu.Instance.OnOpenMenu();
        gameObject.SetActive(false);
    }
}
