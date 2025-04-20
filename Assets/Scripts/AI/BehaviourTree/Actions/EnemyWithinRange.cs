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
            Debug.Log("Checking");
            foreach (Collider c in col)
            {
                Debug.Log(" Not Seen");
                if (c.gameObject.CompareTag("Player") && c.GetComponent<PlayerControllerV2>().IsPlayerActive)
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
                            Debug.Log("Seen");
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
