using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mikealpha;

public class AITree : BaseBT
{
    public Transform[] Waypoints;
    public float ViewAngle = 120f;
    public float ViewRadius = 20f;

    public LayerMask /*EnemyLayer, */ObstacleLayer;

    public List<Transform> targets = new List<Transform>();

    public Animator anim;

    public AiHealth health;

    public BehaviorTreeSaveData saveData;

    protected override Node CreateNode()
    {
        Node root = new Fallback(new List<Node>() {
            new Sequence(new List<Node>
            {
                new EnemyWithinRange(transform)
            }),
            new Patrol(transform)
            });

        return root;
    }

    protected override void Start()
    {
        health = new AiHealth(this);
    }
}
