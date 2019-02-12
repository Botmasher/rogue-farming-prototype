﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	// movement calc
	public float speed = 2f;
	public float distanceZ = -10f; 	// depth distance to pull shot back from or tight to target
	public float rotationX = -20f; 	// rotation to look up at or down on scene
	public float distanceY = 10f; 	// x-rotation compensation distance to fly above or below x-rotated viewed target

	Transform target; 				// transform to track
	Vector3 targetPosition; 		// for storing updated horiz and vert for target without compromising camera z distance
	Quaternion targetRotation; 		// for storing updated x,y,z rotations converted from Euler

	void Start () {
		// default to tracking player grim
		target = GameObject.FindGameObjectWithTag ("Player").transform as Transform;
	}

	void Update () {
		targetPosition = new Vector3 (target.position.x, distanceY, target.position.z + distanceZ);
		targetRotation = Quaternion.Euler(new Vector3 (rotationX, target.rotation.y, target.rotation.z));
		transform.position = Vector3.Lerp (this.transform.position, targetPosition, Time.deltaTime * speed);
		transform.rotation = Quaternion.Lerp (this.transform.rotation, targetRotation, Time.deltaTime * speed);
	}
}
