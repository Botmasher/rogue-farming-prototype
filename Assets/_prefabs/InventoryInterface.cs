using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryInterface : MonoBehaviour {

	// TODO: configure slot interation within individual slots

	// inventory slots
	public Image slot; 					// image and transforms for proliferating and visualizing storage slots
	public int slotCount = 10; 			// number of slots to create for displaying inventory items
	private RectTransform background; 	// self reference for managing inventory position on screen

	// container for referencing created slot images and items by index
	// NOTE: sync up with Inventory in order to keep visuals parallel to data
	private List<Image> slotList = new List<Image> ();

	// inventory images positioning
	public Vector3 slotStartPosition;
	public float slotSpacingHorizontal;

	// animation control
	bool isHidden = false; 				// toggle visibility
	public float hidingSpeed = 3f; 		// speed factor for sliding inventory onscreen or offscreen
	Vector3 backgroundTargetPosition; 	// for storing calculated target show or hide position

	// raycast interaction with inventory
	GraphicRaycaster raycaster; 		// canvas ui raycaster
	PointerEventData pointerEventData; 	// setup for cursor/pointer position
	EventSystem eventSystem; 			// scene hierarchy event system

	// arrange slots
	void Start() {
		// reference event and raycast components
		raycaster = GetComponentInParent <GraphicRaycaster>();
		eventSystem = GetComponentInParent <EventSystem> ();

		// store the script parent image interface component
		background = this.GetComponent<RectTransform> ();

		// set up slot images to display visuals relating to Inventory list data
		float slotPositionX; 	// temporarily hold incrementing slot horizontal positions
		for (int i = 0; i < slotCount; i++) {
			// calculate the next horizontal value for the slot to be created
			slotPositionX = i > 0
				? slotList[i - 1].rectTransform.anchoredPosition3D.x
				: slotStartPosition.x
			;
			
			// create and store slot
			slotList.Add (GameObject.Instantiate (slot) as Image);

			// parent to keep slot and item ui under inventory in canvas
			slotList [i].rectTransform.SetParent (background.transform);

			// position slot along the row
			slotList [i].rectTransform.anchoredPosition3D = new Vector3 (
				slotPositionX + slotSpacingHorizontal,
				slotStartPosition.y,
				slotStartPosition.z
			);
		}
	}

	// interact with and animate ui
	void Update() {

		/*  Inventory interaction */
		// TODO: drag and drop to attach weapons and armor to rogue

		// ui raycast against pointer position on select input
		// adapted from https://docs.unity3d.com/ScriptReference/UI.GraphicRaycaster.Raycast.html
		if (Input.GetButton ("Select")) {
			// track mouse position using event system
			pointerEventData = new PointerEventData (eventSystem);
			pointerEventData.position = Input.mousePosition;
			// container for raycast hits
			List<RaycastResult> raycastResults = new List <RaycastResult> ();
			// perform raycast and check all hit items
			raycaster.Raycast (pointerEventData, raycastResults);
			// identify selected slot
			foreach (RaycastResult raycastResult in raycastResults) {
				if (raycastResult.gameObject.tag == "InterfaceSlot") {
					int slotIndex = slotList.IndexOf(raycastResult.gameObject.GetComponent<Image> ());
					Debug.Log ("The UI element is in my list as slot #" + slotIndex);
				}
			}
		}

		// TODO: raycast hover for info


		/* Inventory visibility */

		// toggle show/hide - control flow for lockout during animation
		if (Input.GetButtonDown ("Inventory")) {
			// flip hiding flag
			isHidden = !isHidden;
			// set position to onscreen or offscreen
			backgroundTargetPosition = isHidden
				? new Vector3 (background.anchoredPosition3D.x, -200f, background.anchoredPosition3D.z)
				: new Vector3 (background.anchoredPosition3D.x, 0f, background.anchoredPosition3D.z)
			;
		}

		// animate visibility
		// snap to final position if reached small wiggle room around it
		if (Mathf.Abs(background.position.y - backgroundTargetPosition.y) <= 0.01f) {
			background.anchoredPosition3D = backgroundTargetPosition;
		}
		// translate towards final show/hide position
		else {
			// refer to anchor position to include otherwise neglected offsets
			background.anchoredPosition3D = Vector3.Lerp (
				background.anchoredPosition3D,
				backgroundTargetPosition,
				Time.deltaTime * hidingSpeed
			);
		}

	}


	/* handle interactions with individual real (non-ui) items and slots */

	// completely rework items ui just to contain only those items in current Inventory items list
	// TODO: just initialize the entire items and slots from the inventory list and refresh them here
	public void RefreshSlots(List <GameObject> newItems) {
		// too many items for the inventory ui
		if (newItems.Count > slotList.Count) {
			Debug.Log ("Inventory UI slots count out of sync with Inventory items!");
		}
		// sync slots and items ui with actual item objects
		int slotIndex = 0;
		foreach (GameObject item in newItems) {
			Debug.Log ("Trying to add a pickup on pass " + slotIndex);
			// reference the script on this item ui
			InventoryInterfaceSlot slotBehavior = slotList [slotIndex].GetComponent <InventoryInterfaceSlot> ();
			// clear out all item data from slot ui
			slotBehavior.Clear ();
			// update the slot ui with info for newly added item
			slotBehavior.Store (newItems [slotIndex]);
			slotIndex++;
		}
	}

	// set the currently selected slot and item based on raycast hit
	void SelectSlot() {
		// if the raycast click is a slot ui then set it to selected
		// if selected item ui (same list index) IsEmpty then selected item is null
	}

	// add an item to a slot
	// OR just use refreshslots above so all adding is done through Inventory
	void AddItem(GameObject item) {
		// TODO: figure out which slots are open and add
		// - reject back so Inventory.Drop() automatically drops if no slots available
		// - the drop can then message back to drop from grim hand
	}

}
