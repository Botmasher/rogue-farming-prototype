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
	public Image renderer; 		// for rendering main sprite
	public Image bgRenderer;	// for sprite below main image
	public Image fgRenderer; 	// for sprite above main image

	// image when not displaying item sprite
	public Sprite defaultSprite;

	// whether currently selected interface slot
	bool selected = false;

	void Start () {
		// clear images and items
		Clear ();

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
		renderer.color = new Color (0.2f, 0.1f, 1f, 1f);
	}
	public void Deselect () {
		selected = false;
		renderer.color = Color.white;
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
		// clear out the slot
		Clear();

		// set the new item as main
		item = newItem;
	
		// update the image displayed in the ui
		if (newItem.GetComponent<SpriteRenderer> ()) {
			renderer.color = Color.white;
			renderer.sprite = newItem.GetComponent <SpriteRenderer> ().sprite;
		}

		// capture rogue, weapon or armor item for further evaluation
		Rogue rogue = newItem.GetComponent<Rogue> ();
		Weapon weapon = newItem.GetComponent<Weapon> ();
		Armor armor = newItem.GetComponent<Armor> ();

		// resize weapon item
		if (weapon != null) {
			renderer.rectTransform.localPosition = new Vector3 (-1f, 2f, 0f);
			renderer.rectTransform.sizeDelta = new Vector2 (55f, 115f);
			renderer.rectTransform.localRotation = Quaternion.Euler (Vector3.forward * 24f);
		}

		if (armor != null) {
			renderer.rectTransform.localPosition = new Vector3 (0f, 1f, 0f);
			renderer.rectTransform.sizeDelta = new Vector2 (75f, 100f);
		}

		// resize armor item

		// break out a rogue's weapon and armor to display
		if (rogue != null && rogue.equipment["weapon"] != null) {
			// render weapon image
			bgRenderer.color = Color.white;
			bgRenderer.sprite = rogue.equipment["weapon"].GetComponent<SpriteRenderer> ().sprite;

			// resize the attached weapon to fit behind rogue hand
			// TODO: avoid hardcoding this here
			bgRenderer.rectTransform.localPosition = new Vector3 (-32f, 10.5f, 0f);
			bgRenderer.rectTransform.sizeDelta = new Vector2 (35f, 78f);
		}
		if (rogue != null && rogue.equipment ["armor"] != null) {
			// set the armor image
			fgRenderer.color = Color.white;
			fgRenderer.sprite = rogue.equipment ["armor"].GetComponent<SpriteRenderer> ().sprite;

			// resize the attached armor to fit in front of rogue
			// TODO: avoid hardcoding this here
			fgRenderer.rectTransform.localPosition = new Vector3 (1.75f, 4.8f, 0f);
			fgRenderer.rectTransform.sizeDelta = new Vector2 (65f, 85f);
		}
	}

	// remove the stored item ui and reference
	public void Clear () {
		// empty out the ui images
		renderer.sprite = null;
		renderer.color = Color.clear;
		renderer.rectTransform.localPosition = Vector3.zero;
		renderer.rectTransform.sizeDelta = new Vector2(100f, 100f);
		renderer.rectTransform.rotation = Quaternion.Euler (Vector3.zero);

		bgRenderer.sprite = null;
		bgRenderer.color = Color.clear;
		bgRenderer.rectTransform.localPosition = Vector3.zero;
		bgRenderer.rectTransform.sizeDelta = new Vector2(100f, 100f);
		bgRenderer.rectTransform.rotation = Quaternion.Euler (Vector3.zero);

		fgRenderer.sprite = null;
		fgRenderer.color = Color.clear;
		fgRenderer.rectTransform.localPosition = Vector3.zero;
		fgRenderer.rectTransform.sizeDelta = new Vector2(100f, 100f);
		fgRenderer.rectTransform.rotation = Quaternion.Euler (Vector3.zero);

		// empty out the pickup
		item = null;

		// set the default sprite
		renderer.sprite = defaultSprite;
		renderer.color = Color.white;

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
			formattedStats += "<b>" + weapon.name + "</b>\n";
			formattedStats += "lvl: " + weapon.level + "\n";
			formattedStats += "dmg: " + weapon.damage + "\n";
			formattedStats += "xp: " + weapon.currentXp + "/" + weapon.levelXp;

		} else if (item.GetComponent<Armor> () != null) {
			Armor armor = item.GetComponent<Armor> ();
			formattedStats += "<b>" + armor.name + "</b>\n";
			formattedStats += "lvl: " + armor.level + "\n";
			formattedStats += "def: " + armor.defense + "\n";
			formattedStats += "xp: " + armor.currentXp + "/" + armor.levelXp;

		} else if (item.GetComponent<Rogue> () != null) {
			Rogue rogue = item.GetComponent<Rogue> ();
			formattedStats += "<b>" + rogue.name + "</b>\n";
			formattedStats += "health: " + rogue.maxHealth + "\n";
			formattedStats += "defense: " + rogue.defense + (rogue.armorEquipment ? " <b> + " + rogue.armorEquipment.GetComponent<Armor> ().defense +"</b>\n" : "\n");
			formattedStats += "attack: " + rogue.attack + (rogue.weaponEquipment ? " <b> + " + rogue.weaponEquipment.GetComponent<Weapon> ().damage +"</b>\n" : "\n");
			formattedStats += "agility: " + rogue.agility + "\n";
			formattedStats += "thievery: " + rogue.thievery + "\n";
			formattedStats += "luck: " + rogue.luck;

			// TODO: show armor and weapon info if those are equipped
			// 	- remove some rogue stats?
			// 	- add as + after armor and attack?

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
