using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mikealpha
{
    public class Sequence : Node
    {
        public Sequence()
        {
          //childNodes = nodes;
        }

        public override status UpdateStatus(float tick)
        {
            for(int i = 0; i<childNodes.Count;i++)
            {
                var status = childNodes[i].UpdateStatus(tick);
                //Debug.Log("status====" + childNodes[i].GetType().Name + "====" + status);
                if (status == status.running)
                    return status.running;
                else if (status == status.failure)
                    return status.failure;
            }
           return status.success;
        }
    }
}
