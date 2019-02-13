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

	// movement based on input
	public float speed = 1f; 		// factor for movement over time
	private Vector2 axes; 			// holder for (horizontal, vertical) input ranging from -1 to 1
	private float leeway = 0.05f; 	// small wiggle room around zero before registering axis
	private Rigidbody rigidbody; 	// reference to grim body for physics

	// behavior flags
	private bool isMoving = false;
	private bool isSwiping = false;
	private float swipeTimer; 		// count down for swipe impact

	// environment interaction
	//private GameObject focusedPlot; // farm plot for planting, gathering or info

	// item held in grim hands
	public GameObject currentItem;

	// raycasting
	RaycastHit hit;

	void Start () {
		// set initial sprite
		renderer = this.GetComponentInChildren <SpriteRenderer> ();
		renderer.sprite = spriteDefault;

		// store body for movement through physics forces
		rigidbody = GetComponent<Rigidbody> ();
	}

	void Update () {
		// TODO state machine to avoid juggling renderer logic and smoothing time

		/* Basic movement */

		// walk around on axis input
		axes = new Vector2 (Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		// side to side
		renderer.flipX = false; 	// reset sprite flipping for positive horizontal movement
		// set body speed on input
		rigidbody.velocity = new Vector3(axes.x, 0f, axes.y) * speed * Time.deltaTime;

//		if (Mathf.Abs (axes.x) > leeway) {
//			renderer.sprite = spriteSide;
//			if (axes.x > 0f && !renderer.flipX) {
//				renderer.flipX = true;
//			}
//			isMoving = true;
//		} else {
//			renderer.sprite = spriteDefault;
//		}
//		// up and down
//		if (Mathf.Abs (axes.y) > leeway) {
//			isMoving = true;
//		}
//		if (isMoving && !isSwiping) {
//			this.transform.Translate (new Vector3(
//				axes.x * speed * Time.deltaTime,
//				0f,
//				axes.y * speed * Time.deltaTime
//			));
//			isMoving = false;
//		}

		/* Swiping action */

		// swipe scythe but avoid stacking swipes
		if (!isSwiping && Input.GetButtonDown ("Swipe")) {
			isSwiping = true;
			swipeTimer = 0.1f;
			renderer.sprite = spriteSwipe;
			HandleSwipe ();
		// wait before allowing another swipe
		} else if (isSwiping && swipeTimer > 0f) {
			swipeTimer -= Time.deltaTime;
			renderer.flipX = false;
			renderer.sprite = spriteSwipe;
		// reenable swiping after delay
		} else {
			swipeTimer = 0f;
			isSwiping = false;
		}

	}


	/* Interact with items */

	// TODO: set up in concert with Inventory and InventoryInterface
	// 	- pass stored item into Inventory
	// 	- Inventory notifies the ui of an add
	// 	- ui fills the slot > item with image of item

	// 	- from there handle selection in reverse (pass info back from inventory ui)
	// 	- should grim know about selected item?
	// 	- what if grim wants to remove or delete item? 

	// set the item held by grim
	void HoldItem(GameObject item) {
		currentItem = item;
		currentItem.transform.SetParent (this.gameObject.transform);
	}

	// place the held item in inventory
	void StoreItem() {
		currentItem.transform.SetParent (inventory.gameObject.transform);
	}


	/* Interact with world */

	// take farm action on swipe
	void SwipeFarm(GameObject farmPlot) {
		Debug.Log (string.Format("Successfully swiped a farm plot named {0}", farmPlot.name));

		// TODO branches for different growth or emptiness of plot
		Plot plotBehavior = farmPlot.GetComponent<Plot> ();
		// if/case ...
	}


	/* Raycast cases */

	void HandleSwipe () {
		// raycast to check for farm plot
		if (Physics.Raycast (this.transform.position, this.transform.TransformDirection (Vector3.down), out hit)) {
			switch (hit.collider.gameObject.tag) {
				// decide farm behavior based on plot status
				case ("FarmPlot"):
					SwipeFarm (hit.collider.gameObject);
					break;
				// ignore raycasted object
				default:
					break;
			}
		}
	}

}
