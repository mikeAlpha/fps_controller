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

    [System.Serializable]
    public abstract class Node
    {
        protected List<Node> childNodes = new List<Node>();
        public virtual status UpdateStatus(float tick) { return status.failure; }

        public void UpdateChildNode(Node node) 
        {
            if (childNodes != null)
            {
                childNodes.Add(node);
            }
        }

        public void RemoveChildNode(Node node) 
        {
            if (childNodes != null && childNodes.Contains(node)) 
            { 
                childNodes.Remove(node);
            }
        }
    }
}
