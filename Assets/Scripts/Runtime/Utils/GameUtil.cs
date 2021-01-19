using UnityEngine;
 using System;
 using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public static class GameUtil {
    static public Type GetStaticType<T>(T v)
    {
        return typeof(T);
    }

    static public Type GetElementTaype<T>(T v)
    {
        var type = GetStaticType(v);
        if (type.IsArray) {
            type = type.GetElementType();
        }
        return type;
    }

    static public K RandomWeightKey<K,V>(Dictionary<K,V> table)
    {
        K resKey = default;
        if (typeof(V) == typeof(float)) {
            var tableP = table as Dictionary<K,float>;
            var weightSum = tableP.Values.Sum();
            var random = Random.Range(0, weightSum);
            List<K> keyList = tableP.Keys.ToList();
            foreach (var key in keyList) {
                random -= tableP[key];
                if (random <= 0) {
                    resKey = key;
                    break;
                }
            }
        } else if (typeof(V) == typeof(int)) {
            var tableP = table as Dictionary<K,int>;
            var weightSum = tableP.Values.Sum();
            var random = Random.Range(0, weightSum);
            List<K> keyList = tableP.Keys.ToList();
            foreach (var key in keyList) {
                random -= tableP[key];
                if (random <= 0) {
                    resKey = key;
                    break;
                }
            }
        }
        return resKey;
    }

    static public int RandomWeightIndex<T>(List<T> table)
    {
        int resKey = default;
        if (typeof(T) == typeof(float)) {
            var tableP = table as List<float>;
            var weightSum = tableP.Sum();
            var random = Random.Range(0, weightSum);
            for (int i = 0; i < tableP.Count; i++){
                random -= tableP[i];
                if (random <= 0) {
                    resKey = i;
                    break;
                }
            }
        } else if (typeof(T) == typeof(int)) {
            var tableP = table as List<int>;
            var weightSum = tableP.Sum();
            var random = Random.Range(0, weightSum);
            for (int i = 0; i < tableP.Count; i++){
                random -= tableP[i];
                if (random <= 0) {
                    resKey = i;
                    break;
                }
            }
        }
        return resKey;
    }
    
    static public void ApplyDicWeight<K>(Dictionary<K,float> source, Dictionary<K,float> inf, bool isSet)
    {
        // 循环加值
        foreach (var newKvp in inf)
        {
            // 找旧值
            float value;
            if (source.ContainsKey(newKvp.Key)){
                value = source[newKvp.Key];
            } else {
                value = default;
            }
            // 改变值
            if (isSet) {
                value = newKvp.Value;
            } else {
                value += newKvp.Value;
            }
            // 设置回表
            source[newKvp.Key] = value;
        }
    }
 }