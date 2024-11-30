using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace mikealpha
{
    public class Condition : Node
    {
        public virtual bool CheckCondition()
        {
            return true;
        }

        public override status UpdateStatus(float tick)
        {
            if (CheckCondition())
                return status.success;
            else
                return status.failure;
        }
    }
}
