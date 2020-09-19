using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class SpriteRendererToUIImage : MonoBehaviour
{

    [MenuItem("GameObject/SpriteToUIImage", false, 0)]
    static void SpriteToUIImage() {
        var go = Selection.activeGameObject;
        RecursiveSpriteToUIImage(go);
    }

    static void RecursiveSpriteToUIImage(GameObject go) {
        // 转换自己
        if (go == null) return;
        var spriteRenderer = go.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) {
            var img = go.AddComponent<Image>();
            img.sprite = spriteRenderer.sprite;
            img.SetNativeSize();
            GameObject.DestroyImmediate(spriteRenderer);
        }
        var rectT = go.GetComponent<RectTransform>();
        if (rectT == null) {
            go.transform.localScale = Vector3.one;
            EditorUtility.SetDirty(go.transform);
            rectT = go.AddComponent<RectTransform>();
            // rectT.localScale = Vector3.one;
        }
        // 回溯转换子项
        foreach (var transform in go.transform) {
            var t = transform as Transform;
            var childGO = t.gameObject;
            RecursiveSpriteToUIImage(childGO);
        }
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
