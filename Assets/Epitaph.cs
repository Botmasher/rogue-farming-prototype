using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Epitaph : MonoBehaviour {

	// settings to move on or off screen
	Vector3 showPosition;
	Vector3 hidePosition;
	Vector3 targetPosition;
	public float speed = 2f;

	// wedge time between plot calls to Show (walk on plot) and Hide (walking away)
	bool isDelayingHide = false;

	void Start () {
		// capture position of prefab object
		showPosition = transform.position;

		// position offscreen
		hidePosition = new Vector3 (
			transform.position.x + 400f,
			transform.position.y - 200f,
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
		StartCoroutine ("BufferShow");
	}

	// slide ui offscreen - called from plot
	public void Hide() {
		if (isDelayingHide) {
			return;
		}
		targetPosition = hidePosition;
	}

	// delay ability to hide in order to transition gracefully between nearby stones
	IEnumerator BufferShow () {
		isDelayingHide = true;
		yield return new WaitForSeconds (1f);
		isDelayingHide = false;
	}



}
