using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public abstract class EquippableItem : ScriptableObject, IInventoryItem
{

    public string ItemName {  get; set; }
    public string ItemDescription {  get; set; }
    public Sprite ItemIcon { get; set; }

    public GameObject DisplayObject;

    [HideInInspector]
    public List<string> boneNames = new List<string>();

    public void ConfigItem()
    {
        boneNames.Clear();

        if (DisplayObject.GetComponentInChildren<SkinnedMeshRenderer>() == null)
        {
            Debug.Log("Does not have SkinnedMeshRenderer");
            return;
        }

        var renderer = DisplayObject.GetComponentInChildren<SkinnedMeshRenderer>();
        var bones = renderer.bones;

        for (int i = 0; i < bones.Length; i++)
        {
            boneNames.Add(bones[i].name);
        }
    }

    public abstract void Use();
}
