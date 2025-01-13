using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BehaviorTreeEditor : EditorWindow
{
    private const float panelWidth = 200f;
    private string[] nodeTypes = { "Composites" , "Decorators", "Actions" };
    private int selectedNodeType = 0;

    private List<BehaviourTreeNode> nodes = new List<BehaviourTreeNode>();
    private List<(BehaviourTreeNode,BehaviourTreeNode)> connections = new List<(BehaviourTreeNode,BehaviourTreeNode)>();

    private BehaviourTreeNode selectedNode;

    [MenuItem("MikeAlpha/AiBehaviorTree")]
    public static void ShowWindow() 
    {  
        GetWindow<BehaviorTreeEditor>(); 
    }

    private void OnGUI()
    {
        DrawPanel();

        GUI.BeginGroup(new Rect(panelWidth, 0, position.width - panelWidth, position.height));
        DrawNodes(Event.current);
        DrawConnections(Event.current);
        ProcessEvents(Event.current);
        GUI.EndGroup();

        if (GUI.changed)
            Repaint();

    }

    private void DrawNodes(Event e)
    {
        var boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.normal.background = MakeTexture(1, 1, Color.green);

        foreach (var node in nodes) 
        {
            Vector2 mousePos = e.mousePosition;
            if (node.InNode.Contains(mousePos))
            {
                node.Draw(boxStyle, null);
            }
            else if (node.OutNode.Contains(mousePos))
            {
                node.Draw(null, boxStyle);
            }
            else
            {
                node.Draw(null, null);
            }
        }
    }

    private Texture2D MakeTexture(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    private void DrawConnections(Event e)
    {
        if (e.type == EventType.Repaint && IsDraggingConnection)
        {
            Handles.DrawBezier(startPos, endPos, startPos + Vector3.right * 5, endPos + Vector3.left * 5, Color.red, null, 3f);
            TryConnectNodes(endPos);
        }
        
        foreach (var conn in connections)
        {
            DrawConnectionInternal(conn.Item1, conn.Item2);
        }

        SceneView.RepaintAll();
    }

    private void RemoveConnection(BehaviourTreeNode node)
    {
        for (int i = 0; i < connections.Count; i++)
        {
            var connection = connections[i];
            if(connection.Item2 == node)
            {
                connections.RemoveAt(i);
                Repaint();
                return;
            }
        }
    }

    private void DrawConnectionInternal(BehaviourTreeNode fromNode, BehaviourTreeNode toNode)
    {
        Vector3 start = new Vector3(fromNode.OutNode.center.x, fromNode.OutNode.center.y, 0);
        Vector3 end = new Vector3(toNode.InNode.center.x, toNode.InNode.center.y, 0);
        Handles.DrawBezier(start,end, start + Vector3.right * 5, end + Vector3.left * 5,Color.red,null,3f);
    }

    private Vector3 endPos;
    private void ProcessEvents(Event e)
    {
        switch (e.type)
        { 
            case EventType.MouseDown:
                if (e.button == 1)
                {
                    CreateContectMenu(e.mousePosition);
                }
                else if(e.button == 0)
                {
                    if(selectedNode != null)
                    {
                        //TryConnectNodes(e.mousePosition);
                    }
                    else
                    {
                        SelectNode(e.mousePosition);
                    }
                }
                break;
            case EventType.MouseDrag:
                if(e.button == 0)
                {
                    if(startPos != Vector3.zero)
                        endPos = e.mousePosition;
                    
                    OnDragSelectedNode(e);
                }
                break;
            case EventType.MouseUp:
                if(e.button == 0)
                {
                    selectedNode = null;
                    draggingNode = null;
                    IsDraggingConnection = false;
                }
                break;
        }

        Repaint();
    }

    private void DrawPanel()
    {
        Rect panelRect = new Rect(0, 0, panelWidth, position.height);
        GUI.Box(panelRect, "Actions");

        GUILayout.BeginArea(panelRect);
        GUILayout.Space(10);

        GUILayout.Label("Node Type:");
        selectedNodeType = EditorGUILayout.Popup(selectedNodeType, nodeTypes);

        if (GUILayout.Button("Create Node"))
        {
            Vector2 position = new Vector2(panelWidth + 10, panelRect.height / 2);
            CreateNode(position, nodeTypes[selectedNodeType]);
        }

        GUILayout.EndArea();
    }

    private void CreateNode(Vector2 position, string nodeType = "Default")
    {
        //BehaviourTreeNode newNode = new BehaviourTreeNode
        //{
        //    rect = new Rect(position.x, position.y, 150, 75),
        //    name = nodeType
        //};
        //nodes.Add(newNode);
    }

    private void CreateContectMenu(Vector2 mousePos)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Add Node"), false, () => {
            var node = new BehaviourTreeNode("Node " + nodes.Count, mousePos, null);
            nodes.Add(node);
        });
        menu.ShowAsContext();
    }

    private void TryConnectNodes(Vector2 mousePos)
    {
        foreach (var node in nodes) 
        {
            if (node.InNode.Contains(mousePos) && node != selectedNode)
            {
                connections.Add((selectedNode, node));
                selectedNode = null;
                IsDraggingConnection=false;
                break;
            }
        }
    }

    private BehaviourTreeNode draggingNode;
    private bool IsDraggingConnection = false;
    private Vector3 startPos;
    private void SelectNode(Vector2 mousePos)
    {
        foreach(var node in nodes)
        {
            if (node.rect.Contains(mousePos))
            {
                draggingNode = node;
                break;
            }

            if (node.InNode.Contains(mousePos))
            {
                RemoveConnection(node);
            }


            if (node.OutNode.Contains(mousePos) && !IsDraggingConnection)
            {
                IsDraggingConnection = true;
                startPos = mousePos;
                endPos = startPos;
                selectedNode = node;
            }
        }
    }

    private void OnDragSelectedNode(Event e)
    {
        if (draggingNode != null)
        {
            draggingNode.UpdatePosition(e.delta);
        }
    }
}
