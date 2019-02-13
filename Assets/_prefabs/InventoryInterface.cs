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
	public Vector3 slotStartPosition;
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
				? slotList[i - 1].rectTransform.anchoredPosition3D.x
				: slotStartPosition.x
			;
			
			// create and store slot
			slotList.Add (GameObject.Instantiate (slot) as Image);
			itemList.Add (GameObject.Instantiate (slotItem) as Image);

			// parent to keep slot and item ui under inventory in canvas
			slotList [i].rectTransform.SetParent (background.transform);
			itemList [i].rectTransform.SetParent (slotList [i].transform);

			// position slot along the row
			slotList [i].rectTransform.anchoredPosition3D = new Vector3 (
				slotPositionX + slotSpacingHorizontal,
				slotStartPosition.y,
				slotStartPosition.z
			);

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

}
