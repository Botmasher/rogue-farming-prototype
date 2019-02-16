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
	private Vector3 velocity; 		// store calculated physics body velocity
	private float leeway = 0.05f; 	// small wiggle room around zero before registering axis
	private Rigidbody rigidbody; 	// reference to grim body for physics

	// behavior checks
	private bool isMoving = false;
	private bool isSwiping = false;
	private float swipeTimer; 			// count down for swipe impact
	private GameObject collidedObject; 	// store recent collision for interactables like pickups

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

		// fetch the inventory behavior for this grim
		inventory = GetComponentInChildren <Inventory> ();

		// store body for movement through physics forces
		rigidbody = GetComponent<Rigidbody> ();
	}
		
	void Update () {
		// TODO state machine to juggle sprite rendering and adding multiframe animations

		/* Basic movement */

		// walk around on axis input
		renderer.flipX = false; 	// reset sprite flipping for positive horizontal movement
		// set body speed on input
		velocity = new Vector3(
			Input.GetAxis("Horizontal") * speed,
			rigidbody.velocity.y,
			Input.GetAxis("Vertical") * speed
		);

		// set sprite to base image
		renderer.sprite = spriteDefault;

		// animate character movement horizontally
		if (Mathf.Abs (velocity.x) > 0f) {
			// instead use sideways sprite
			renderer.sprite = spriteSide;
			// flip sprite when traveling left
			renderer.flipX = velocity.x > 0;
		}

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

		/* Use action */
		// ui slot selected and press "use" then attempt to plant
		// - if selected rogue and raycast hit a FarmPlot then plant
		// - "use"? - if selected weapon/armor have it highlighted then store it until selecting rogue to attach it to
		if (Input.GetButtonDown ("Use")) {
			HandleUse ();
		}

	}

	void FixedUpdate () {
		rigidbody.velocity = velocity;
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

	// grab inventory item on use
	void PickUp (GameObject pickupItem) {
		// physically pick up item and attach it here
		pickupItem.transform.position = this.transform.position;

		// place item in inventory
		bool didAdd = inventory.AddItem (pickupItem);

		// did not fit - have grim toss the pickup
		if (!didAdd) {
			pickupItem.GetComponent<Rigidbody> ().AddForce (Vector3.right * 10f);
		}
		return;
	}

	/* Input actions - including their colliders and raycasts */

	// on explicit use input
	void HandleUse () {
		// TODO: use an inventory item if one exists
		GameObject usedItem = inventory.UseSelected ();

		if (usedItem != null) {
			// move and toss the item slightly outside to avoid collider push
			usedItem.transform.Translate (Vector3.back * 1.2f);
			usedItem.GetComponent<Rigidbody> ().AddForce (Vector3.right * 10f);

			// TEMP coroutine for testing thrown out item
			StartCoroutine (WaitThenResetPickup (usedItem, 0.8f));
		}

		return;
	}

	// TEMP coroutine for resetting tossed item
	IEnumerator WaitThenResetPickup (GameObject pickup, float timer) {
		pickup.tag = "Untagged";
		yield return new WaitForSeconds (timer);
		pickup.tag = "Pickup";
	}

	void HandleSwipe () {
		// raycast to check for farm plot
		if (Physics.Raycast (transform.position, transform.TransformDirection (Vector3.down), out hit)) {
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


	// pickup touched objects automatically
	void OnCollisionEnter (Collision collision) {
		if (collision.gameObject.tag == "Pickup") {
			PickUp (collision.gameObject);
		}
	}

}
