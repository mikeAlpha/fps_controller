using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WeaponMenuEditor : EditorWindow
{
    [MenuItem("MikeAlpha/WeaponCreationTool")]
    public static void ShowWindow()
    {
        var wEditor = GetWindow<WeaponMenuEditor>();
    }


    private GameObject mWeapon;
    private Transform mWeaponMagTransform, mWeaponFireTransform, mRightHandIK, mLeftHandIK;
    //private BaseWeapon mPlayerWeapon;

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Attach Weapon Object:");
        mWeapon = EditorGUILayout.ObjectField(mWeapon, typeof(GameObject), false) as GameObject;

        if(mWeapon != null)
        {
            EditorGUILayout.ObjectField(mWeaponMagTransform, typeof(Transform), false);
            EditorGUILayout.ObjectField(mWeaponFireTransform, typeof(Transform), false);
            EditorGUILayout.ObjectField(mRightHandIK, typeof(Transform), false);
            EditorGUILayout.ObjectField(mLeftHandIK, typeof(Transform), false);
            //EditorGUILayout.ObjectField(mPlayerWeapon, typeof(BaseWeapon), false);
        }

        if (GUILayout.Button("Configure Weapon"))
        {
            GenerateTransforms();
        }
    }

    void GenerateTransforms()
    {
        GameObject wp = Instantiate(mWeapon);
        var wc = wp.AddComponent<WeaponController>();
        var mag_pos = new GameObject("mag_position");
        mag_pos.transform.SetParent(wp.transform);

        var fire_pos = new GameObject("fire_position");
        fire_pos.transform.SetParent(wp.transform);

        var right_pos = new GameObject("rightHandIK");
        right_pos.transform.SetParent(wp.transform);

        var left_pos = new GameObject("leftHandIK");
        left_pos.transform.SetParent(wp.transform);

        mWeaponFireTransform = fire_pos.transform;
        mWeaponMagTransform = mag_pos.transform;
        mRightHandIK = right_pos.transform;
        mLeftHandIK = left_pos.transform;

        wc.MagPoint = mag_pos.transform;
        wc.FirePoint = fire_pos.transform;
        wc.rightHandIK = right_pos.transform;
        wc.leftHandIK = left_pos.transform;
        //wc.weapon.DisplayObject = wp;
    }
}
