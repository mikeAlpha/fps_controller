using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace mikealpha
{
    public class Attack : Action
    {
        private Transform mTransform;
        private AITree mITree;
        private NavMeshAgent mAgent;
        private float AttackDistance = 30f;

        public Attack(Transform transform)
        {
            mTransform = transform;
            mITree = mTransform.GetComponent<AITree>();
            mAgent = mTransform.GetComponent<NavMeshAgent>();
        }
        protected override void DoAction(float tick)
        {
            //EventHandler.ExecuteEvent(mTransform.gameObject, GameEvents.OnAiFireUpdate);
            ChaseAndAttack();
        }

        private void ChaseAndAttack()
        {
            if (mITree.targets.Count == 1)
            {
                var target = mITree.targets[0];
                if (Vector3.Distance(mTransform.position, target.position) > AttackDistance)
                {
                    mAgent.destination = target.position;
                    mAgent.isStopped = false;
                }

                else
                {
                    mAgent.destination = target.position;
                    mAgent.isStopped = true;
                    var anim = mITree.anim;
                    anim.SetFloat("SpeedX", 0, 0.1f, 0.1f);
                    anim.SetFloat("SpeedY", /*Mathf.Abs(direction.z)*/0.0f, 0.1f, 0.1f);
                    EventHandler.ExecuteEvent(mTransform.gameObject, GameEvents.OnAiFireUpdate);
                }
            }
        }
    }
}