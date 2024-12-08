using mikealpha;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mikealpha
{
    public abstract class BaseBT : MonoBehaviour
    {
        private Node mRootNode;

        protected float Tick = 0.5f;

        protected abstract void Start();

        private void Awake()
        {
            mRootNode = CreateNode();
        }

        private void Update()
        {
            if (mRootNode != null)
                mRootNode.UpdateStatus(Tick);
        }

        protected abstract Node CreateNode();
    }
}
