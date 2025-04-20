using UnityEditor;
using UnityEngine;

public enum ConnectionPointType { In, Out }

public class Connection
{
    public TreeNode fromNode;
    public TreeNode toNode;

    public Connection(TreeNode from, TreeNode to)
    {
        fromNode = from;
        toNode = to;
    }

    public void Draw()
    {
        Handles.DrawBezier(
            fromNode.OutPoint.center,
            toNode.InPoint.center,
            fromNode.OutPoint.center + Vector2.right * 5f,
            toNode.InPoint.center + Vector2.left * 5f,
            Color.green,
            null,
            3f
        );
    }
}