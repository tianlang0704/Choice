using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : SingletonBehaviour<ResourceManager>
{
    Dictionary<string, List<GameObject>> pool = new Dictionary<string, List<GameObject>>();
    Dictionary<string, int> refCount = new Dictionary<string, int>();
     
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public T LoadObject<T>(string path) where T:class
    {
        var obj = Resources.Load(path);
        return obj as T;
    }
}
