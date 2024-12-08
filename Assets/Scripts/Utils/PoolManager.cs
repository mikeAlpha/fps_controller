using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    [Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<string, IPool> poolDictionary;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        poolDictionary = new Dictionary<string, IPool>();

        foreach (Pool pool in pools)
        {
            var objectPoolType = typeof(ObjectPool<>).MakeGenericType(pool.prefab.GetComponent<PoolableObject>().GetType());
            var objectPool = (IPool)Activator.CreateInstance(objectPoolType, pool.prefab, pool.size);
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public T GetObject<T>(string tag) where T : PoolableObject
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            return null;
        }

        var objectPool = poolDictionary[tag] as ObjectPool<T>;
        return objectPool.GetObject();
    }

    public void ReturnObject<T>(string tag, T obj) where T : PoolableObject
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            return;
        }

        var objectPool = poolDictionary[tag] as ObjectPool<T>;
        objectPool.ReturnObject(obj);

    }
}

public interface IPool { }