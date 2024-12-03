using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mikealpha
{
    public class Action : Node
    {
        protected virtual void DoAction(float tick) { }

        public override status UpdateStatus(float tick)
        {
            DoAction(tick);
            return status.running;
        }
    }
}
