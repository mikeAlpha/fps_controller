using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mikealpha;
using TMPro;
using UnityEngine.AI;

namespace mikealpha
{
    public class Patrol : Action
    {

        private Transform mTransform;
        private Transform[] mWaypoints;

        private int mCurrent_index = 0;

        public Patrol(Transform transform)
        {
            mTransform = transform;
            if (mTransform.GetComponent<AITree>() != null)
            {
                var aiTree = mTransform.GetComponent<AITree>();
                mWaypoints = aiTree.Waypoints;
            }
        }

        protected override void DoAction(float tick)
        {

            Vector3 pos = mWaypoints[mCurrent_index].position;
            pos.y = mTransform.position.y;

            var navMesh = mTransform.GetComponent<NavMeshAgent>();
            navMesh.destination = pos;
            navMesh.isStopped = false;
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
}