using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(AITree))]
public class FovEditor : Editor
{
    private void OnSceneGUI()
    {
        var ai_obj = target as AITree;

        Vector3 A = mikealpha.Math.GetViewDir(-ai_obj.ViewAngle / 2, ai_obj.transform);
        Vector3 B = mikealpha.Math.GetViewDir(ai_obj.ViewAngle / 2, ai_obj.transform);

        Handles.DrawLine(ai_obj.transform.position, ai_obj.transform.position + A * ai_obj.ViewRadius);
        Handles.DrawLine(ai_obj.transform.position, ai_obj.transform.position + B * ai_obj.ViewRadius);

        Handles.DrawWireCube(ai_obj.transform.position + A * ai_obj.ViewRadius, Vector3.one);
        Handles.DrawWireCube(ai_obj.transform.position + B * ai_obj.ViewRadius, Vector3.one);

        Handles.color = Color.red;
        for(int i = 0; i < ai_obj.targets.Count; i++)
        {
            Handles.DrawLine(ai_obj.transform.position, ai_obj.targets[i].position);
        }
    }
}
