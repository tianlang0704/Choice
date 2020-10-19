using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public static class MonoBehaviourExtension {
    public static GameObject i(this MonoBehaviour mb, string key) {
        return mb.gameObject.i(key);
    }

    public static T i<T>(this MonoBehaviour mb, string key) where T : class {
        return mb.gameObject.i<T>(key);
    }
}

public static class GameObjectExtension {
    public static GameObject i(this GameObject gameObject, string key) {
        return i<GameObject>(gameObject, key);
    }
    public static T i<T>(this GameObject gameObject, string key) where T : class {
        var uiBase = gameObject.GetComponent<UIBase>();
        if (uiBase == null) return null;
        var go = uiBase[key];
        if (typeof(T) == typeof(GameObject)) {
            return go as T;
        } else {
            return go.GetComponent<T>();
        }
    }
    public static void lc(this GameObject gameObject, UIBase.LifeCycle lc, Action cb, string key = null) {
        var uiBase = gameObject.GetComponent<UIBase>();
        if (uiBase == null) return;
        uiBase[lc, key] = cb;
    }
 }

public class UIBase : MonoBehaviour
{
    [System.Serializable]
    public struct KVP {
        public string name;
        public GameObject value;
    }
    public List<KVP> inj = new List<KVP>();
    Dictionary<string, GameObject> injectCache;
    public GameObject GetObj(string key) {
        if (injectCache == null) {
            injectCache = inj
                .ToLookup((entry)=>entry.name)
                .ToDictionary((entry)=>entry.Key, (entry)=>entry.First().value);
        }
        if (injectCache.ContainsKey(key)) {
            return injectCache[key];
        }
        return null;
    }
    public GameObject this[string key]{
        get { return GetObj(key); }
    }
    public T i<T>(string key) where T : class {
        var go = GetObj(key);
        var comp = go.GetComponent<T>();
        return comp;
    }
    public Action this[LifeCycle lc, string key = null] {
        set {
            lifeCycleCallback[lc].Add(new ActionInfo() {cb = value, key = key});
        }
    }

    public enum LifeCycle {
        OnEnable,
        OnStart,
        OnDisable,
        OnDistroy,
    }

    struct ActionInfo {
        public Action cb;
        public string key;
    }
    Dictionary<LifeCycle, List<ActionInfo>> lifeCycleCallback = new Dictionary<LifeCycle, List<ActionInfo>>() {
        {LifeCycle.OnEnable, new List<ActionInfo>()},
        {LifeCycle.OnStart, new List<ActionInfo>()},
        {LifeCycle.OnDisable, new List<ActionInfo>()},
        {LifeCycle.OnDistroy, new List<ActionInfo>()},
    };
    void ClearLifeCycleCallbacks(LifeCycle lc, string key = null) {
        var cbList = lifeCycleCallback[lc];
        cbList.RemoveAll((actionInfo) => key == null ? true : actionInfo.key == key);
    }
    void CallLifeCycleCallbacks(LifeCycle lc) {
        var cbInfoList = lifeCycleCallback[lc];
        foreach (var cbInfo in cbInfoList) {
            if (cbInfo.cb != null) 
                cbInfo.cb();
        }
    }
    void OnAwake()
    {
        
    }

    void OnEnable()
    {
        CallLifeCycleCallbacks(LifeCycle.OnEnable);
    }
    // Start is called before the first frame update
    void Start()
    {
        CallLifeCycleCallbacks(LifeCycle.OnStart);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDisable()
    {
        CallLifeCycleCallbacks(LifeCycle.OnDisable);
    }

    void OnDistroy()
    {
        CallLifeCycleCallbacks(LifeCycle.OnDistroy);

    }
}
