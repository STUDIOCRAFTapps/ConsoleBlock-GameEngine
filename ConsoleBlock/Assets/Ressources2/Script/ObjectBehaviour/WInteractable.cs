using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WInteractable : WObject {

    public List<Variable> GlobalVariable = new List<Variable>();
    public List<FunctionTemplate> GlobalFunctionsDictionnairy = new List<FunctionTemplate>();
    public List<FunctionCaller> FunctionCall = new List<FunctionCaller>();
    public WObjectTransmitter transmitter = new WObjectTransmitter();
    [HideInInspector]
    public bool IsInteracting = false;

    virtual public void OnInteraction (Player player) {
        IsInteracting = true;
        player.OpenUI();
        player.uiManager.OpenUI("Default Naming Editor", this);
    }

    virtual public void OnPointedAt (Player player) {

    }
}

public class FunctionTemplate {
    public string Name;
    public List<VariableTemplate> Parameters;

    public FunctionTemplate (string Name, List<VariableTemplate> Parameters) {
        this.Name = Name;
        this.Parameters = Parameters;
    }
}

public class VariableTemplate {
    public string Name;
    public VariableType type;

    public VariableTemplate (string Name, VariableType type) {
        this.Name = Name;
        this.type = type;
    }
}
