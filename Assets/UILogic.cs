using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILogic : MonoBehaviour
{
    UIBase ui;
    // Start is called before the first frame update
    void Start()
    {
        ui = GetComponent<UIBase>();
        var dialog = ui.GetObj("Ex_弹窗");
        dialog.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
