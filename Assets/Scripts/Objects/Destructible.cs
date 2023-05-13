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

    private void OnDisable()
    {
        if (isDestroyed) Destroy(gameObject);
    }

    public void InstantKill() { }

    public void AnimationFinishTrigger()
    {
        if (m_spriteRenderer != null)
        {
            StartCoroutine(FadeOutSprite(m_spriteRenderer));
        }
    }

    private IEnumerator FadeOutSprite(SpriteRenderer spriteRenderer)
    {
        Color color = spriteRenderer.color;
        float alpha = color.a;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime;
            color = new Color(color.r, color.g, color.b, alpha);
            spriteRenderer.color = color;
            yield return null;
        }
        Destroy(gameObject);
    }
}
