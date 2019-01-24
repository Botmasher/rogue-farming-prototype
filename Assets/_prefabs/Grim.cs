using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grim : MonoBehaviour {

	// sprites to render
	public Sprite spriteDefault;
	public Sprite spriteSwipe;
	public Sprite spriteSide;
	private SpriteRenderer renderer; // store reference to render component

	// input calc
	public float speed = 1f; 		// factor for movement over time
	private Vector2 axes; 			// holder for (horizontal, vertical) input ranging from -1 to 1
	private float leeway = 0.05f; 	// small wiggle room around zero before registering axis

	// behavior flags
	private bool isMoving = false;
	private bool isSwiping = false;
	private float swipeTimer; 		// count down for swipe impact

	// raycasting
	RaycastHit hit;

	void Start () {
		// set initial sprite
		renderer = this.GetComponentInChildren <SpriteRenderer> ();
		renderer.sprite = spriteDefault;
	}

	void Update () {
		// TODO state machine to avoid juggling renderer logic and smoothing time

		/* character movement */
		// walk around on axis input
		axes = new Vector2 (Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		// side to side
		renderer.flipX = false; 	// reset sprite flipping for positive horizontal movement
		if (Mathf.Abs (axes.x) > leeway) {
			renderer.sprite = spriteSide;
			if (axes.x > 0f && !renderer.flipX) {
				renderer.flipX = true;
			}
			isMoving = true;
		} else {
			renderer.sprite = spriteDefault;
		}
		// up and down
		if (Mathf.Abs (axes.y) > leeway) {
			isMoving = true;
		}
		if (isMoving && !isSwiping) {
			this.transform.Translate (new Vector2(
				axes.x * speed * Time.deltaTime,
				axes.y * speed * Time.deltaTime
			));
			isMoving = false;
		}

		/* character actions */
		// swipe scythe but avoid stacking swipes
		if (!isSwiping && Input.GetButtonDown ("Swipe")) {
			isSwiping = true;
			swipeTimer = 0.1f;
			renderer.sprite = spriteSwipe;

			// raycast to check for farm plot
			if (Physics.Raycast (this.transform.position, this.transform.TransformDirection (Vector3.forward), out hit)) {
				if (hit.collider.gameObject.tag == "FarmPlot") {
					this.ScytheFarm (hit.collider.gameObject);
				}	
			}


		} else if (isSwiping && swipeTimer > 0f) {
			swipeTimer -= Time.deltaTime;
			renderer.flipX = false;
			renderer.sprite = spriteSwipe;
		} else {
			swipeTimer = 0f;
			isSwiping = false;
		}

		/* character inventory */
		// TODO inventory toggle
		// 	- place or use items
		// 	- if inventory available manage it with movement
	}

	// take farm action on raycast hit
	void ScytheFarm(GameObject farmPlot) {
		Debug.Log (string.Format("Successfully swiped a farm plot named {0}", farmPlot.name));

	}
}
