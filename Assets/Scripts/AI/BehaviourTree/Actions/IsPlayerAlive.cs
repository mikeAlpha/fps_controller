using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace mikealpha
{
    public class IsPlayerAlive : Condition
    {
        private Transform mTransform;
        private AITree mTree;
        int deadCount = 0;
        bool allDead = false;

        public IsPlayerAlive(Transform transform)
        {
            mTransform = transform;
            if (mTransform.GetComponent<AITree>() != null)
            {
                mTree = mTransform.GetComponent<AITree>();
            }
        }

        void CheckTargetHealth()
        {
            if (mTree != null && mTree.targets.Count > 0)
            {
                int tCount = mTree.targets.Count;
                var targets = mTree.targets;
                for(int i = 0; i < targets.Count;i++ )
                {
                    if (targets[i].GetComponent<PlayerController>() != null) {
                        var player = targets[i].GetComponent<PlayerController>();
                        if (!targets[i].GetComponent<PlayerController>().IsPlayerActive)
                        {
                            deadCount++;
                        }
                    }
                    if (deadCount == tCount)
                    {
                        allDead = true;
                        break;
                    }
                }
            }
        }
        public override bool CheckCondition()
        {
            CheckTargetHealth();
            if (allDead)
                return false;
            else 
                return true;
            
        }
    }
}
