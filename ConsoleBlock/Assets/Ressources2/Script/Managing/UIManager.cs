using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public Widget[] widget;
    public DefaultUI[] UIComponents;
    public RectTransform PauseMenu;
    public Dropdown[] PauseMenuDropdowns;
    public Player player;

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

    public void CloseUI () {
        foreach(DefaultUI ui in UIComponents) {
            ui.gameObject.SetActive(false);
        }
    }

    public void OpenUI (string ID, WInteractable interactable) {
        for(int i = 0; i < UIComponents.Length; i++) {
            if(UIComponents[i].ID == ID) {
                UIComponents[i].manager = this;
                UIComponents[i].Target = interactable;
                UIComponents[i].gameObject.SetActive(true);
                UIComponents[i].OpenUI();
                break;
            }
        }
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

[Serializable]
public class BasicUIComponent {
    public string Title;
    public BasicUITabs[] uiTabs;
}

[Serializable]
public class BasicUITabs {
    public string Name;
    public BasicUITabType type;
    public UIPanel panel;
    public Button button;
}

public enum BasicUITabType {
    Button,
    TabPanelOpener
}

[Serializable]
public class UIInputs {
    public RectTransform input;
    public UIType type;

    public UIInputs (RectTransform input, UIType type) {
        this.input = input;
        this.type = type;
    }
}

public enum UIType {
    Button,
    InputField,
    NumberField,
    AutocompleteHelper
}