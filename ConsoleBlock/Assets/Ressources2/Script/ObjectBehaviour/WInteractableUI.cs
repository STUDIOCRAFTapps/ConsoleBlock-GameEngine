using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WInteractableUI : DefaultUI {

    public InputField nameInputField;

    override public void OpenUI () {
        nameInputField.text = Target.Name;
    }

    public void Save () {
        Target.Name = nameInputField.text;
    }
}
