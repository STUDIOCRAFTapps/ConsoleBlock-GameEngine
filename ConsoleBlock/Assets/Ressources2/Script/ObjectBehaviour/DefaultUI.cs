using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultUI : MonoBehaviour {

    public string ID;
    public WInteractable Target;
    public UIManager manager;

    virtual public void CloseUI () {
        manager.CloseUI();
        manager.player.CloseUI();
    }

    virtual public void OpenUI () {

    }
}
