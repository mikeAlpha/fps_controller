using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DestroyAsset : MonoBehaviour
{
    [SerializeField] private float time = 0.2f;
    float timer = 0f;

    public string mtag;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > time)
        {
            gameObject.SetActive(false);
            PoolManager.Instance.ReturnObject(mtag, GetComponent<PoolableObject>());
            timer = 0f;
        }
        //Destroy(gameObject, time);
    }
}
