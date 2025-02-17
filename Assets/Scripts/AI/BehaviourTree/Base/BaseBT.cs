using UnityEngine;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mikealpha
{
    public abstract class BaseBT : MonoBehaviour
    {
        public Node mRootNode;

        protected float Tick = 0.5f;

        public BehaviorTreeSaveData saveData;

        private List<KeyValuePair<string,Node>> mNodes = new List<KeyValuePair<string, Node>>();

        protected abstract void Start();

        private void Awake()
        {
            CreateNode();
        }

        private void Update()
        {
            if (mRootNode != null)
            {
                mRootNode.UpdateStatus(Tick);
            }
        }

        protected void CreateNode()
        {
            try
            {
                for (int i = 0; i < saveData.Nodes.Count; i++)
                {
                    Assembly assembly = Assembly.Load("Assembly-CSharp");

                    Type type = null;
                    if (saveData.Nodes[i].name == "RootNode")
                        type = assembly.GetType("mikealpha.Fallback");
                    else
                        type = assembly.GetType("mikealpha." + saveData.Nodes[i].name);

                    object[] args = { null };
                    Node nodeData = null;
                    if (type.BaseType.Name == "Action" || type.BaseType.Name == "Condition")
                    {
                        args[0] = transform;
                        nodeData = (Node)Activator.CreateInstance(type, args);
                    }
                    else
                        nodeData = (Node)Activator.CreateInstance(type);


                    mNodes.Add(new KeyValuePair<string, Node>(saveData.Nodes[i].name, nodeData));
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
                for (int i = 0; i < saveData.Connections_2.Count; i++) 
                {
                    string parent_key = saveData.Connections_2[i].parent.name;
                    string child_key = saveData.Connections_2[i].child.name;
                    var parentNodeData = mNodes.FindAll(x => x.Key == parent_key);
                    var childNodeData = mNodes.FindAll(x => x.Key == child_key);

                    if (parentNodeData.Count == 1 && childNodeData.Count == 1)
                        parentNodeData[0].Value.UpdateChildNode(childNodeData[0].Value);

                    //Debug.Log(mNodes.Contains(saveData.Connections_2[i].parent.name));
                    //if (mNodes.ContainsKey(saveData.Connections_2[i].parent.name))
                    //{
                    //    mNodes[saveData.Connections_2[i].parent.name].UpdateChildNode(mNodes[saveData.Connections_2[i].child.name]);
                    //    Debug.Log("Conns====Parent===" + mNodes[saveData.Connections_2[i].parent.name].GetType().Name +"===Child==="+ mNodes[saveData.Connections_2[i].child.name].GetType());
                    //}
                }
            }
        }
    }
}
