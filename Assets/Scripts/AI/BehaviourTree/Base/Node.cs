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
        public List<Node> childNodes = new List<Node>();
        public virtual status UpdateStatus(float tick) { return status.failure; }

        public void UpdateChildNode(Node node) 
        {
            if (childNodes != null)
            {
                Debug.Log("Added");
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
