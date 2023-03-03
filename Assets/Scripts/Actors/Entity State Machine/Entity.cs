using Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    #region State Variables
    public StateMachine StateMachine { get; set; }
    #endregion

    #region Components
    public Core Core { get; set; }
    public Animator Anim { get; private set; }
    public BoxCollider2D MovementCollider { get; private set; }
    #endregion

    #region Other Variables
    protected Vector2 workspace;
    #endregion

    #region Unity Callback Functions
    public virtual void Awake()
    {
        Core = GetComponentInChildren<Core>();
        StateMachine = new StateMachine();
    }

    public virtual void Start()
    {
        Anim = GetComponent<Animator>();
        MovementCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        Core.LogicUpdate();
        StateMachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }
    #endregion

    #region Other Functions
    protected void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();
    protected void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();
    protected void AnimationActionTrigger() => StateMachine.CurrentState.AnimationActionTrigger();
    public abstract State GetDeathState();
    public abstract State GetHurtState();
    
    public void PlaySound(SfxClip sfxClip)
    {
        if (sfxClip != null)
            sfxClip.AudioGroup.RaisePrimaryAudioEvent(sfxClip.AudioGroup.AudioSource, sfxClip, sfxClip.AudioConfiguration);

    }
    #endregion
}
