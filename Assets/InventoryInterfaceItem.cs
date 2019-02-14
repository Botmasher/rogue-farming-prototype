using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryInterfaceItem : MonoBehaviour {

	// pickup behavior associated with item
	public Pickup item;

	// expect stored items to have an icon - reference the rendering component
	Image image;

	void Start () {
		// rendering component for setting sprite
		image = GetComponent<Image> ();
	}

	// point to an inventory object
	public void Store (GameObject newItem) {
		item = newItem.GetComponent<Pickup> ();
		// update the image displayed in the ui
		image.sprite = item.inventorySprite;
	}

	// send back and remove the pointed object
	public GameObject Retrieve () {
		return item.gameObject;
		// empty out the ui image
		image.sprite = null;
	}

	public void Clear () {
		item = null;
	}

	// check if there is no pickup
	public bool IsEmpty () {
		return item == null;
	}

}
