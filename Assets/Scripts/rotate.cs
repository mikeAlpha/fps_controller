using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
    void Update()
    {
        var rot = transform.rotation.eulerAngles;
        //rot.x = CalculateAngle(rot.x);
        //rot.y = CalculateAngle(rot.y);
        //rot.z = CalculateAngle(rot.z);

        Debug.Log(rot);
    }

    float CalculateAngle(float angle)
    {
        if (angle > 180) angle -= 360;
        return angle;
    }
}
