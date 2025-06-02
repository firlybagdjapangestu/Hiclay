using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    [System.Serializable]
    public struct PoolItem
    {
        public GameObject prefab;
        public int initialSize;
    }

    [Header("Pools To Initialize")]
    [SerializeField] private List<PoolItem> poolsToInitialize;

    private Dictionary<GameObject, Queue<GameObject>> m_objectPools = new Dictionary<GameObject, Queue<GameObject>>();

    private void Start()
    {
        foreach (var poolItem in poolsToInitialize)
        {
            CreatePool(poolItem.prefab, poolItem.initialSize);
        }
    }

    private void CreatePool(GameObject prefab, int initialSize)
    {
        if (!m_objectPools.ContainsKey(prefab))
        {
            m_objectPools[prefab] = new Queue<GameObject>();
            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.GetComponent<PrefabIdentifier>().SetPrefab(prefab); // Wajib ada PrefabIdentifier
                obj.SetActive(false);
                m_objectPools[prefab].Enqueue(obj);
            }
        }
    }

    public GameObject ActiveObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!m_objectPools.ContainsKey(prefab) || m_objectPools[prefab].Count == 0)
        {
            Debug.LogWarning("No available objects in pool for: " + prefab.name);
            return null;
        }

        GameObject obj = m_objectPools[prefab].Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);
        return obj;
    }

    public void DeactivateObject(GameObject obj)
    {
        var identifier = obj.GetComponent<PrefabIdentifier>();
        if (identifier == null)
        {
            Debug.LogWarning("Object doesn't have PrefabIdentifier: " + obj.name);
            return;
        }

        obj.SetActive(false);
        m_objectPools[identifier.prefab].Enqueue(obj);
    }
}
