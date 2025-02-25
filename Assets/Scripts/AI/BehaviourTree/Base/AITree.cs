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
    public AiHealth health;

    public Transform RayCastPoint;

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
    }

    private void AiDead()
    {
        IsAiactive = false;
        var navMesh = GetComponent<NavMeshAgent>();
        navMesh.isStopped = true;
        GetComponent<CapsuleCollider>().enabled = false;
        anim.Play("death");
    }

}
