using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

	// NOTE: the game splits real (possessed) items from their ui representations
	// 	- actual items in Inventory
	// 	- slots and items ui in InventoryInterface (separated out so that each item ui can handle its own logic and sprites)
	// 	- references to game objects are passed through into ui but intended for grabbing and passing back info about them

	// TODO: successfully store, put together and retrieve items
	// 	- for demo any "item" is one of rogue components: rogue wight (seriously? idk sounds fun), armor, weapon
	//  - simplify: a rogue has a weapon slot and an armor slot
	// 	- eventually consider things like non-equipment items: potions...
	// 	- eventually consider things like abilities: magic, skills, ...

	// TODO: interface displaying visuals for items
	// - update when armor, weapon attached to rogue
	// - rogue can be taken apart / put together for its three components
	// - rogue enters or leaves inventory with or without weapon and armor

	// inventory and items data
	List<GameObject> items = new List<GameObject> ();
	public int itemLimit = 10;
	int selectedIndex = -1;

	// ui reference for visualization
	// NOTE: keep list in sync with visualization list
	public InventoryInterface inventoryUI;


	/* Interact with inventory on input */

	// TODO: player input hits inventory slot (GET FROM InventoryInterface)
	public void SelectSlot(int slot) {
		selectedIndex = slot;
	}


	/* Manage list data */

	// TODO: deal with selected index and ui selected slot
	void RemoveItem(GameObject item) {
		items.Remove (item);
		// reactivate item and unparent from carrier
		item.transform.parent = null;
		item.SetActive (true);
	}

	// pass item into inventory
	public bool AddItem(GameObject item) {
		// add it to the items list if it fits
		if (items.Count < itemLimit && !items.Contains(item)) {
			// add pickup to the list
			this.items.Add (item);

			// deactivate and nest item object under the Inventory
			item.transform.SetParent (this.transform);
			item.SetActive (false);

			// add pickup to the UI
			inventoryUI.RefreshSlots (items);

			return true;
		}

		// did not fit
		return false;
	}

	// TODO: plant in plot or affect player instead of putting it back in the world
	// NOTE: a "DropSelected" would be more fitting for current procedure
	//
	// fetch currently selected object and perform its action
	public GameObject UseSelected() {
		// only operate on known indexes
		if (selectedIndex >= 0 && selectedIndex < items.Count) {

			// retrieve the selected item
			GameObject item = items[selectedIndex];

		//	// put the item back in the world
			items.RemoveAt (selectedIndex);
			item.transform.parent = null;
			item.SetActive (true);
		
			// reset the inventory ui
			inventoryUI.RefreshSlots (items);

			// return a reference to the item
			return item;
		}
		return null;
	}

	// read the inventory count
	public int Length() {
		return items.Count;
	}

	// read the storage limit - used to sync number of ui slots
	public int Limit() {
		return itemLimit;
	}


	/* 	Collection methods so far underdeveloped
	 *
	 * 	/!\ all below is at risk of heavy change or removal /!\
	 */

	// NOTE: remove automatically drops the element from the list; keeping it null could facilitate spacing/unspacing option
	void UnspaceItems() {
		List<GameObject> unspacedItems = new List<GameObject> ();
		//items.ForEach (item => Debug.Log (item));
	}

	GameObject RemoveItemAt(int slot) {
		int clampedSlot = this.ClampSlot (slot);
		items.RemoveAt (clampedSlot);
		GameObject item = items [clampedSlot];
		return item;
	}

	int ClampSlot(int slot) {
		return Mathf.Clamp(slot, 0, this.items.Count);
	}

	GameObject RequestSlot(int slot) {
		int clampedSlot = this.ClampSlot (slot);
		return this.items [clampedSlot];
	}

	void SwapSlots(int slot1, int slot2) {
		int clampedSlot1 = this.ClampSlot (slot1);
		int clampedSlot2 = this.ClampSlot (slot2);
		GameObject swappedObject = this.items[clampedSlot2];
		this.items [clampedSlot2] = this.items [clampedSlot1];
		this.items [clampedSlot1] = swappedObject;
		return;
	}

}
