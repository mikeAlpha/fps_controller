using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateWeaponEditor : EditorWindow
{
    [MenuItem("MikeAlpha/WeaponCreationTool")]
    public static void ShowWindow()
    {
        var wEditor = EditorWindow.GetWindow<CreateWeaponEditor>();
    }


    private GameObject mWeapon;
    private Transform mWeaponMagTransform, mWeaponFireTransform;

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Attach Weapon Object:");
        mWeapon = EditorGUILayout.ObjectField(mWeapon, typeof(GameObject), false) as GameObject;

        if(mWeapon != null)
        {
            EditorGUILayout.ObjectField(mWeaponMagTransform, typeof(GameObject), false);
            EditorGUILayout.ObjectField(mWeaponFireTransform, typeof(Transform), false);
        }

        if (GUILayout.Button("Configure Weapon"))
        {
            GenerateTransforms();
        }
    }

    void GenerateTransforms()
    {
        GameObject wp = Instantiate(mWeapon);
        wp.AddComponent<PlayerWeapon>();
        var mag_pos = new GameObject("mag_position");
        mag_pos.transform.SetParent(wp.transform);

        var fire_pos = new GameObject("fire_position");
        fire_pos.transform.SetParent(wp.transform);

        mWeaponFireTransform = fire_pos.transform;
        mWeaponMagTransform = mag_pos.transform;
    }
}
