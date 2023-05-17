using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour, IDamageable
{
    private bool isDestroyed = false;
    private Animator m_animator;
    private SpriteRenderer m_spriteRenderer;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Damage(IDamageable.DamageData damageData)
    {
        if (!isDestroyed)
        {
            isDestroyed = true;
            m_animator.SetBool("Destroy", true);
        }
    }

    public void InstantKill() { }

    public void AnimationFinishTrigger()
    {
        if (m_spriteRenderer != null)
        {
            Destroy(gameObject);
        }
    }
}
