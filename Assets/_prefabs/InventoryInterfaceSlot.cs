using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryInterfaceSlot : MonoBehaviour {

	// pickup behavior associated with item for grabbing its sprite
	public GameObject item;

	// displaying popups for slot item stats
	public GameObject statsUI;

	// reference the renderer for stored item icons
	// expect incoming stored item to have a sprite icon to render
	Image image;

	// image when not displaying item sprite
	public Sprite defaultSprite;

	// whether currently selected interface slot
	bool selected = false;

	void Start () {
		// grab renderer and display the starting sprite
		image = GetComponent<Image> ();
		image.sprite = defaultSprite;

		// spawn stats ui within the inventory slot
		statsUI = GameObject.Instantiate (statsUI);
		statsUI.transform.SetParent (this.transform);
		statsUI.transform.position = new Vector3 (
			transform.position.x + statsUI.transform.position.x,
			transform.position.y + statsUI.transform.position.y,
			transform.position.z + statsUI.transform.position.z
		);

		// hide UI for now
		statsUI.SetActive (false);
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
		item = newItem;
		// update the image displayed in the ui
		image.sprite = newItem.GetComponent <SpriteRenderer> ().sprite;
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

	// bring up UI on hover over - called via inspector
	public void ShowItemStatsUI () {
		// verify there is an item to show stats for
		if (item == null) {
			return;
		}

		// reactivate popup
		statsUI.SetActive (true);

		// build up stats text to display
		Text statsText = statsUI.GetComponentInChildren<Text> ();
		string formattedStats = "";

		int level;
		int mainValue;
		//int secondaryValue;
		//int worth;

		// TODO: format stats from within each item script instead

		if (item.GetComponent<Weapon> () != null) {
			Weapon weapon = item.GetComponent<Weapon> ();
			formattedStats += weapon.name + "\n\n";
			formattedStats += "lvl: " + weapon.level + "\n";
			formattedStats += "dmg: " + weapon.damage + "\n";

		} else if (item.GetComponent<Armor> () != null) {
			Armor armor = item.GetComponent<Armor> ();
			formattedStats += armor.name + "\n\n";
			formattedStats += "lvl: " + armor.level + "\n";
			formattedStats += "def: " + armor.defense + "\n";

		} else if (item.GetComponent<Rogue> () != null) {
			Rogue rogue = item.GetComponent<Rogue> ();
			formattedStats += rogue.name + "\n";
			formattedStats += "arm: " + rogue.armor + "\n";
			formattedStats += "att: " + rogue.attack + "\n";
			formattedStats += "agl: " + rogue.agility + "\n";
			formattedStats += "thv: " + rogue.thievery + "\n";
			formattedStats += "lck: " + rogue.luck + "\n";

		} else {
			formattedStats = item.name;
		}

		statsText.text = formattedStats;


	}
	// close UI on hover exit - called via inspector
	public void HideItemStatsUI () {
		statsUI.SetActive (false);
	}

}
