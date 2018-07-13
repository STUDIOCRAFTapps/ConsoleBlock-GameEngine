using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrivingSeatScript : WInteractable {

    bool IsSitting = false;
    Player p;

    public override void OnInteraction (Player player) {
        IsSitting = true;
        p = player;
        p.playerRigidbody.isKinematic = true;
        p.playerRigidbody.detectCollisions = false;
        p.playerRigidbody.transform.parent = transform;
        p.playerRigidbody.transform.localPosition = Vector3.up * 1f;
        p.playerRigidbody.transform.localEulerAngles = Vector3.zero;
        p.controller.TargetCamXRot = -90;
        p.controller.CurrentCamYRot = p.controller.TargetCamXRot;
    }

    private void Update () {
        if(IsSitting) {
            if(!p.IsUICurrentlyOpened && InputControl.GetInputUp(InputControl.InputType.MouvementJump)) {
                p.playerRigidbody.isKinematic = false;
                p.playerRigidbody.detectCollisions = true;
                p.playerRigidbody.transform.parent = null;
                p.playerRigidbody.transform.localEulerAngles = Vector3.zero;
                p.controller.TargetCamXRot = transform.eulerAngles.y + 270;
                p.controller.CurrentCamYRot = p.controller.TargetCamXRot;
                IsSitting = false;
            } else {
                p.ForcedMouvementFreeze = true;
                p.ForcedCompleteMotionFreeze = true;
                p.playerRigidbody.transform.localPosition = Vector3.up * 0.85f;
            }
        }
    }
}
