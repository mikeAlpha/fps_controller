using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mikealpha
{
    public class Parallel : Node
    {
        public Parallel()
        {
            //childNodes = nodes;
        }

        public override status UpdateStatus(float tick)
        {
            int pCount = 0;
            int nCount = 0;
            int count = 0;
            int length = childNodes.Count;
            foreach(Node n in childNodes)
            {
                count++;
                var status = n.UpdateStatus(tick);
                if (status == status.success)
                    pCount++;
                else if (status == status.failure)
                    nCount++;

                if (pCount >= nCount && count >= length)
                    return status.success;
                else if (pCount < nCount && count >= length)
                    return status.failure;
            }
            return status.running;
        }
    }
}
