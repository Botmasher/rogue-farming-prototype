using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Epitaph : MonoBehaviour {

	// settings to move on or off screen
	Vector3 showPosition; 		// onscreen position - read from starting position
	Vector3 hidePosition;		// offscreen position
	Vector3 targetPosition;		// container for animating between show or hide position
	public float speed = 2f; 	// animation speed factor

	void Start () {
		// capture position of prefab object
		showPosition = transform.position;

		// position offscreen
		hidePosition = new Vector3 (
			transform.position.x + 500f,
			transform.position.y - 250f,
			transform.position.z
		);

		// start out hidden
		transform.position = hidePosition;
		targetPosition = hidePosition;
	}

	void Update () {
		// transition on/off screen
		if (transform.position != targetPosition) {
			transform.position = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * speed);
		}

		// harcode allowing user-input hiding
		if (Input.GetKeyDown(KeyCode.J)) {
			Show ();
		}
	}

	// slide ui onscreen - called from plot
	public void Show() {
		targetPosition = showPosition;
	}

	// slide ui offscreen - called from plot
	public void Hide() {
		targetPosition = hidePosition;
	}

}
