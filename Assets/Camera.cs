using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour {

	// movement calc
	public float smoothing = 2f;

	Transform target; 			// transform to track
	Vector3 targetPosition; 	// for storing updated horiz and vert for target without compromising camera z distance
	float distanceZ = -15f; 	// depth distance to pull shot back from or tight to target

	void Start () {
		// default to tracking player grim
		target = GameObject.FindGameObjectWithTag ("Player").transform as Transform;
	}
	
	void Update () {
		targetPosition = new Vector3 (target.position.x, target.position.y, distanceZ);
		transform.position = Vector3.Lerp (this.transform.position, targetPosition, Time.deltaTime * smoothing);
	}
}
