using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mikealpha
{
    [System.Serializable]
    public class NodeData
    {
        public string id;
        public string title;
        public Vector2 position;
        public bool InNode;
        public bool OutNode;
    }

    [System.Serializable]
    public class ConnectionData
    {
        public string fromNodeId;
        public string toNodeId;
    }

    [System.Serializable]
    public class GraphData
    {
        public List<NodeData> nodes = new List<NodeData>();
        public List<ConnectionData> connections = new List<ConnectionData>();
    }
}
