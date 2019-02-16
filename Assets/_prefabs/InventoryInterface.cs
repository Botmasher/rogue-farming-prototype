using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryInterface : MonoBehaviour {

	// inventory ui slots info
	public Image slot; 					// image and transforms for proliferating and visualizing storage slots
	private RectTransform background; 	// self reference for managing inventory position on screen

	// container for referencing created slot images and items by index
	// NOTE: slots length derived from inventory items - sync Inventory to keep visuals parallel to data
	private List<Image> slotList = new List<Image> ();

	// inventory ui image positioning
	public Vector3 slotStartPosition;
	public float slotSpacingHorizontal;

	// reference to the inventory tied to this ui for sending results of player interactions
	public Inventory inventory;

	// animation control
	bool isHidden = false; 				// toggle visibility
	public float hidingSpeed = 3f; 		// speed factor for sliding inventory onscreen or offscreen
	Vector3 backgroundTargetPosition; 	// for storing calculated target show or hide position

	// raycast interaction with inventory
	GraphicRaycaster raycaster; 		// canvas ui raycaster
	PointerEventData pointerEventData; 	// setup for cursor/pointer position
	EventSystem eventSystem; 			// scene hierarchy event system

	// currently selected item slot or -1 if none selected
	int selectedSlot = -1;

	// arrange slots
	void Start() {
		// reference event and raycast components
		raycaster = GetComponentInParent <GraphicRaycaster>();
		eventSystem = GetComponentInParent <EventSystem> ();

		// store the script parent image interface component
		background = this.GetComponent<RectTransform> ();

		// set up slot images to display visuals relating to Inventory list data
		float slotPositionX; 	// temporarily hold incrementing slot horizontal positions
		for (int i = 0; i < inventory.Limit(); i++) {
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

		// use pointer input to select
		// adapted from https://docs.unity3d.com/ScriptReference/UI.GraphicRaycaster.Raycast.html
		if (Input.GetButtonDown ("Select")) {
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
					// determine the index of the hit ui slot
					int slotIndex = slotList.IndexOf (raycastResult.gameObject.GetComponent<Image> ());

					// notify the inventory of the selected index
					inventory.SelectSlot (slotIndex);

					// unhighlight the previously selected item
					if (selectedSlot > -1) {
						slotList [selectedSlot].GetComponent <InventoryInterfaceSlot> ().Deselect ();
					}
						
					// deselect the same slot selected again 
					if (selectedSlot == slotIndex) {
						slotList [selectedSlot].GetComponent <InventoryInterfaceSlot> ().Deselect ();
						selectedSlot = -1;
					// set and highlight the newly selected one
					} else {
						selectedSlot = slotIndex;
						slotList [selectedSlot].GetComponent <InventoryInterfaceSlot> ().Select();
					}
				}
			}
		}

		// TODO: update inventory if player swaps or attaches items, then have inventory refresh ui

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
	public void RefreshSlots(List <GameObject> newItems) {
		Debug.Log ("Refreshing the inventory completely");

		// completely deselect the item and the ui connection to it
		inventory.SelectSlot (-1);
		selectedSlot = -1;

		// too many items for the inventory ui
		if (newItems.Count > slotList.Count) {
			Debug.Log ("Inventory UI slots count out of sync with Inventory items!");
			return;
		}

		// sync slots and items ui with actual item objects
		for (int i = 0; i < slotList.Count; i++) {

			// reference the script on this item ui
			InventoryInterfaceSlot slotBehavior = slotList [i].GetComponent <InventoryInterfaceSlot> ();

			// clear out all item data from slot ui
			slotBehavior.Clear ();

			// update slot ui with info for newly added item
			if (i < newItems.Count) {
				slotBehavior.Store (newItems [i]);
			}
		}

	}

}
