using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
