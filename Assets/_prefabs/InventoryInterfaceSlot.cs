using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryInterfaceSlot : MonoBehaviour {

	// pickup behavior associated with item
	public Pickup item;

	// reference the renderer for stored item icons
	// expect incoming stored item to have a sprite icon to render
	Image image;

	// image when not displaying item sprite
	public Sprite defaultSprite;

	void Start () {
		// grab renderer and display the starting sprite
		image = GetComponent<Image> ();
		image.sprite = defaultSprite;
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
	}

	// check if there is no pickup
	public bool IsEmpty () {
		return item == null;
	}

}
