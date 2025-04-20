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
            if (mITree.targets.Count > 0)
            {
                var target = mITree.targets;
                foreach (var t in target)
                {
                    if (t.GetComponent<PlayerControllerV2>().IsPlayerActive)
                    {
                        if (Vector3.Distance(mTransform.position, t.position) > AttackDistance)
                        {
                            mAgent.destination = t.position;
                            mAgent.isStopped = false;
                            var anim = mITree.anim;
                            anim.SetFloat("SpeedX", 0, 0.1f, 0.1f);
                            anim.SetFloat("SpeedY", /*Mathf.Abs(direction.z)*/1.0f, 0.1f, 0.1f);
                        }

                        else
                        {
                            mAgent.destination = t.position;
                            mAgent.isStopped = true;

                            var rot = Quaternion.LookRotation(t.position - mTransform.position);
                            mTransform.rotation = rot;

                            var anim = mITree.anim;
                            anim.SetFloat("SpeedX", 0, 0.1f, 0.1f);
                            anim.SetFloat("SpeedY", /*Mathf.Abs(direction.z)*/0.0f, 0.1f, 0.1f);
                            EventHandler.ExecuteEvent(mTransform.gameObject, GameEvents.OnAiFireUpdate);
                        }
                    }
                }
            }
        }
    }
}