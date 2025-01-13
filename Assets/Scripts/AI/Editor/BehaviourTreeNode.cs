using System;
using UnityEngine;
using mikealpha;
using Unity.VisualScripting;

[Serializable]
public class BehaviourTreeNode
{
    public string name;
    public Rect rect;
    public Node node;
    public Rect InNode, OutNode;

    public BehaviourTreeNode(string name, Vector2 pos, Node node)
    {
        this.name = name;
        this.rect = new Rect(pos.x, pos.y, 150, 50);
        this.node = node;
        
        this.InNode = new Rect(this.rect.center.x, this.rect.yMin - 10, 10, 10);
        this.OutNode = new Rect(this.rect.center.x, this.rect.yMax, 10, 10);
    }

    public void Draw(GUIStyle InNodeStyle, GUIStyle OutNodeStyle)
    {
        GUI.Box(rect, name);

        if(InNodeStyle == null && OutNodeStyle != null)
        {
            GUI.Box(OutNode, string.Empty, OutNodeStyle);
            GUI.Box(InNode, string.Empty);
        }
        
        else if(OutNodeStyle == null && InNodeStyle != null)
        {
            GUI.Box(InNode, string.Empty, InNodeStyle);
            GUI.Box(OutNode, string.Empty);
        }
        else
        {
            GUI.Box(OutNode, string.Empty);
            GUI.Box(InNode, string.Empty);
        }
    }

    public void UpdatePosition(Vector2 delta)
    {
        this.rect.position += delta;
        this.InNode = new Rect(this.rect.center.x, this.rect.yMin - 10, 10, 10);
        this.OutNode = new Rect(this.rect.center.x, this.rect.yMax, 10, 10);
    }
}
