using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    public Movement Movement
    {
        get => GenericNotImplementedError<Movement>.TryGet(movement, transform.parent.name);
        private set => movement = value;
    }
    public CollisionSenses CollisionSenses
    {
        get => GenericNotImplementedError<CollisionSenses>.TryGet(collisionSenses, transform.parent.name);
        private set => collisionSenses = value;
    }
    public Combat Combat
    {
        get => GenericNotImplementedError<Combat>.TryGet(combat, transform.parent.name);
        private set => combat = value;
    }
    public Stats Stats
    {
        get => GenericNotImplementedError<Stats>.TryGet(stats, transform.parent.name);
        private set => stats = value;
    }
    public Weapon Weapon
    {
        get => GenericNotImplementedError<Weapon>.TryGet(weapon, transform.parent.name);
        private set => weapon = value;
    }
    public EntityDetector EntityDetector
    {
        get => GenericNotImplementedError<EntityDetector>.TryGet(entityDetector, transform.parent.name);
        private set => entityDetector = value;
    }
    public AIMeleeAttackDetector AIMeleeAttackDetector
    {
        get => GenericNotImplementedError<AIMeleeAttackDetector>.TryGet(aiMeleeAttackDetector, transform.parent.name);
        private set => aiMeleeAttackDetector = value;
    }
    public AIRaycast AIRaycast
    {
        get => GenericNotImplementedError<AIRaycast>.TryGet(aiRaycast, transform.parent.name);
        private set => aiRaycast = value;
    }
    public HurtEffect HurtEffect
    {
        get => GenericNotImplementedError<HurtEffect>.TryGet(hurtEffect, transform.parent.name);
        private set => hurtEffect = value;
    }

    private Movement movement;
    private CollisionSenses collisionSenses;
    private Combat combat;
    private Stats stats;
    private Weapon weapon;
    private EntityDetector entityDetector;
    private AIMeleeAttackDetector aiMeleeAttackDetector;
    private AIRaycast aiRaycast;
    private HurtEffect hurtEffect;

    private void Awake()
    {
        Movement = GetComponentInChildren<Movement>();
        CollisionSenses = GetComponentInChildren<CollisionSenses>();
        Combat = GetComponentInChildren<Combat>();
        Stats = GetComponentInChildren<Stats>();
        Weapon = GetComponentInChildren<Weapon>();
        EntityDetector = GetComponentInChildren<EntityDetector>();
        AIMeleeAttackDetector = GetComponentInChildren<AIMeleeAttackDetector>();
        AIRaycast = GetComponentInChildren<AIRaycast>();
        HurtEffect = GetComponentInChildren<HurtEffect>();
    }

    public void LogicUpdate()
    {
        Combat.LogicUpdate();
        Movement.LogicUpdate();
    }
}