using mikealpha;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Behavior Tree", menuName = "MikeyTools/Create new BTree..")]
[System.Serializable]
public class BehaviorTreeSaveData : ScriptableObject
{
    [HideInInspector]
    public List<BehaviourTreeNode> Nodes;

    public List<(BehaviourTreeNode, BehaviourTreeNode)> Connections;

    public List<BehaviourTreeConnections> Connections_2;
}

[System.Serializable]
public class BehaviourTreeConnections
{
    public BehaviourTreeNode parent;
    public BehaviourTreeNode child;
}