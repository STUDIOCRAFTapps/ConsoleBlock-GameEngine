using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Widget[] widget;

    public void EditWidgetValue (string Id, int Offset) {
        for(int i = 0; i < widget.Length; i++) {
            if(widget[i].Id == Id) {
                widget[i].Value = (int)Mathf.Repeat(widget[i].Value + Offset, widget[i].MaxValue);
                UpdateWidget(i);
                return;
            }
        }
    }

    public int GetWidgetValue (string Id) {
        for(int i = 0; i < widget.Length; i++) {
            if(widget[i].Id == Id) {
                return widget[i].Value;
            }
        }
        return 0;
    }

    public void UpdateWidget (int Id) {
        widget[Id].Display.sprite = widget[Id].sprites[widget[Id].Value];
    }
    
}

[Serializable]
public class Widget {
    public string Id;
    public int Value;
    public int MaxValue;
    public Image Display;

    public Sprite[] sprites;
}
