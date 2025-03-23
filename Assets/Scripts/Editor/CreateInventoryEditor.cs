using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EquippableItem))]
public class CreateInventoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var item = (EquippableItem)target;

        if(GUILayout.Button("Config Item"))
        {
            item.ConfigItem();
        }

        base.OnInspectorGUI();
    }
}
