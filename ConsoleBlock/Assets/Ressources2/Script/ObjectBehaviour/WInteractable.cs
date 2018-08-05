using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WInteractable : WObject {

    public List<Variable> GlobalVariable = new List<Variable>();
    public List<FunctionTemplate> GlobalFunctionsDictionnairy = new List<FunctionTemplate>();
    public List<FunctionCaller> FunctionCall = new List<FunctionCaller>();
    public WObjectTransmitter transmitter = new WObjectTransmitter();
    public WInteractable PowerSource = null;

    public BuildingBlock AnchorChild;

    [HideInInspector]
    public bool IsInteracting = false;
    public bool ComsumePPS = false;
    public bool GeneratePPS = false;
    public float PPSComsumption = 0.0f;
    public float PPSExpulsion = 0.0f;
    public bool InfinitePPSFilling = false;
    public float PPSPool = 0.0f;

    virtual public void OnInteraction (Player player) {
        IsInteracting = true;
        player.OpenUI();
        player.uiManager.OpenUI("Default Naming Editor", this);
    }

    virtual public void OnPointedAt (Player player) {

    }

    public bool CanExecuteCode () {
        if(ComsumePPS) {
            if(PowerSource != null) {
                if(PowerSource.PPSPool - PPSComsumption * Time.deltaTime >= 0f) {
                    PowerSource.PPSPool -= PPSComsumption * Time.deltaTime;
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        } else {
            return true;
        }
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
    public string Id;
    public VariableType variableType;

    public VariableTemplate (string Name, VariableType type) {
        this.Id = Name;
        this.variableType = type;
    }
}
