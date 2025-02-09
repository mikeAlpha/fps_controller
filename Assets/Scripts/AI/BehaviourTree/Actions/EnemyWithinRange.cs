using UnityEngine;

namespace mikealpha
{
    public class EnemyWithinRange : Condition
    {
        private Transform mTransform;
        private AITree mTree;

        public EnemyWithinRange(Transform transform)
        {
            mTransform = transform;
            if (mTransform.GetComponent<AITree>() != null)
            {
                mTree = mTransform.GetComponent<AITree>();
            }
        }

        void FindTarget()
        {
            mTree.targets.Clear();
            Collider[] col = Physics.OverlapSphere(mTransform.position, mTree.ViewRadius);
            foreach (Collider c in col)
            {
                if (c.gameObject.CompareTag("Player"))
                {
                    var dir = (c.transform.position - mTransform.position).normalized;
                    float angleWithEnemy = Vector3.Angle(mTransform.forward, dir);
                    var dst = Vector3.Distance(mTransform.position, c.transform.position);
                    if (angleWithEnemy < mTree.ViewAngle / 2)
                    {
                        if (Physics.Raycast(mTransform.position, dir, dst, mTree.ObstacleLayer))
                            return;
                        else
                        {
                            mTree.targets.Add(c.transform);
                        }
                    }
                }
            }
        }

        public override bool CheckCondition()
        {
            FindTarget();
            if (mTree.targets.Count > 0)
                return true;
            else
                return false;
        }
    }
}
