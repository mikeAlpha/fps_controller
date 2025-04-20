using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public abstract class EquippableItem : ScriptableObject, IInventoryItem
{

    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;

    public string ItemName {  
            get
            {
                return itemName;
            }
            set 
                { 
                itemName = value; 
            } 
        }
    public string ItemDescription { 
        get 
        {
            return itemDescription;
        }
        set 
        {
            itemDescription = value;
        } 
    }
    public Sprite ItemIcon {
        get
        {
            return itemIcon;
        }
        set
        {
            itemIcon = value;
        }
    }



    public GameObject DisplayObject;

    [HideInInspector]
    public List<string> boneNames = new List<string>();

    private void OnValidate()
    {
        //ConfigItem();
    }


    public void ConfigItem()
    {
        boneNames.Clear();

        if(DisplayObject == null)
        {
            Debug.LogWarning("DisplayObject is null. Please drop a skinnedmeshrenderer");
            return;
        }

        if (DisplayObject.GetComponentInChildren<SkinnedMeshRenderer>() == null)
        {
            Debug.Log("Does not have SkinnedMeshRenderer");
            return;
        }

        var renderer = DisplayObject.GetComponentInChildren<SkinnedMeshRenderer>();
        var bones = renderer.bones;

        for (int i = 0; i < bones.Length; i++)
        {
            //Debug.Log("Bones====" + bones[i].name.GetHashCode() + "====" + bones[i].name);
            boneNames.Add(bones[i].name);
        }
    }

    public abstract void Use();
}
