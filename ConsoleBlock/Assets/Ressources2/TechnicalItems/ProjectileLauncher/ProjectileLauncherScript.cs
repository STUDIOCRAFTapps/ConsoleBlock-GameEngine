using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncherScript : MonoBehaviour {

    public GameObject ProjectileReference;
    Quaternion LookRotation;
    Vector3 Direction;

    public Vector3 up;

    // Update is called once per frame
    void Update () {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Display.main.systemWidth/2, Display.main.systemHeight/2));
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)) {
            if(Input.GetMouseButtonDown(1)) {
                GameObject Projectile = (GameObject)Instantiate(ProjectileReference, null);
                Projectile.SetActive(true);
                Projectile.transform.position = ProjectileReference.transform.position;
                Projectile.transform.LookAt(hit.point);
                //Projectile.transform.eulerAngles = ProjectileReference.transform.eulerAngles;
                Projectile.GetComponent<Rigidbody>().velocity = Projectile.transform.forward * 40f;
            }
            //ProjectileReference.transform.parent.LookAt(hit.point);
            //find the vector pointing from our position to the target
            Direction = (hit.point - ProjectileReference.transform.parent.position).normalized;
            LookRotation = Quaternion.LookRotation(-Direction, up);
            transform.rotation = Quaternion.Slerp(ProjectileReference.transform.parent.rotation, LookRotation, Time.deltaTime * 3f);
        } else {
            if(Input.GetMouseButtonDown(1)) {
                GameObject Projectile = (GameObject)Instantiate(ProjectileReference, null);
                Projectile.SetActive(true);
                Projectile.transform.position = ProjectileReference.transform.position;
                //Projectile.transform.LookAt(hit.point);
                Projectile.transform.eulerAngles = ProjectileReference.transform.eulerAngles;
                Projectile.GetComponent<Rigidbody>().velocity = Projectile.transform.forward * -40f;
            }
        }
	}
}
