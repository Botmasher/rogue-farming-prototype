using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryInterface : MonoBehaviour {

	// TODO: configure slot interation within individual slots

	// inventory slots
	public Image slot; 					// image and transforms for proliferating and visualizing storage slots
	public Image slotItem; 				// empty item icon holder to place above each slot
	public int slotCount = 10; 			// number of slots to create for displaying inventory items
	private RectTransform background; 	// self reference for managing inventory position on screen

	// container for referencing created slot images and items by index
	// NOTE: sync up with Inventory in order to keep visuals parallel to data
	private List<Image> slotList = new List<Image> ();
	private List<Image> itemList = new List<Image> ();

	// inventory images positioning
	public Vector2 slotStartPosition;
	public float slotSpacingHorizontal;

	// animation control
	bool isHidden = false; 				// toggle visibility
	public float hidingSpeed = 3f; 		// speed factor for sliding inventory onscreen or offscreen
	Vector3 backgroundTargetPosition; 	// for storing calculated target show or hide position

	// arrange slots
	void Start() {
		// store the script parent image interface component
		background = this.GetComponent<RectTransform> ();

		// set up slot images to display visuals relating to Inventory list data
		float slotPositionX; 	// temporarily hold incrementing slot horizontal positions
		for (int i = 0; i < slotCount; i++) {
			// calculate the next horizontal value for the slot to be created
			slotPositionX = i > 0
				? slotList[i - 1].rectTransform.anchoredPosition.x
				: slotStartPosition.x
			;
			
			// create and store slot
			slotList.Add (GameObject.Instantiate (slot) as Image);
			itemList.Add (GameObject.Instantiate (slotItem));

			// parent to keep slot and item ui under inventory in canvas
			slotList [i].rectTransform.SetParent (background);
			itemList [i].rectTransform.SetParent (background);

			// position slot along the row
			slotList [i].rectTransform.anchoredPosition = new Vector2 (slotPositionX + slotSpacingHorizontal, slotStartPosition.y);

			// position a corresponding item holder slightly atop the slot
			itemList [i].rectTransform.anchoredPosition3D = new Vector3 (
				slotList [i].rectTransform.anchoredPosition3D.x,
				slotList [i].rectTransform.anchoredPosition3D.y,
				slotList [i].rectTransform.anchoredPosition3D.z - 1f
			);
		}
	}

	// animate ui
	void Update() {
		// toggle show/hide - control flow for lockout during animation
		if (Input.GetKeyDown (KeyCode.I)) {
			// flip hiding flag
			isHidden = !isHidden;
			// set position to onscreen or offscreen
			backgroundTargetPosition = isHidden
				? new Vector3 (background.anchoredPosition.x, -200f)
				: new Vector3 (background.anchoredPosition.x, 0f)
			;
		}

		// animate visibility
		// snap to final position if reached small wiggle room around it
		if (Mathf.Abs(background.position.y - backgroundTargetPosition.y) <= 0.01f) {
			background.anchoredPosition = backgroundTargetPosition;
		}
		// translate towards final show/hide position
		else {
			// refer to anchor position to include otherwise neglected offsets
			background.anchoredPosition = Vector3.Lerp (
				background.anchoredPosition,
				backgroundTargetPosition,
				Time.deltaTime * hidingSpeed
			);
		}

	}

}
