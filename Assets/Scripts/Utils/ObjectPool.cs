using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : IPool where T : PoolableObject
{
    private GameObject objectPrefab;
    private Queue<T> objectPool;

    public ObjectPool(GameObject prefab, int initialPoolSize)
    {
        objectPrefab = prefab;
        objectPool = new Queue<T>();

        for (int i = 0; i < initialPoolSize; i++)
        {
            T obj = Object.Instantiate(objectPrefab).GetComponent<T>();
            obj.gameObject.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }

    public T GetObject()
    {

        if (objectPool.Count > 0)
        {
            T obj = objectPool.Dequeue();
            obj.gameObject.SetActive(true);
            obj.OnObjectReuse();

            return obj;
        }
        else
        {
            T obj = Object.Instantiate(objectPrefab).GetComponent<T>();

            return obj;
        }
    }

    public void ReturnObject(T obj)
    {


        if (obj != null)
        {
            obj.gameObject.SetActive(false);
            obj.transform.position = Vector3.up * 10000f;
            obj.transform.parent = null;
            objectPool.Enqueue(obj);
        }
    }
}
