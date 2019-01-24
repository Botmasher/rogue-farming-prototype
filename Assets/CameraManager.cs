using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	// movement calc
	public float smoothing = 2f;
	public float distanceZ = -10f; 	// depth distance to pull shot back from or tight to target
	public float distanceY = -10f; 	// x-rotation compensation distance to pull shot back from or tight to target

	Transform target; 				// transform to track
	Vector3 targetPosition; 		// for storing updated horiz and vert for target without compromising camera z distance

	void Start () {
		// default to tracking player grim
		target = GameObject.FindGameObjectWithTag ("Player").transform as Transform;
	}

	void Update () {
		targetPosition = new Vector3 (target.position.x, target.position.y + distanceY, distanceZ);
		transform.position = Vector3.Lerp (this.transform.position, targetPosition, Time.deltaTime * smoothing);
	}
}
