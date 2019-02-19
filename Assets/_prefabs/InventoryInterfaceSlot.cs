using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryInterfaceSlot : MonoBehaviour {

	// pickup behavior associated with item for grabbing its sprite
	public Pickup item;

	// reference the renderer for stored item icons
	// expect incoming stored item to have a sprite icon to render
	Image image;

	// image when not displaying item sprite
	public Sprite defaultSprite;

	// 
	bool selected = false;

	void Start () {
		// grab renderer and display the starting sprite
		image = GetComponent<Image> ();
		image.sprite = defaultSprite;
	}

	// set selection status and color of display
	public void Select () {
		selected = true;
		image.color = Color.red;
	}
	public void Deselect () {
		selected = false;
		image.color = Color.white;
	}
	public void Toggle () {
		// switch selection status for choosing the same slot multiple times
		if (!selected) {
			Select ();
		} else {
			Deselect ();
		}
	}

	// point to an inventory object
	public void Store (GameObject newItem) {
		item = newItem.GetComponent<Pickup> ();
		// update the image displayed in the ui
		image.sprite = item.inventorySprite;
	}

	// send back and remove the pointed object
	public void Clear () {
		// empty out the ui image and the pickup
		image.sprite = defaultSprite;
		item = null;
		// maintain selection status to allow toggling
		//Deselect();
	}

	// check if there is no pickup
	public bool IsEmpty () {
		return item == null;
	}

	// return the original item stored as the pickup
	public GameObject Item () {
		return item.gameObject;
	}

}
