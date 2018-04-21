using UnityEngine;
using System.Collections;

public class FOV : MonoBehaviour {

	public UnityStandardAssets.ImageEffects.DepthOfField dof;

	void Update () {
		Vector3 fwd = transform.TransformDirection(Vector3.forward);
		RaycastHit ray;

		if (Physics.Raycast(transform.position, fwd, out ray, 2000)) {
			dof.focalLength = ray.distance;
		}

	}
}
