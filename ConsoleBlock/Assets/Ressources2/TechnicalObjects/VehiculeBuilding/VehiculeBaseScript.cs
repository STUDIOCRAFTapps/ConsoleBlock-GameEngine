﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiculeBaseScript : WInteractable {

    public SpringJoint spring;
    public Transform CurrentAnchor;
    public Transform DetectedAnchor;
    int Rotation = 0;

	void Start () {
        GlobalVariable.Add(new Variable("CurrentRotaion", VariableType.v_int, 0, new VariableParameters(true, VariableAccessType.v_readonly)));
        GlobalVariable.Add(new Variable("HasAnchor", VariableType.v_bool, 0, new VariableParameters(true, VariableAccessType.v_readonly)));
        GlobalVariable.Add(new Variable("AnchorName", VariableType.v_bool, 0, new VariableParameters(true, VariableAccessType.v_readonly)));

        GlobalFunctionsDictionnairy.Add(new FunctionTemplate("GripNearestAnchor", new List<VariableTemplate>() {
        }));

        GlobalFunctionsDictionnairy.Add(new FunctionTemplate("UngripAnchor", new List<VariableTemplate>() {
        }));

        GlobalFunctionsDictionnairy.Add(new FunctionTemplate("SetRotation", new List<VariableTemplate>() {
            new VariableTemplate("RoundedAngle_90", VariableType.v_int)
        }));

    }

    override public void OnInteraction (Player player) {
        IsInteracting = true;
        player.OpenUI();
        player.uiManager.OpenUI("Vehicule Anchor Base", this);
    }

    override public void Update () {
        GlobalVariable[0].source = Rotation;
        GlobalVariable[1].source = CurrentAnchor != null;
        if(CurrentAnchor != null) {
            GlobalVariable[2].source = CurrentAnchor.GetComponentInChildren<WInteractable>().Name;
        } else {
            GlobalVariable[2].source = string.Empty;
        }

        for(int i = 0; i < FunctionCall.Count; i++) {
            FunctionCaller fc = FunctionCall[FunctionCall.Count - 1];
            FunctionCall.RemoveAt(0);
            int pr = i;
            i = 0;
            if(fc.Name == "GripNearestAnchor") {
                Undock();

                Dock(DetectedAnchor);
            } else if(fc.Name == "SetRotation") {
                SetRotation((int)fc.parameters[0].source);
            } else if(fc.Name == "UngripAnchor") {
                Undock();
            }
        }
    }

    public void Undock () {
        if(CurrentAnchor != null) {
            CurrentAnchor.parent.parent = null;
            CurrentAnchor.parent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            spring.connectedBody = null;
            CurrentAnchor = null;
        }
    }

    public void Dock (Transform Anchor) {
        CurrentAnchor = Anchor;
        spring.connectedBody = CurrentAnchor.parent.GetComponent<Rigidbody>();
        CurrentAnchor.parent.parent = transform;
        CurrentAnchor.parent.localEulerAngles = Vector3.zero;
        CurrentAnchor.parent.localPosition = Vector3.up * 3f;
        CurrentAnchor.parent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll ^ RigidbodyConstraints.FreezePositionY;
    }

    public void SetRotation (int Rot) {
        if(Rot <= -1) {
            Rotation = 3;
        } else if(Rot >= 4) {
            Rotation = 0;
        } else {
            Rotation = Rot;
        }
        if(CurrentAnchor != null) {
            CurrentAnchor.parent.eulerAngles = Vector3.up * Rotation * 90;
        }
        GlobalVariable[0].source = Rotation;
    }

    private void OnTriggerStay (Collider other) {
        if(other.name == "VehiculeAnchor") {
            DetectedAnchor = other.transform;
        }
    }

    private void OnTriggerExit (Collider other) {
        if(other.name == "VehiculeAnchor") {
            if(DetectedAnchor = other.transform) {
                DetectedAnchor = null;
            }
        }
    }
}