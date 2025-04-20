using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using mikealpha;
using System;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class BehaviorTreeEditor : EditorWindow
{
    //private const float panelWidth = 200f;
    //private string[] nodesTy = new string[] { "Composites", "Actions", "Conditions" };
    //private Dictionary<string, List<string>> nodeTypes = new Dictionary<string, List<string>> {
    //    { "Composites", new List<string>() },
    //    { "Actions", new List<string> () },
    //    { "Conditions", new List<string>()}
    //};


    //private int selectedNodeType = 0;
    //private List<string> selectedList = new List<string>();

    //public AITree aiTree;
    //private bool IsSelected = false;

    //private List<BehaviourTreeNode> nodes = new List<BehaviourTreeNode>();
    //private List<(BehaviourTreeNode, BehaviourTreeNode)> connections = new List<(BehaviourTreeNode, BehaviourTreeNode)>();
    //private List<string> nodeList = new List<string>();
    //private BehaviourTreeNode selectedNode;

    //private bool IsConnectionloaded = false;
    //public static bool IsWindowActivated = false;

    //[MenuItem("MikeAlpha/AiBehaviorTree")]
    //public static void ShowWindow()
    //{
    //    var bTreeEditor = GetWindow<BehaviorTreeEditor>();
    //    bTreeEditor.GetAllNodeData();
    //}


    //private void GetAllNodeData()
    //{
    //    var getCompositeNodeData = GetDerivedClassNames.GetDerivedClasses<Node>();
    //    foreach (var node in getCompositeNodeData)
    //    {
    //        if (node.Name == "Sequence" || node.Name == "Fallback" || node.Name == "Parallel")
    //            nodeTypes["Composites"].Add(node.Name);
    //    }

    //    var getConditionNodeData = GetDerivedClassNames.GetDerivedClasses<Condition>();
    //    foreach (var node in getConditionNodeData)
    //        nodeTypes["Conditions"].Add(node.Name);

    //    var getActionNodeData = GetDerivedClassNames.GetDerivedClasses<mikealpha.Action>();
    //    foreach (var node in getActionNodeData)
    //        nodeTypes["Actions"].Add(node.Name);
    //}

    //private void OnGUI()
    //{
    //    CheckGOAiTree();
    //    if (aiTree != null)
    //    {
    //        DrawPanel();

    //        GUILayout.BeginHorizontal(EditorStyles.toolbar);
    //        if (GUILayout.Button("Save Tree", EditorStyles.toolbarButton))
    //            SaveAsset();

    //        //if (GUILayout.Button("Load Tree", EditorStyles.toolbarButton))
    //        //    LoadAsset();

    //        GUILayout.EndHorizontal();

    //        GUI.BeginGroup(new Rect(panelWidth, 0, position.width - panelWidth, position.height));
    //        DrawNodes(Event.current);
    //        DrawConnections(Event.current);
    //        ProcessEvents(Event.current);
    //        GUI.EndGroup();

    //        if (GUI.changed)
    //            Repaint();
    //    }

    //}

    //private void CheckGOAiTree()
    //{
    //    var go = Selection.activeGameObject;
    //    if (go != null)
    //    {
    //        if (go.GetComponent<AITree>() != null)
    //        {
    //            aiTree = go.GetComponent<AITree>();
    //            if (!IsSelected && aiTree.saveData != null)
    //            {

    //                if (aiTree.saveData.Nodes != null && aiTree.saveData.Nodes.Count > 0)
    //                {
    //                    nodes = aiTree.saveData.Nodes;
    //                    if (aiTree.saveData.Connections_2 != null)
    //                    {
    //                        //connections = aiTree.saveData.Connections;
    //                        foreach (var conn in aiTree.saveData.Connections_2)
    //                            connections.Add((conn.parent, conn.child));
                      
    //                    }
    //                    //UpdateConns();
    //                }
    //                else
    //                {

    //                    //Node nodeData = (Node)Activator.CreateInstance(type, args);
    //                    Vector2 position = new Vector2(panelWidth + 10, panelRect.height / 2);


    //                    var node = new BehaviourTreeNode("RootNode", position, false, true);
    //                    node.IsSelected = false;
    //                    nodes.Add(node);
    //                }

    //                IsSelected = true;
    //            }
    //        }
    //        else
    //            aiTree = null;

    //        if (Selection.activeGameObject == null)
    //        {
    //            aiTree = null;
    //            IsSelected = false;
    //        }
    //    }
    //}

    //private void UpdateConns()
    //{
    //    foreach (var conn in aiTree.saveData.Connections_2)
    //    {
    //        startPos = conn.parent.OutNode.center;
    //        endPos = conn.child.InNode.center;
    //        Handles.DrawBezier(startPos, endPos, startPos + Vector3.right * 5, endPos + Vector3.left * 5, Color.red, null, 3f);
    //        Debug.Log("Here====update conns");
    //    }
    //}

    //private void DrawNodes(Event e)
    //{
    //    var boxStyle = new GUIStyle(GUI.skin.box);
    //    boxStyle.normal.background = MakeTexture(1, 1, Color.green);

    //    foreach (var node in nodes)
    //    {
    //        Vector2 mousePos = e.mousePosition;
    //        if (node.InNode.Contains(mousePos))
    //        {
    //            node.Draw(boxStyle, null);
    //        }
    //        else if (node.OutNode.Contains(mousePos))
    //        {
    //            node.Draw(null, boxStyle);
    //        }
    //        else
    //        {
    //            node.Draw(null, null);
    //        }
    //    }
    //}

    //private Texture2D MakeTexture(int width, int height, Color color)
    //{
    //    Texture2D texture = new Texture2D(width, height);
    //    Color[] pixels = new Color[width * height];
    //    for (int i = 0; i < pixels.Length; i++)
    //    {
    //        pixels[i] = color;
    //    }
    //    texture.SetPixels(pixels);
    //    texture.Apply();
    //    return texture;
    //}

    //private void DrawConnections(Event e)
    //{
    //    if (e.type == EventType.Repaint && IsDraggingConnection)
    //    {
    //        Handles.DrawBezier(startPos, endPos, startPos + Vector3.right * 5, endPos + Vector3.left * 5, Color.red, null, 3f);
    //        TryConnectNodes(endPos);
    //    }

    //    if (connections != null)
    //    {
    //        foreach (var conn in connections)
    //        {
    //            //Debug.Log(conn.Item1.name + "=====" + conn.Item2.name);
    //            DrawConnectionInternal(conn.Item1, conn.Item2);
    //        }
    //        //IsConnectionloaded = true;
    //    }

    //    SceneView.RepaintAll();
    //}

    //private void RemoveConnection(BehaviourTreeNode node)
    //{
    //    for (int i = 0; i < connections.Count; i++)
    //    {
    //        var connection = connections[i];
    //        if (connection.Item2 == node)
    //        {
    //            //connection.Item1.node.RemoveChildNode(connection.Item2.node);
    //            connections.RemoveAt(i);
    //            Repaint();
    //            return;
    //        }
    //    }
    //}

    //void LoadAsset()
    //{
    //    string path = EditorUtility.OpenFilePanel("Load Tree", "", "tree");
    //    if (string.IsNullOrEmpty(path))
    //    {
    //        var formatter = new BinaryFormatter();
    //        using (var stream = new FileStream(path, FileMode.Open))
    //        {
    //            var saveData = (BehaviorTreeSaveData)formatter.Deserialize(stream);
    //            nodes = saveData.Nodes;
    //            connections = saveData.Connections;
    //        }
    //    }
    //}

    //void SaveAsset()
    //{
    //    try
    //    {
    //        BehaviorTreeSaveData saveData = aiTree.saveData;

    //        saveData.Nodes = nodes;
    //        //saveData.Connections = connections;

    //        if (saveData.Connections_2 == null)
    //            saveData.Connections_2 = new List<BehaviourTreeConnections>();

    //        foreach(var conn in connections)
    //            saveData.Connections_2.Add(new BehaviourTreeConnections { parent = conn.Item1, child = conn.Item2 });

    //        EditorUtility.SetDirty(saveData);
    //        AssetDatabase.SaveAssetIfDirty(saveData);
    //        AssetDatabase.Refresh();
    //        Debug.Log("Tree saved");
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.Log(ex.Message);
    //    }
    //}

    //private void DrawConnectionInternal(BehaviourTreeNode fromNode, BehaviourTreeNode toNode)
    //{
    //    Vector3 start = new Vector3(fromNode.OutNode.center.x, fromNode.OutNode.center.y, 0);
    //    Vector3 end = new Vector3(toNode.InNode.center.x, toNode.InNode.center.y, 0);

    //    Debug.Log(start + "====" + end);

    //    Handles.DrawBezier(start, end, start + Vector3.right * 5, end + Vector3.left * 5, Color.red, null, 3f);

    //    //Debug.Log("Update conns=====");
        
    //    if (!connections.Contains((fromNode, toNode)))
    //    {
    //        //if (fromNode.node != null && toNode.node != null)
    //        //{
    //        //    fromNode.node.UpdateChildNode(toNode.node);

    //        if (aiTree.saveData.Connections_2 == null)
    //            aiTree.saveData.Connections_2 = new List<BehaviourTreeConnections>();

    //        aiTree.saveData.Connections_2.Add(new BehaviourTreeConnections { parent = fromNode, child = toNode });
    //        //}
    //    }
    //}

    //private Vector3 endPos;
    //private void ProcessEvents(Event e)
    //{
    //    switch (e.type)
    //    {
    //        case EventType.MouseDown:
    //            if (e.button == 1)
    //            {
    //                //CreateContectMenu(e.mousePosition);
    //            }
    //            else if (e.button == 0)
    //            {
    //                if (selectedNode != null)
    //                {
    //                    //TryConnectNodes(e.mousePosition);
    //                }
    //                else
    //                {
    //                    SelectNode(e.mousePosition);
    //                }
    //            }
    //            break;
    //        case EventType.MouseDrag:
    //            if (e.button == 0)
    //            {
    //                if (startPos != Vector3.zero)
    //                    endPos = e.mousePosition;

    //                OnDragSelectedNode(e);
    //            }
    //            break;
    //        case EventType.MouseUp:
    //            if (e.button == 0)
    //            {
    //                if (selectedNode != null)
    //                    selectedNode.IsSelected = false;
    //                if(draggingNode != null)
    //                    draggingNode.IsSelected = false;

    //                selectedNode = null;
    //                draggingNode = null;
    //                IsDraggingConnection = false;
    //            }
    //            break;
    //    }

    //    Repaint();
    //}


    //Rect panelRect;
    //private void DrawPanel()
    //{
    //    panelRect = new Rect(0, 0, panelWidth, position.height);
    //    GUI.Box(panelRect, "Actions");

    //    GUILayout.BeginArea(panelRect);
    //    GUILayout.Space(10);

    //    GUILayout.Label("");
    //    foreach (var node in nodeTypes)
    //    {
    //        EditorGUILayout.LabelField(node.Key.ToUpper());
    //        var nodeLists = node.Value;
    //        for (int i = 0; i < nodeLists.Count; i++)
    //        {
    //            if (GUILayout.Button(nodeLists[i]))
    //            {
    //                try
    //                {
    //                    Vector2 position = new Vector2(panelWidth + 10, panelRect.height / 2);

    //                    Assembly assembly = Assembly.Load("Assembly-CSharp");
    //                    Type type = assembly.GetType("mikealpha." + nodeLists[i]);
                        
    //                    //object[] args = { null };
    //                    var InNode = true;
    //                    var OutNode = true;
    //                    if (type.BaseType.Name == "Action" || type.BaseType.Name == "Condition") {
    //                        //args[0] = aiTree.transform;
    //                        InNode = true;
    //                        OutNode = false;
    //                    }

    //                    //Node nodeData = (Node)Activator.CreateInstance(type, args);
    //                    CreateNode(position, /*nodeData,*/ InNode, OutNode, nodeLists[i]);
    //                }
    //                catch (Exception ex)
    //                {
    //                    Debug.LogException(ex);
    //                }
    //            }
    //        }
    //    }

    //    GUILayout.EndArea();
    //}

    //private void CreateNode(Vector2 position,/* Node nodeData,*/ bool InNode, bool OutNode,string nodeName = "Default")
    //{
    //    var node = new BehaviourTreeNode(nodeName, position, /*nodeData,*/ InNode, OutNode);
    //    nodes.Add(node);
    //}

    //private void TryConnectNodes(Vector2 mousePos)
    //{
    //    foreach (var node in nodes)
    //    {
    //        if (node.InNode.Contains(mousePos) && node != selectedNode)
    //        {
    //            connections.Add((selectedNode, node));
    //            selectedNode = null;
    //            IsDraggingConnection = false;
    //            break;
    //        }
    //    }
    //}

    //private BehaviourTreeNode draggingNode;
    //private bool IsDraggingConnection = false;
    //private Vector3 startPos;
    //private void SelectNode(Vector2 mousePos)
    //{
    //    foreach (var node in nodes)
    //    {
    //        if (node.rect.Contains(mousePos))
    //        {
    //            draggingNode = node;
    //            //draggingNode.IsSelected = true;
    //            break;
    //        }

    //        if (node.InNode.Contains(mousePos))
    //        {
    //            RemoveConnection(node);
    //        }


    //        if (node.OutNode.Contains(mousePos) && !IsDraggingConnection)
    //        {
    //            IsDraggingConnection = true;
    //            startPos = mousePos;
    //            endPos = startPos;
    //            selectedNode = node;
    //            //selectedNode.IsSelected = true;
    //        }
    //    }
    //}

    //private void OnDragSelectedNode(Event e)
    //{
    //    if (draggingNode != null)
    //    {
    //        draggingNode.UpdatePosition(e.delta);
    //    }
    //}
}
