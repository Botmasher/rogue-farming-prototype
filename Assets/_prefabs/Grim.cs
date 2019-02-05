using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grim : MonoBehaviour {

	// components
	private SpriteRenderer renderer; 	// store reference to render component
	private Inventory inventory; 		// inventory management

	// sprites to render
	public Sprite spriteDefault;
	public Sprite spriteSwipe;
	public Sprite spriteSide;

	// input calc
	public float speed = 1f; 		// factor for movement over time
	private Vector2 axes; 			// holder for (horizontal, vertical) input ranging from -1 to 1
	private float leeway = 0.05f; 	// small wiggle room around zero before registering axis

	// behavior flags
	private bool isMoving = false;
	private bool isSwiping = false;
	private float swipeTimer; 		// count down for swipe impact

	// environment interaction
	private GameObject focusedPlot; // farm plot for planting, gathering or info

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
			this.transform.Translate (new Vector3(
				axes.x * speed * Time.deltaTime,
				0f,
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
			if (Physics.Raycast (this.transform.position, this.transform.TransformDirection (Vector3.down), out hit)) {
				if (hit.collider.gameObject.tag == "FarmPlot") {
					// remember the focused plot
					this.SetFarm (hit.collider.gameObject);
					// decide behavior based on plot status
					this.HandlePlotSwipe ();
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

	// assign currently focused farm plot
	void SetFarm(GameObject farmPlot) {
		this.focusedPlot = farmPlot;
	}

	// take farm action on swipe
	void HandlePlotSwipe() {
		Debug.Log (string.Format("Successfully swiped a farm plot named {0}", this.focusedPlot.name));

		// TODO branches for different growth or emptiness of plot
		Plot plotBehavior = this.focusedPlot.GetComponent<Plot> ();
		// if/case ...
	}
}
