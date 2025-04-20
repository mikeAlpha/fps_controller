using UnityEngine;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mikealpha
{
    public abstract class BaseBT : MonoBehaviour, IPlayerController
    {
        public Node mRootNode;

        protected float Tick = 0.5f;

        private GraphData saveData;

        [SerializeField]
        private TextAsset TreeData;

        private List<KeyValuePair<string, Node>> mNodes = new List<KeyValuePair<string, Node>>();

        public bool IsAiactive = true;

        protected abstract void Start();
        protected abstract void OnEnable();

        protected abstract void OnDisable();

        private void Awake()
        {
            CreateNode();
        }

        private void Update()
        {
            Debug.Log(mRootNode);
            if (mRootNode != null && IsAiactive)
            {
                mRootNode.UpdateStatus(Tick);
            }
        }

        protected void CreateNode()
        {
            try
            {
                saveData = JsonUtility.FromJson<GraphData>(TreeData.text);

                for (int i = 0; i < saveData.nodes.Count; i++)
                {
                    Assembly assembly = Assembly.Load("Assembly-CSharp");

                    Type type = null;
                    if (saveData.nodes[i].title == "RootNode")
                        type = assembly.GetType("mikealpha.Fallback");
                    else
                        type = assembly.GetType("mikealpha." + saveData.nodes[i].title);

                    object[] args = { null };
                    Node nodeData = null;
                    if (type.BaseType.Name == "Action" || type.BaseType.Name == "Condition")
                    {
                        args[0] = transform;
                        nodeData = (Node)Activator.CreateInstance(type, args);
                    }
                    else
                        nodeData = (Node)Activator.CreateInstance(type);


                    mNodes.Add(new KeyValuePair<string, Node>(saveData.nodes[i].title, nodeData));
                }

                ConfigConns();
                mRootNode = mNodes[0].Value;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        protected void ConfigConns()
        {
            if (mNodes != null && mNodes.Count > 0)
            {
                for (int i = 0; i < saveData.connections.Count; i++)
                {
                    string parent_key = saveData.nodes.Find(x => x.id == saveData.connections[i].fromNodeId).title;
                    string child_key = saveData.nodes.Find(x => x.id == saveData.connections[i].toNodeId).title;

                    var parentNodeData = mNodes.FindAll(x => x.Key == parent_key);
                    var childNodeData = mNodes.FindAll(x => x.Key == child_key);

                    if (parentNodeData.Count == 1 && childNodeData.Count == 1)
                    {
                        parentNodeData[0].Value.UpdateChildNode(childNodeData[0].Value);
                    }
                }
            }
        }
    }
}

