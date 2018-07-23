using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Rigidbody MainRigidbody;
    public Camera MainCamera;
    public Transform Head;
    public HingeJoint HeadXBoobing;
    public HingeJoint HeadZBoobing;
    public Transform Body;
    public MouvementParameters mouvementParameters;
    public CameraParameters cameraParameters;

    public bool FreezeMouvement = false;
    public bool FreezeCamera = false;

    public float TargetCamYRot = 0f;
    public float TargetCamXRot = 0f;
    public float CurrentCamXRot = 0f;
    public float CurrentCamYRot = 0f;
    Vector3 MoveTo = Vector3.zero;

    Transform ParentStick;
    Transform OldParentStick;
    Vector3 OldBodyPosition;
    Vector3 OldPosition;
    Vector3 OldParentRelativePosition;
    Vector3 OldEulerAngles;
    Vector3 OldVelocity;
    bool LastFrameGrounded = false;

    void FixedUpdate () {
        OldParentStick = ParentStick;

        float t = 1f;
        RaycastHit hit;
        RaycastHit hitHandGrab;
        RaycastHit stepJump;
        bool GroundDetected = Physics.Raycast(Body.TransformPoint(new Vector3(0f, -0.74f, 0f)), Body.TransformDirection(new Vector3(0f, -1f, 0f)), out hit);
        bool LedgeDetected = Physics.Raycast(Body.TransformPoint(new Vector3(0f, 1.15f, 0.75f)), Body.TransformDirection(new Vector3(0f, -1f, 0f)), out hitHandGrab);
        hit.distance -= 0.4f;
        if(transform.parent != null) {
            GroundDetected = false;
            LedgeDetected = false;
        }

        if(!GroundDetected || hit.distance >= 0.37f) {
            ParentStick = null;
            if(MainRigidbody.velocity.y > 0) {
                MainRigidbody.velocity = new Vector3(
                    MainRigidbody.velocity.x * mouvementParameters.ArealFriction,
                    MainRigidbody.velocity.y * mouvementParameters.VerticalArealFriction,
                    MainRigidbody.velocity.z * mouvementParameters.ArealFriction
                );
            } else {
                MainRigidbody.velocity = new Vector3(
                    MainRigidbody.velocity.x * mouvementParameters.ArealFriction,
                    MainRigidbody.velocity.y,
                    MainRigidbody.velocity.z * mouvementParameters.ArealFriction
                );
            }
        } else if(GroundDetected && hit.distance < 0.37f) {
            if(hit.collider.name != "WCollider") {
                ParentStick = hit.collider.transform;
            }
            MainRigidbody.velocity = new Vector3(MainRigidbody.velocity.x * mouvementParameters.GroundSlowDown, MainRigidbody.velocity.y, MainRigidbody.velocity.z * mouvementParameters.GroundSlowDown);
        }

        MoveTo = Vector3.zero;
        Vector2 MoveToSquare = Vector2.zero;
        if(InputControl.GetInput(InputControl.InputType.MouvementFoward) && !FreezeMouvement) {
            MoveTo += Body.forward;
            MoveToSquare.y += 1;
        }
        if(InputControl.GetInput(InputControl.InputType.MouvementBackward) && !FreezeMouvement) {
            MoveTo += -Body.forward;
            MoveToSquare.y += -1;
        }
        if(InputControl.GetInput(InputControl.InputType.MouvementLeft) && !FreezeMouvement) {
            MoveTo += -Body.right;
            MoveToSquare.x += 1;
            HeadZBoobing.connectedBody.angularVelocity += -Body.forward * 0.003f;
        }
        if(InputControl.GetInput(InputControl.InputType.MouvementRight) && !FreezeMouvement) {
            MoveTo += Body.right;
            MoveToSquare.x += 1;
            HeadZBoobing.connectedBody.angularVelocity += Body.forward * 0.003f;
        }
        bool StepDetected = false;
        if(GroundDetected && hit.distance < 0.27) {
            StepDetected = Physics.Raycast(Body.TransformPoint(new Vector3(MoveToSquare.x * 0.61f, (1f - hit.normal.y) * 0.5f, MoveToSquare.y * 0.61f)), Body.TransformDirection(new Vector3(0f, -1f, 0f)), out stepJump);
            if(StepDetected && stepJump.distance < 1.05f && MoveTo != Vector3.zero) {
                //MainRigidbody.velocity += Vector3.up * (4f * (1.1f-stepJump.distance) + 2f);
                //Body.position += Vector3.up * ((1.05f - stepJump.distance) * 1.0f + 0.3f);
                //Body.position += MoveTo * 0.07f;
                MainRigidbody.velocity += MoveTo * 2f;
                MainRigidbody.velocity = new Vector3(MainRigidbody.velocity.x, Mathf.Max((1.05f - stepJump.distance) * 10f + 5.5f, MainRigidbody.velocity.y), MainRigidbody.velocity.z);
            }
        }

        if(GroundDetected && hit.distance < 0.27f) {
            Accelerate(MoveTo.normalized, Vector3.one * mouvementParameters.MouvementAcceleration, new Vector3(1, 0, 1) * mouvementParameters.MaxSpeed);
        } else {
            Accelerate(MoveTo.normalized, Vector3.one * mouvementParameters.ArealMouvementAcceleration, new Vector3(1, 0, 1) * mouvementParameters.MaxSpeed * 0.9f);
        }
        if(GroundDetected && hit.distance < 0.27f && InputControl.GetInput(InputControl.InputType.MouvementJump) && !FreezeMouvement) {
            MainRigidbody.velocity = new Vector3(MainRigidbody.velocity.x + hit.normal.x * mouvementParameters.JumpForce, Mathf.Max(MainRigidbody.velocity.y, mouvementParameters.JumpForce) * hit.normal.y, MainRigidbody.velocity.z + hit.normal.z * mouvementParameters.JumpForce);
        }
        if(LedgeDetected && hitHandGrab.distance < 0.37f) {
            if(InputControl.GetInput(InputControl.InputType.MouvementJump) && !FreezeMouvement) {
                if(MainRigidbody.velocity.y < mouvementParameters.LedgeGrabIncrementation) {
                    MainRigidbody.velocity += Vector3.up * mouvementParameters.LedgeGrabIncrementation;
                    MainRigidbody.velocity = new Vector3(MainRigidbody.velocity.x, Mathf.Max(MainRigidbody.velocity.y, 0f), MainRigidbody.velocity.z);
                }
            }
        }
        if(LedgeDetected && hitHandGrab.distance < 0.45f) {
            if(ParentStick == null) {
                ParentStick = hitHandGrab.collider.transform;
            }
            if(InputControl.GetInputUp(InputControl.InputType.MouvementJump) && (MainRigidbody.velocity.y > -mouvementParameters.LedgeGrabMaxForce && MainRigidbody.velocity.y < mouvementParameters.LedgeGrabMaxForce + mouvementParameters.LedgeGrabIncrementation) && !FreezeMouvement) {
                MainRigidbody.velocity += Vector3.up * mouvementParameters.LedgeGrabBoost;
            }
        }
        if(MainRigidbody.velocity.y < 0) {
            MainRigidbody.velocity = new Vector3(MainRigidbody.velocity.x, Mathf.Clamp(MainRigidbody.velocity.y * mouvementParameters.GravityBoost, -40f, 0f), MainRigidbody.velocity.z);
        }
        MainRigidbody.velocity = new Vector3(MainRigidbody.velocity.x, Mathf.Clamp(MainRigidbody.velocity.y - mouvementParameters.PermaGravityBoost, -40f, Mathf.Infinity), MainRigidbody.velocity.z);
        

        if(ParentStick != null) {
            /*if(ParentStick.root.GetComponent<Rigidbody>() != null) {
                Rigidbody parentbody = ParentStick.root.GetComponent<Rigidbody>();
                Accelerate(parentbody.velocity, parentbody.velocity, parentbody.velocity);
                AccelerateAngle(parentbody.angularVelocity, parentbody.angularVelocity, parentbody.angularVelocity);
            }*/
            if(OldParentStick != null && OldParentStick == ParentStick) {
                if(OldParentRelativePosition != ParentStick.InverseTransformPoint(OldBodyPosition)) {
                    Vector3 mi = Body.position - OldBodyPosition;
                    Body.position -= mi;
                    Body.position = ParentStick.TransformPoint(OldParentRelativePosition);
                    Body.position += mi;
                }
                if(OldEulerAngles != ParentStick.eulerAngles) {
                    Vector3 vectorA = ParentStick.forward;
                    vectorA = new Vector3(vectorA.x, 0f, vectorA.z).normalized;
                    Vector3 vectorB = OldEulerAngles;
                    vectorB = new Vector3(vectorB.x, 0f, vectorB.z).normalized;
                    TargetCamXRot -= AngleSigned(vectorA, vectorB, Vector3.up);//AngleOffAroundAxis(Quaternion.Euler(ParentStick.eulerAngles - OldEulerAngles) * Vector3.up, ParentStick.up, Vector3.up) * Mathf.Rad2Deg;//ParentStick.eulerAngles.y - OldEulerAngles.y;
                    CurrentCamYRot = TargetCamXRot;
                }
            }
        }

        if(!LastFrameGrounded && GroundDetected && hit.distance < 0.27f) {
            HeadXBoobing.connectedBody.angularVelocity = Body.right * -OldVelocity.y * 0.1f;
        }

        if(ParentStick != null) {
            OldPosition = ParentStick.position;
            OldEulerAngles = ParentStick.forward;//ParentStick.eulerAngles;
            OldParentRelativePosition = ParentStick.InverseTransformPoint(Body.position);
            OldBodyPosition = Body.position;
        }
        OldVelocity = MainRigidbody.velocity;
        LastFrameGrounded = GroundDetected && hit.distance < 0.27f;
    }

    public static float AngleSigned (Vector3 v1, Vector3 v2, Vector3 n) {
        return Mathf.Atan2(
            Vector3.Dot(n, Vector3.Cross(v1, v2)),
            Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }

    /*void LateUpdate () {
        OldParentStick = ParentStick;
        DeltaReforming = Time.deltaTime*170f;

        float t = 1f;
        RaycastHit hit;
        RaycastHit hitHandGrab;
        bool GroundDetected = Physics.Raycast(Body.TransformPoint(new Vector3(0f, -0.74f, 0f)), Body.TransformDirection(new Vector3(0f, -1f, 0f)), out hit);
        bool LedgeDetected = Physics.Raycast(Body.TransformPoint(new Vector3(0f, 1.15f, 0.75f)), Body.TransformDirection(new Vector3(0f, -1f, 0f)), out hitHandGrab);
        hit.distance -= 0.4f;
        if(transform.parent != null) {
            GroundDetected = false;
            LedgeDetected = false;
        }

        if(!GroundDetected || hit.distance >= 0.37f) {
            ParentStick = null;
            if(MainRigidbody.velocity.y > 0) {
                MainRigidbody.velocity = new Vector3(
                    MainRigidbody.velocity.x * Mathf.Pow(mouvementParameters.ArealFriction, DeltaReforming),
                    MainRigidbody.velocity.y * Mathf.Pow(mouvementParameters.VerticalArealFriction, DeltaReforming),
                    MainRigidbody.velocity.z * Mathf.Pow(mouvementParameters.ArealFriction, DeltaReforming)
                );
            } else {
                MainRigidbody.velocity = new Vector3(
                    MainRigidbody.velocity.x * Mathf.Pow(mouvementParameters.ArealFriction, DeltaReforming),
                    MainRigidbody.velocity.y,
                    MainRigidbody.velocity.z * Mathf.Pow(mouvementParameters.ArealFriction, DeltaReforming)
                );
            }
        } else if(GroundDetected && hit.distance < 0.37f) {
            ParentStick = hit.collider.transform;
            MainRigidbody.velocity = new Vector3(MainRigidbody.velocity.x * Mathf.Pow(mouvementParameters.GroundSlowDown, DeltaReforming), MainRigidbody.velocity.y, MainRigidbody.velocity.z * Mathf.Pow(mouvementParameters.GroundSlowDown, DeltaReforming));
        }

        MoveTo = Vector3.zero;
        if(InputControl.GetInput(InputControl.InputType.MouvementFoward) && !FreezeMouvement) {
            MoveTo += Body.forward;
        }
        if(InputControl.GetInput(InputControl.InputType.MouvementBackward) && !FreezeMouvement) {
            MoveTo += -Body.forward;
        }
        if(InputControl.GetInput(InputControl.InputType.MouvementLeft) && !FreezeMouvement) {
            MoveTo += -Body.right;
            HeadZBoobing.connectedBody.angularVelocity += -Body.forward * 0.003f;
        }
        if(InputControl.GetInput(InputControl.InputType.MouvementRight) && !FreezeMouvement) {
            MoveTo += Body.right;
            HeadZBoobing.connectedBody.angularVelocity += Body.forward * 0.003f;
        }
        if(GroundDetected && hit.distance < 0.27f) {
            Accelerate(MoveTo.normalized, Vector3.one * Mathf.Pow(mouvementParameters.MouvementAcceleration, DeltaReforming), new Vector3(1, 0, 1) * mouvementParameters.MaxSpeed);
        } else {
            Accelerate(MoveTo.normalized, Vector3.one * Mathf.Pow(mouvementParameters.ArealMouvementAcceleration, DeltaReforming), new Vector3(1, 0, 1) * mouvementParameters.MaxSpeed * 0.9f);
        }
        if(GroundDetected && hit.distance < 0.27f && InputControl.GetInput(InputControl.InputType.MouvementJump) && !FreezeMouvement) {
            MainRigidbody.velocity = new Vector3(MainRigidbody.velocity.x + hit.normal.x * mouvementParameters.JumpForce, Mathf.Max(MainRigidbody.velocity.y, mouvementParameters.JumpForce) * hit.normal.y, MainRigidbody.velocity.z + hit.normal.z * mouvementParameters.JumpForce);
        }
        if(LedgeDetected && hitHandGrab.distance < 0.37f) {
            if(InputControl.GetInput(InputControl.InputType.MouvementJump) && !FreezeMouvement) {
                if(MainRigidbody.velocity.y < mouvementParameters.LedgeGrabIncrementation) {
                    MainRigidbody.velocity += Vector3.up * mouvementParameters.LedgeGrabIncrementation * DeltaReforming;
                    MainRigidbody.velocity = new Vector3(MainRigidbody.velocity.x, Mathf.Max(MainRigidbody.velocity.y,0f), MainRigidbody.velocity.z);
                }
            }
        }
        if(LedgeDetected && hitHandGrab.distance < 0.45f) {
            if(ParentStick == null) {
                ParentStick = hitHandGrab.collider.transform;
            }
            if(InputControl.GetInputUp(InputControl.InputType.MouvementJump) && (MainRigidbody.velocity.y > -mouvementParameters.LedgeGrabMaxForce && MainRigidbody.velocity.y < mouvementParameters.LedgeGrabMaxForce + mouvementParameters.LedgeGrabIncrementation) && !FreezeMouvement) {
                MainRigidbody.velocity += Vector3.up * mouvementParameters.LedgeGrabBoost;
            }
        }
        if(MainRigidbody.velocity.y < 0) {
            MainRigidbody.velocity = new Vector3(MainRigidbody.velocity.x, Mathf.Clamp(MainRigidbody.velocity.y * Mathf.Pow(mouvementParameters.GravityBoost, DeltaReforming), -100f, 0f), MainRigidbody.velocity.z);
        }
        MainRigidbody.velocity = new Vector3(MainRigidbody.velocity.x, Mathf.Clamp(MainRigidbody.velocity.y - (mouvementParameters.PermaGravityBoost * DeltaReforming), -100f, Mathf.Infinity), MainRigidbody.velocity.z);

        if(ParentStick != null) {
            if(OldParentStick != null && OldParentStick == ParentStick) {
                if(OldParentRelativePosition != ParentStick.InverseTransformPoint(OldBodyPosition)) {
                    Vector3 mi = Body.position - OldBodyPosition;
                    Body.position -= mi;
                    Body.position = ParentStick.TransformPoint(OldParentRelativePosition);
                    Body.position += mi;
                }
                if(OldEulerAngles != ParentStick.eulerAngles) {
                    TargetCamXRot += ParentStick.eulerAngles.y - OldEulerAngles.y;
                    CurrentCamYRot = TargetCamXRot;
                }
            }
        }

        if(!LastFrameGrounded && GroundDetected && hit.distance < 0.27f) {
            HeadXBoobing.connectedBody.angularVelocity = Body.right * -OldVelocity.y * 0.1f;
        }

        if(ParentStick != null) {
            OldPosition = ParentStick.position;
            OldEulerAngles = ParentStick.eulerAngles;
            OldParentRelativePosition = ParentStick.InverseTransformPoint(Body.position);
            OldBodyPosition = Body.position;
        }
        OldVelocity = MainRigidbody.velocity;
        LastFrameGrounded = GroundDetected && hit.distance < 0.27f;
    }*/

    void Update () {
        MainCamera.fieldOfView = Mathf.Lerp(MainCamera.fieldOfView, Mathf.Clamp(cameraParameters.BaseFOV + MainRigidbody.velocity.magnitude * cameraParameters.VelocityFOVBoost, 0f, 145f), cameraParameters.VelocityFOVBoostSpeed);
        if(!FreezeCamera) {
            TargetCamYRot = Mathf.Clamp(TargetCamYRot + Input.GetAxis("Mouse Y") * -cameraParameters.SensibilityX, -90, 90);
            TargetCamXRot = TargetCamXRot + Input.GetAxis("Mouse X") * cameraParameters.SensibilityY;
        }
        CurrentCamXRot = Mathf.Lerp(CurrentCamXRot, TargetCamYRot, Mathf.Pow(cameraParameters.SmoothingSpeed, Time.deltaTime));
        CurrentCamYRot = Mathf.Lerp(CurrentCamYRot, TargetCamXRot, Mathf.Pow(cameraParameters.SmoothingSpeed, Time.deltaTime));
        Body.localEulerAngles = new Vector3(Body.localEulerAngles.x, CurrentCamYRot, Body.localEulerAngles.z);
        Head.localEulerAngles = new Vector3(CurrentCamXRot, Head.localEulerAngles.y, Head.localEulerAngles.z);
    }

    void Accelerate (Vector3 Direction, Vector3 AccelerationPerAxis, Vector3 MaxSpeedPerAxis) {
        float t = 1f;
        Vector3 CVel = MainRigidbody.velocity;
        Vector3 Vel = Vector3.zero;
        if((Direction.x > 0 && CVel.x < MaxSpeedPerAxis.x * Direction.x) || (Direction.x < 0 && CVel.x > MaxSpeedPerAxis.x * Direction.x)) {
            Vel.x = Mathf.Clamp(CVel.x + (AccelerationPerAxis.x * Direction.x), -Mathf.Abs(MaxSpeedPerAxis.x * Direction.x), Mathf.Abs(MaxSpeedPerAxis.x * Direction.x));
        } else {
            Vel.x = CVel.x;
        }
        if((Direction.y > 0 && CVel.y < MaxSpeedPerAxis.y * Direction.y) || (Direction.y < 0 && CVel.y > MaxSpeedPerAxis.y * Direction.y)) {
            Vel.y = Mathf.Clamp(CVel.y + (AccelerationPerAxis.y * Direction.y), -Mathf.Abs(MaxSpeedPerAxis.y * Direction.y), Mathf.Abs(MaxSpeedPerAxis.y * Direction.y));
        } else {
            Vel.y = CVel.y;
        }
        if((Direction.z > 0 && CVel.z < MaxSpeedPerAxis.z * Direction.z) || (Direction.z < 0 && CVel.z > MaxSpeedPerAxis.z * Direction.z)) {
            Vel.z = Mathf.Clamp(CVel.z + (AccelerationPerAxis.z * Direction.z), -Mathf.Abs(MaxSpeedPerAxis.z * Direction.z), Mathf.Abs(MaxSpeedPerAxis.z * Direction.z));
        } else {
            Vel.z = CVel.z;
        }
        MainRigidbody.velocity = Vel*t;
    }

    void AccelerateAngle (Vector3 Direction, Vector3 AccelerationPerAxis, Vector3 MaxSpeedPerAxis) {
        float t = 1f;
        Vector3 CVel = MainRigidbody.angularVelocity;
        Vector3 Vel = Vector3.zero;
        if((Direction.x > 0 && CVel.x < MaxSpeedPerAxis.x * Direction.x) || (Direction.x < 0 && CVel.x > MaxSpeedPerAxis.x * Direction.x)) {
            Vel.x = Mathf.Clamp(CVel.x + (AccelerationPerAxis.x * Direction.x), -Mathf.Abs(MaxSpeedPerAxis.x * Direction.x), Mathf.Abs(MaxSpeedPerAxis.x * Direction.x));
        } else {
            Vel.x = CVel.x;
        }
        if((Direction.y > 0 && CVel.y < MaxSpeedPerAxis.y * Direction.y) || (Direction.y < 0 && CVel.y > MaxSpeedPerAxis.y * Direction.y)) {
            Vel.y = Mathf.Clamp(CVel.y + (AccelerationPerAxis.y * Direction.y), -Mathf.Abs(MaxSpeedPerAxis.y * Direction.y), Mathf.Abs(MaxSpeedPerAxis.y * Direction.y));
        } else {
            Vel.y = CVel.y;
        }
        if((Direction.z > 0 && CVel.z < MaxSpeedPerAxis.z * Direction.z) || (Direction.z < 0 && CVel.z > MaxSpeedPerAxis.z * Direction.z)) {
            Vel.z = Mathf.Clamp(CVel.z + (AccelerationPerAxis.z * Direction.z), -Mathf.Abs(MaxSpeedPerAxis.z * Direction.z), Mathf.Abs(MaxSpeedPerAxis.z * Direction.z));
        } else {
            Vel.z = CVel.z;
        }
        MainRigidbody.angularVelocity = Vel * t;
    }
}

[Serializable]
public class CameraParameters {
    public float BaseFOV = 0f;
    public float SensibilityX = 0f;
    public float SensibilityY = 0f;
    public float SmoothingSpeed = 0.1f;
    public float VelocityFOVBoost = 0f;
    public float VelocityFOVBoostSpeed = 0.1f;
}

[Serializable]
public class MouvementParameters {
    public float MaxSpeed = 1f;
    public float ArealMouvementAcceleration = 0f;
    public float MouvementAcceleration = 0.075f;
    public float ArealFriction = 0f;
    public float VerticalArealFriction = 0f;
    public float GroundSlowDown = 0f;
    public float JumpForce = 3f;
    public float GravityBoost = 1.01f;
    public float PermaGravityBoost = 0.02f;

    public float LedgeGrabMaxForce = 10f;
    public float LedgeGrabIncrementation = 0.2f;
    public float LedgeGrabBoost = 45f;
}
