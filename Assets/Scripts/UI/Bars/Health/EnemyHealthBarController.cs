using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBarController : HealthBarController
{
    [SerializeField] GameObject body;
    [SerializeField] Vector3 yAdjust = new Vector3(-0.65f, 1, 0);

    private Animator Anim;

    virtual public void Start()
    {
        Anim = GetComponent<Animator>();

        if (stats)
        {
            SetMaxHealth(stats.GetMaxHealth());
            health = stats.GetMaxHealth();

            stats.Damaged += TakeDamage;
        }
    }

    public override void UpdateHealthUI()
    {
        base.UpdateHealthUI();
        SetUIPosition();

        if (health <= 0)
        {
            stats.Damaged -= TakeDamage;
            Anim.Play("FadeOut");
        }
    }


    private void SetUIPosition()
    {
        transform.position = body.transform.position - yAdjust;
    }
}
