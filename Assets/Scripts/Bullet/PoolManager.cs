using System.Collections.Generic;
using UnityEngine;

public enum PoolKey
{
    BULLET,
    EFFECT
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [System.Serializable]
    public struct PoolData
    {
        public PoolKey key;
        public Transform parentGroup;
        public GameObject prefab;
        public int count;
    }

    [SerializeField] private List<PoolData> _poolList = new();

    private Dictionary<PoolKey, Queue<GameObject>> _poolDictionary = new();
    private Dictionary<PoolKey, PoolData> _poolDataDictionary = new();
    private HashSet<GameObject> _pooledObjects = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        CreateAllPool();
    }

    private void CreateAllPool()
    {
        _poolDictionary.Clear();
        _poolDataDictionary.Clear();
        _pooledObjects.Clear();

        foreach (var data in _poolList)
        {
            if (data.prefab == null)
            {
                Debug.LogWarning($"{data.key} 프리팹이 비어 있습니다.");
                continue;
            }

            if (_poolDictionary.ContainsKey(data.key))
            {
                Debug.LogWarning($"{data.key} 가 중복 등록되어 있습니다.");
                continue;
            }

            Queue<GameObject> queue = new Queue<GameObject>();
            _poolDictionary.Add(data.key, queue);
            _poolDataDictionary.Add(data.key, data);

            for (int i = 0; i < data.count; i++)
            {
                GameObject obj = CreateNewObject(data);
                obj.SetActive(false);
                queue.Enqueue(obj);
                _pooledObjects.Add(obj);
            }
        }
    }

    private GameObject CreateNewObject(PoolData data)
    {
        GameObject obj = Instantiate(data.prefab, data.parentGroup);
        return obj;
    }

    public GameObject Get(PoolKey key)
    {
        if (_poolDictionary.ContainsKey(key) == false)
        {
            Debug.LogWarning($"{key} 가 적절하지 않습니다.");
            return null;
        }

        Queue<GameObject> queue = _poolDictionary[key];

        while (queue.Count > 0)
        {
            GameObject obj = queue.Dequeue();

            if (obj == null)
            {
                continue;
            }

            _pooledObjects.Remove(obj);
            obj.SetActive(true);
            return obj;
        }

        if (_poolDataDictionary.ContainsKey(key))
        {
            GameObject newObj = CreateNewObject(_poolDataDictionary[key]);
            newObj.SetActive(true);
            return newObj;
        }

        Debug.LogWarning($"{key} 에 해당하는 PoolData를 찾을 수 없습니다.");
        return null;
    }

    public void Return(GameObject obj, PoolKey key)
    {
        if (obj == null)
        {
            return;
        }

        if (_poolDictionary.ContainsKey(key) == false)
        {
            Debug.LogWarning($"{key} 가 적절하지 않습니다.");
            return;
        }

        if (_pooledObjects.Contains(obj))
        {
            return;
        }

        obj.SetActive(false);

        if (_poolDataDictionary.ContainsKey(key) && _poolDataDictionary[key].parentGroup != null)
        {
            obj.transform.SetParent(_poolDataDictionary[key].parentGroup);
        }

        _poolDictionary[key].Enqueue(obj);
        _pooledObjects.Add(obj);
    }
}