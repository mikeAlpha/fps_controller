using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mikealpha;
using TMPro;
using UnityEngine.AI;

public class Patrol : Action
{

    private Transform mTransform;
    private Transform[] mWaypoints;

    private int mCurrent_index = 0;

    public Patrol(Transform transform, Transform[] waypoints)
    {
        mTransform = transform;
        mWaypoints = waypoints;
    }

    protected override void DoAction(float tick)
    {
        Vector3 pos = mWaypoints[mCurrent_index].position;
        pos.y = mTransform.position.y;

        var navMesh = mTransform.GetComponent<NavMeshAgent>();

        navMesh.destination = pos;

        //var direction = navMesh.velocity.normalized;

        var anim = mTransform.GetComponent<AITree>().anim;

        anim.SetFloat("SpeedX", 0, 0.1f, 0.1f);
        anim.SetFloat("SpeedY", /*Mathf.Abs(direction.z)*/1.0f, 0.1f, 0.1f);


        if (Vector3.Distance(mTransform.position, pos) < 0.5f)
        {
            mCurrent_index = (mCurrent_index + 1) % mWaypoints.Length;
        }

        //mTransform.GetComponentInChildren<TMP_Text>().text = "Patrol";
    }
}
