using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAsset : MonoBehaviour
{
    [SerializeField] private float time = 0.2f;

    void Start()
    {
        Destroy(gameObject, time);
    }
}
