using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructionAnimationHandler : MonoBehaviour
{
    private const string BROKE_TILE_ANIM = "Broke Tile";
    private const string BROKE_WALL_ANIM = "Broke Wall";

    private TilemapDestruction tilemapDestruction;
    private Animator animator;
    private Collider2D parentCollider;
    private TilemapRenderer parentTilemapRenderer;

    private void Awake()
    {
        tilemapDestruction = GetComponentInParent<TilemapDestruction>();
        animator = GetComponent<Animator>();
        
        // Cache parent components
        Transform parentTransform = transform.parent;
        if (parentTransform != null)
        {
            parentCollider = parentTransform.GetComponent<Collider2D>();
            parentTilemapRenderer = parentTransform.GetComponent<TilemapRenderer>();
        }
    }

    public void PlayDestructionAnimation()
    {
        gameObject.SetActive(true);
        animator.Play(BROKE_TILE_ANIM, 0, 0f);
    }

    public void PlayFinalDestructionAnimation()
    {
        gameObject.SetActive(true);
        
        // Disable parent components
        if (parentCollider != null) parentCollider.enabled = false;
        if (parentTilemapRenderer != null) parentTilemapRenderer.enabled = false;
        
        animator.Play(BROKE_WALL_ANIM, 0, 0f);
    }

    private void OnAnimationFinishTrigger()
    {
        gameObject.SetActive(false);
        animator.StopPlayback();
    }

    private void OnFinalAnimationFinishTrigger()
    {
        if (tilemapDestruction != null)
        {
            tilemapDestruction.DestroyWall();
        }
    }
}
