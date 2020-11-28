using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : SingletonBehaviour<ObjectPoolManager>
{
    public string poolGOName = "__Pool__";
    public GameObject poolObject = null;

    private Dictionary<string, List<GameObject>> poolTracker = new Dictionary<string, List<GameObject>>();
    private Dictionary<GameObject, string> outTracker = new Dictionary<GameObject, string>();
    // Start is called before the first frame update
    void Start()
    {
        if (poolObject == null) {
            poolObject = new GameObject(poolGOName);
        }
        DontDestroyOnLoad(poolObject);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 获取池子载体
    private GameObject GetPoolObject()
    {
        return poolObject;
    }

    // 获取一个池子
    private List<GameObject> GetPool(string path)
    {
        if (!poolTracker.ContainsKey(path)) {
            poolTracker[path] = new List<GameObject>();
        }
        return poolTracker[path];
    }

    public GameObject GetGameObject(string path)
    {
        // 尝试从池子中取出或者重新加载一个GO
        GameObject gameObject;
        var pool = GetPool(path);
        if (pool.Count > 0) {
            gameObject = pool[0];
            pool.RemoveAt(0);
        } else {
            var res = ResourceManager.Instance.LoadObject<GameObject>(path);
            gameObject = Instantiate(res);
        }
        // 放入借出列队
        outTracker[gameObject] = path;
        // 初始化
        gameObject.SetActive(true);
        return gameObject;
    }

    public T GetGameObject<T>(string path) where T:Component
    {
        return GetGameObject(path).GetComponent<T>();
    }

    public void RecycleGameObject(GameObject gameObject)
    {
        // 检查GO是否是借出的
        if (!outTracker.ContainsKey(gameObject)) {
            return;
        }
        // 从借出列队中取出
        var path = outTracker[gameObject];
        outTracker.Remove(gameObject);
        // 放回池子中
        gameObject.SetActive(false);
        GetPool(path).Add(gameObject);
        gameObject.transform.SetParent(GetPoolObject().transform);
    }
}
