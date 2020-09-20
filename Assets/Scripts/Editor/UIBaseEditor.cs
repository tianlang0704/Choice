using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIBase))]
public class LuaInjectInspector : Editor
{

    protected UIBase comp;
    void OnEnable() {
        comp = target as UIBase;
    }

    public override void OnInspectorGUI()
    {
        if (comp == null) return;
        GUILayout.Label("注入节点", GUILayout.Width(100));

        // 循环显示已有注入
        for(int i = 0; i < comp.inj.Count; i++)
        {
            // 注入变量名
            UIBase.KVP entry = comp.inj[i];
            GUILayout.BeginHorizontal();
            entry.name = GUILayout.TextField(entry.name, GUILayout.Width(100));

            // 注入对象
            UnityEngine.Object obj = null;
            obj = EditorGUILayout.ObjectField(entry.value, typeof(UnityEngine.GameObject), true);
            if(GUI.changed)
            {
                if(obj != null && obj != entry.value)
                {
                    entry.value = obj as GameObject;
                }
                else if(obj == null && entry.value != null)
                {
                    entry.value = null;
                }
                if (string.IsNullOrEmpty(entry.name)) {
                    entry.name = "Ex_" + entry.value.name;
                }
                comp.inj[i] = entry;
            }

            // 删除注入项按钮
            if( GUILayout.Button("删", GUILayout.Width(20)))
            {
                comp.inj.RemoveAt(i);
                return;
            }
            GUILayout.EndHorizontal();
        }

        // 添加注入项按钮
        GUI.color = Color.magenta;
        GUILayout.Space(10);
        if (GUILayout.Button("添加"))
        {
            comp.inj.Add(new UIBase.KVP());
        }
        GUI.color = Color.white;

        // 记录操作
        Undo.RecordObject(comp, "StateChange");
    }    
}
