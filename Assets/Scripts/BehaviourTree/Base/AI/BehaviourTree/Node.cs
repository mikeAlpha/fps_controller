using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace mikealpha
{

    public enum status
    {
        success,
        running,
        failure
    }


    public abstract class Node
    {
        protected List<Node> childNodes = new List<Node>();
        public virtual status UpdateStatus(float tick) { return status.failure; }
    }
}
