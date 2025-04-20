using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;
using mikealpha;

public class TreeNodeEditor : EditorWindow
{
    private const float panelWidth = 200f;
    private static List<TreeNode> nodes;
    private static List<Connection> connections;

    private Dictionary<string, List<string>> nodeTypes = new Dictionary<string, List<string>> {
        { "Composites", new List<string>() },
        { "Actions", new List<string> () },
        { "Conditions", new List<string>()}
    };


    private static TreeNode selectedInNode;
    private static TreeNode selectedOutNode;

    private Vector2 offset;
    private Vector2 drag;

    private static TreeNode dragFromNode;
    private static Vector2 dragToPosition;
    private static bool isDraggingConnection = false;
    private static bool isDraggingEditor = false;

    Rect panelRect;

    [MenuItem("Window/Behaviour Tree Designer")]
    private static void OpenWindow()
    {
        TreeNodeEditor window = GetWindow<TreeNodeEditor>();
        window.titleContent = new GUIContent("Behaviour Tree Designer");
        window.GetAllNodeData();
    }

    private void OnEnable()
    {
        nodes = new List<TreeNode>();
        connections = new List<Connection>();
    }

    private void GetAllNodeData()
    {
        var getCompositeNodeData = GetDerivedClassNames.GetDerivedClasses<Node>();
        foreach (var node in getCompositeNodeData)
        {
            if (node.Name == "Sequence" || node.Name == "Fallback" || node.Name == "Parallel")
                nodeTypes["Composites"].Add(node.Name);
        }

        var getConditionNodeData = GetDerivedClassNames.GetDerivedClasses<Condition>();
        foreach (var node in getConditionNodeData)
            nodeTypes["Conditions"].Add(node.Name);

        var getActionNodeData = GetDerivedClassNames.GetDerivedClasses<mikealpha.Action>();
        foreach (var node in getActionNodeData)
            nodeTypes["Actions"].Add(node.Name);
    }

    private void OnGUI()
    {
        DrawToolbar();
        DrawPanel();


        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawConnections();
        EvaluateGraph();

        DrawConnectionLine(Event.current);

        DrawNodes();


        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        if (GUI.changed) Repaint();
    }

