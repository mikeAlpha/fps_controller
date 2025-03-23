using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mikealpha {
    public class EnemyWithinHearingRange : Condition
    {
        private Transform mTransform;
        private AITree mTree;

        public EnemyWithinHearingRange(Transform transform)
        {
            mTransform = transform;
            if (mTransform.GetComponent<AITree>() != null)
            {
                mTree = mTransform.GetComponent<AITree>();
            }
        }

        public override bool CheckCondition()
        {
            return base.CheckCondition();
        }
    }
}
