using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mikealpha;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class AITree : BaseBT
{
    public Transform[] Waypoints;
    public float ViewAngle = 120f;
    public float ViewRadius = 20f;

    public LayerMask /*EnemyLayer, */ObstacleLayer;
    public List<Transform> targets = new List<Transform>();
    public Animator anim;
    public PlayerTPSController tps_controller;
    public AiHealth health;

    public Transform RayCastPoint;
    public WeaponController mWeaponController;

    public EquippableItem currentItem;

    protected override void OnEnable()
    {
        EventHandler.RegisterEvent(gameObject, GameEvents.OnAiDead, AiDead);
    }

    protected override void OnDisable()
    {
        EventHandler.UnregisterEvent(gameObject, GameEvents.OnAiDead, AiDead);
    }

    protected override void Start()
    {
        health = new AiHealth(this);
        mWeaponController.InitWeapon(currentItem as BaseWeapon, this);
    }

    private void AiDead()
    {
        tps_controller.DeadWeight();
        var navMesh = GetComponent<NavMeshAgent>();
        navMesh.isStopped = true;
        IsAiactive = false;
        GetComponent<CapsuleCollider>().enabled = false;
        anim.Play("death");
    }

}