    private void DrawPanel()
    {
        if(nodes.Count == 0) return;


        panelRect = new Rect(0, 0, panelWidth, position.height);
        GUI.Box(panelRect, "Actions");

        GUILayout.BeginArea(panelRect);
        GUILayout.Space(10);

        GUILayout.Label("");
        foreach (var node in nodeTypes)
        {
            EditorGUILayout.LabelField(node.Key.ToUpper());
            var nodeLists = node.Value;
            for (int i = 0; i < nodeLists.Count; i++)
            {
                if (GUILayout.Button(nodeLists[i]))
                {
                    try
                    {
                        Vector2 position = new Vector2(panelWidth + 10, panelRect.height / 2);

                        Assembly assembly = Assembly.Load("Assembly-CSharp");
                        Type type = assembly.GetType("mikealpha." + nodeLists[i]);

                        //object[] args = { null };
                        var InNode = true;
                        var OutNode = true;
                        if (type.BaseType.Name == "Action" || type.BaseType.Name == "Condition")
                        {
                            //args[0] = aiTree.transform;
                            InNode = true;
                            OutNode = false;
                        }

                        //Node nodeData = (Node)Activator.CreateInstance(type, args);
                        OnClickAddNode(position, nodeLists[i],InNode,OutNode);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
        }

        GUILayout.EndArea();
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset,
                             new Vector3(gridSpacing * i, position.height, 0f) + newOffset);

        for (int j = 0; j < heightDivs; j++)
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset,
                             new Vector3(position.width, gridSpacing * j, 0f) + newOffset);

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void DrawToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        if (nodes.Count == 0)
        {
            if (GUILayout.Button("Create", EditorStyles.toolbarButton))
            {
                Vector2 position = new Vector2(panelWidth + 10, panelRect.height / 2);
                OnClickAddNode(position,"RootNode",false,true);
            }
        }

        else {
            if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                SaveGraph();
        }

        if (GUILayout.Button("Load", EditorStyles.toolbarButton))
            LoadGraph();

        GUILayout.EndHorizontal();
    }

    private void SaveGraph()
    {
        var graph = new GraphData();

        foreach (var node in nodes)
        {
            graph.nodes.Add(new NodeData
            {
                id = node.id,
                title = node.title,
                position = node.rect.position,
                InNode = node.InNode,
                OutNode = node.OutNode,
            });
        }

        foreach (var conn in connections)
        {
            graph.connections.Add(new ConnectionData
            {
                fromNodeId = conn.fromNode.id,
                toNodeId = conn.toNode.id
            });
        }

        string json = JsonUtility.ToJson(graph, true);
        System.IO.File.WriteAllText(Application.dataPath + "/SavedNodeGraph.json", json);
        AssetDatabase.Refresh();
    }

    private void LoadGraph()
    {
        string path = Application.dataPath + "/SavedNodeGraph.json";
        if (!System.IO.File.Exists(path)) return;

        string json = System.IO.File.ReadAllText(path);
        var graph = JsonUtility.FromJson<GraphData>(json);

        nodes.Clear();
        connections.Clear();

        Dictionary<string, TreeNode> idToNode = new Dictionary<string, TreeNode>();

        foreach (var data in graph.nodes)
        {
            var node = new TreeNode(data.position, 100, 50, data.title,data.InNode,data.OutNode);
            node.id = data.id;
            node.OnRemoveNode = OnClickRemoveNode;
            nodes.Add(node);
            idToNode[data.id] = node;
        }

        foreach (var connData in graph.connections)
        {
            if (idToNode.TryGetValue(connData.fromNodeId, out var fromNode) &&
                idToNode.TryGetValue(connData.toNodeId, out var toNode))
            {
                connections.Add(new Connection(fromNode, toNode));
            }
        }
    }

    private void EvaluateGraph()
    {
        foreach (var conn in connections)
        {
            if (conn.fromNode != null && conn.toNode != null)
            {
                conn.toNode.inputValue = conn.fromNode.outputValue;
            }
        }
    }

    private void DrawNodes()
    {
        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                node.Draw();
            }
        }
    }

    private void DrawConnectionLine(Event e)
    {
        if (isDraggingConnection && dragFromNode != null)
        {
            Vector2 startPos = dragFromNode.OutPoint.center;
            Vector2 endPos = e.mousePosition;

            Handles.DrawBezier(
                startPos,
                endPos,
                startPos + Vector2.right * 5f,
                endPos + Vector2.left * 5f,
                Color.red,
                null,
                3f
            );

            GUI.changed = true;
        }
    }

    private void DrawConnections()
    {
        if (connections != null)
        {
            foreach (var con in connections)
            {
                con.Draw();
            }
        }
    }

    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        if (e.button == 0)
        {
            if(!CheckNodeStatus())
                OnDrag(e.delta);
        }

        if ((e.type == EventType.MouseDown && e.button == 1) ||
            (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape))
        {
            isDraggingConnection = false;
            dragFromNode = null;
        }

        if (e.type == EventType.MouseDown && e.button == 1)
        {
            bool clickedOnNode = false;
            foreach (var node in nodes)
            {
                if (node.rect.Contains(e.mousePosition))
                {
                    clickedOnNode = true;
                    break;
                }
            }

            if (!clickedOnNode)
            {
                ShowContextMenu(e.mousePosition);
                e.Use();
            }
        }

        foreach (var node in nodes)
        {
            if (node.ProcessEvents(e))
            {
                GUI.changed = true;
            }
        }
    }

    private bool CheckNodeStatus()
    {
        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                if (node.isDragged || node.isSelected)
                    return true;
            }
        }
        return false;
    }

    private void ProcessNodeEvents(Event e)
    {
        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                bool guiChanged = node.ProcessEvents(e);
                if (guiChanged) GUI.changed = true;
            }
        }
    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;
        
        if (nodes != null)
        {
            foreach (var node in nodes)
            {
                node.Drag(delta);
            }
        }

        GUI.changed = true;
    }

    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Add Node"), false, () => OnClickAddNode(mousePosition,"",true, true));
        menu.ShowAsContext();
    }

    private void OnClickAddNode(Vector2 mousePosition,string name, bool InNode, bool OutNode)
    {
        if (nodes == null) nodes = new List<TreeNode>();

        var node = new TreeNode(mousePosition, 100, 50, name, InNode,OutNode);
        node.OnRemoveNode = OnClickRemoveNode;
        nodes.Add(node);
    }

    private void OnClickRemoveNode(TreeNode node)
    {
        if (connections != null)
        {
            connections.RemoveAll(c => c.fromNode == node || c.toNode == node);
        }

        nodes.Remove(node);
    }

    public static void OnClickConnectionPoint(TreeNode node, ConnectionPointType type)
    {
        if (type == ConnectionPointType.Out)
        {
            dragFromNode = node;
            isDraggingConnection = true;
        }
        else if (type == ConnectionPointType.In && isDraggingConnection)
        {
            if (dragFromNode != node)
            {
                connections.Add(new Connection(dragFromNode, node));
            }

            isDraggingConnection = false;
            dragFromNode = null;
        }
    }
}