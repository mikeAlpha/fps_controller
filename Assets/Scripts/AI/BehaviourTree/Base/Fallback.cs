using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mikealpha
{
    public class Fallback : Node
    {
        public Fallback()
        {
            //childNodes = nodes;
        }

        public override status UpdateStatus(float tick)
        {
            for (int i = 0; i < childNodes.Count; i++)
            {
                var status = childNodes[i].UpdateStatus(tick);
                if (status == status.running)
                    return status.running;
                else if (status == status.success)
                   return status.success;
            }
            return status.failure;
        }
    }
}
