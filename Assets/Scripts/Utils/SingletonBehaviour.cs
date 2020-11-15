using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T: SingletonBehaviour<T>
{
    private static T instance = null;
    public static T Instance { 
        get {
            return instance == null ? (instance = (new GameObject("单例宿主")).AddComponent<T>()) : instance;
        } 
        protected set {
            instance = value;
        } 
    }
 
    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            throw new System.Exception("单例已存在, 请勿添加多个.");
        }
        else
        {
            instance = (T)this;
        }
    }
}