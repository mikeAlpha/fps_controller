using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mikealpha
{
    public static class Math
    {
        public static Vector3 GetViewDir(float angleinView, Transform transform)
        {
            angleinView += transform.eulerAngles.y;
            Vector3 pos = new Vector3(Mathf.Sin(angleinView * Mathf.Deg2Rad), 0, Mathf.Cos(angleinView * Mathf.Deg2Rad));
            return pos;
        }
    }
}
