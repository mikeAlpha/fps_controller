using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using mikealpha;

public class TreeNode
{
    public Rect rect;
    public string title;
    public bool isDragged;
    public bool isSelected;

    public Rect InPoint, OutPoint;

    public Node Node;

    public Action<TreeNode> OnRemoveNode;

    public List<Connection> connections = new List<Connection>();

    public string id = Guid.NewGuid().ToString();

    public string inputValue = "";
    public string outputValue = "";

    public bool InNode, OutNode;

    public TreeNode(Vector2 position, float width, float height, string title, bool InNode, bool OutNode)
    {
        rect = new Rect(position.x, position.y, width, height);
        this.title = title;
        this.InNode = InNode;
        this.OutNode = OutNode;
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    public void Draw()
    {
        GUI.Box(rect, title);

        if(InNode) 
            DrawInPoint();
        
        if(OutNode)
            DrawOutPoint();
    }

    public void DrawInPoint()
    {
        InPoint = new Rect(this.rect.center.x, this.rect.yMin - 10, 10, 10);
        
        GUI.Box(InPoint,"");

        if (GUI.Button(InPoint, "", GUIStyle.none))
        {
            TreeNodeEditor.OnClickConnectionPoint(this, ConnectionPointType.In);
        }
    }

    public void DrawOutPoint()
    {
        OutPoint = new Rect(this.rect.center.x, this.rect.yMax, 10, 10);

        GUI.Box(OutPoint, "");

        if (GUI.Button(OutPoint, "", GUIStyle.none))
        {
            TreeNodeEditor.OnClickConnectionPoint(this, ConnectionPointType.Out);
        }
    }

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                    }
                    else
                    {
                        GUI.changed = true;
                        isSelected = false;
                    }
                }

                if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                {
                    ShowContextMenu();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                isDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }

        return false;
    }

    private void ShowContextMenu()
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Delete Node"), false, () =>
        {
            OnRemoveNode?.Invoke(this);
        });
        menu.ShowAsContext();
    }
}
