﻿using UnityEngine;
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

    static public V RandomRemoveFromList<V>(List<V> list)
    {
        if (list.Count <= 0) return default;
        var v = list[Random.Range(0, list.Count)];
        list.Remove(v);
        return v;
    }
    
    static public List<V> RandomRemoveFromList<V>(List<V> list, int num, Func<V,V> modifier = null)
    {
        var res = new List<V>();
        for (int i = 0; i < num; i++) {
            if (list.Count <= 0) break;
            var v = RandomRemoveFromList(list);
            if (modifier != null) {
                v = modifier(v);
            }
            res.Add(v);
        }
        return res;
    }

    static public void ApplyListDicAttr<K,V>(Attr baseAttr, AttrInfluence influence)
    {
        ApplyDicAttr<K, List<V>>(baseAttr, influence, (a, b) => {
            if (a == null) a = new List<V>();
            a.AddRange(b); 
            return a; 
        });
    }
    static public void ApplyFloatDicAttr<K>(Attr baseAttr, AttrInfluence influence)
    {
        ApplyDicAttr<K, float>(baseAttr, influence, (a, b) => a + b);
    }
    static public void ApplyDicAttr<K, V>(Attr baseAttr, AttrInfluence influence, Func<V,V,V> addFunc = null)
    {
        if (influence.Attr.Type != Attr.DataType.CUSTOM) return;
        // 获取新表
        var newAttr = influence.Attr;
        var newWeights = newAttr.GetValue<Dictionary<K, V>>();
        if (newWeights == null) return;
        // 从属性中获取现在表
        var curWeights = baseAttr.GetValue<Dictionary<K, V>>();
        if (curWeights == null) {
            curWeights = new Dictionary<K, V>();
            baseAttr.SetValue(curWeights);
        }
        // 循环加值
        foreach (var newKvp in newWeights)
        {
            // 找旧值
            V value;
            if (curWeights.ContainsKey(newKvp.Key)){
                value = curWeights[newKvp.Key];
            } else {
                value = default;
            }
            // 改变值
            if (influence.IsSet) {
                value = newKvp.Value;
            } else {
                value = addFunc(value, newKvp.Value);
            }
            // 设置回表
            curWeights[newKvp.Key] = value;
        }
    }

    static public void ApplyListAttr<V>(Attr baseAttr, AttrInfluence influence)
    {
        if (influence.Attr.Type != Attr.DataType.CUSTOM) return;
        
        // 应用对应值
        if (influence.IsSet) {
            baseAttr.SetValue(influence.Attr.GetValue<List<V>>());
        } else {
            // 获取新表
            var newAttr = influence.Attr;
            var newList = newAttr.GetValue<List<V>>();
            if (newList == null || newList.Count <= 0) return;
            // 从属性中获取现在表
            var baseList = baseAttr.GetValue<List<V>>();
            if (baseList == null) {
                baseList = new List<V>();
                baseAttr.SetValue(baseList);
            }
            baseList.AddRange(newList);
        }
    }

    static public int CardId(int rawId)
    {
        return (int)ProfileIdBase.Card + rawId;
    }

    static public int ItemId(int rawId)
    {
        return (int)ProfileIdBase.Item + rawId;
    }

    static public Color ItemQualityToColor(ItemQuality quality)
    {
        return Color.white;
    }

    static public Color CardQualityToColor(CardQuality quality)
    {
        Color res = Color.white;
        switch (quality)
        {
            case CardQuality.Red:
            res = Color.red;
            break;
            case CardQuality.White:
            res = Color.white;
            break;
            case CardQuality.Green:
            res = Color.green;
            break;
            case CardQuality.Blue:
            // res = Color.blue;
            res = new Color(0.2f, 0.5f, 1f);
            break;
            case CardQuality.Purple:
            res = new Color(1f, 0.3f, 1f);
            break;
            case CardQuality.Gold:
            res = Color.yellow;
            break;
            default:
            break;
        }
        return res;
    }

    static public Vector2 ScreenToDesignFactor()
    {
        var designRes = new Vector2(1334, 750);
        var factor = new Vector2(designRes.x / Screen.width, designRes.y / Screen.height);
        return factor;
    }
 }