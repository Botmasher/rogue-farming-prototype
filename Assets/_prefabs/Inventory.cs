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

	// vet item and determine whether and how to add it to inventory
	public bool AddItem(GameObject item) {
		// attempt to separate rogue into equipment pieces
		if (item.GetComponent <Rogue> ()) {
			// either split and add to inventory or throw back
			return TakeApart (item);
		}

		// add it to the items list if it fits
		else if (items.Count < itemLimit) {
			// store and parent to this object
			StoreWithinInventory (item);

			// add pickup to the UI
			inventoryUI.RefreshSlots (items);

			return true;
		}

		// did not fit
		return false;
	}

	// store pickup item in and parent item to inventory
	private void StoreWithinInventory (GameObject item) {
		if (!items.Contains (item)) {
			// add item to the list
			this.items.Add (item);

			// deactivate and nest item object under the Inventory
			item.transform.SetParent (this.transform);
			item.SetActive (false);

			Debug.Log ("Inventory length: " + items.Count);
		}
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

			// put the item back in the world
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

	// split one item into multiple in AddItem - used for rogue equipment
	private bool TakeApart (GameObject item) {
		// TODO: abstract out (as with AttachItem) for anything with attachments
		if (item.GetComponent<Rogue> ()) {
			Debug.Log ("Definitely dealing with a rogue here...");
			// grab rogue and its equipment 
			Rogue rogue = item.GetComponent <Rogue> ();
			GameObject weapon = rogue.weaponEquipment;
			GameObject armor = rogue.armorEquipment;

			// add up the equipment to be separated
			int piecesCount = 1; 	// at least a rogue
			piecesCount = weapon != null ? piecesCount + 1 : piecesCount + 0;
			piecesCount = armor != null ? piecesCount + 1 : piecesCount + 0;

			// avoid splitting and storing if rogue pieces do not fit in inventory
			if (items.Count + piecesCount > itemLimit) {
				return false;
			}

			// add rogue and equipment to inventory
			if (weapon != null) StoreWithinInventory (weapon);
			if (armor != null) StoreWithinInventory (armor);
			StoreWithinInventory (rogue.gameObject);

			// remove items from rogue now that they are stored in inventory
			rogue.weaponEquipment = null;
			rogue.armorEquipment = null;

			// reset inventory interface
			inventoryUI.RefreshSlots (items);

			// successfully added rogue and equipment
			return true;
		}
		// unable to add rogue and equipment
		return false;
	}

	// UI callable method to try to attach item under another - use to add Weapon and Armor to Rogue
	// return the new index of the original source item, either its original spot or its new attached spot
	public int AttachItem (int sourceIndex, int targetIndex) {
		// catch indexes out of items range
		if (sourceIndex + 1 > items.Count || targetIndex + 1 > items.Count) {
			return -1;
		}

		// grab the item attachment and the item to attach it to
		GameObject sourceItem = items [sourceIndex];
		GameObject targetItem = items [targetIndex];

		// TODO: abstract this to any attachables to anything with attachments
		// check target for a rogue
		if (targetItem.GetComponent<Rogue> () != null) {
			Rogue rogue = targetItem.GetComponent<Rogue> ();
			// make sure the source is a piece of weapon or armor equipment
			// and attach if a spot is available within the rogue
			if (sourceItem.GetComponent<Weapon> () && rogue.weaponEquipment == null) {
				rogue.weaponEquipment = sourceItem;
				// get rid of this item in slots since it's now part of rogue
				RemoveItemAt (sourceIndex);
				// return the index of the target if the two were combined
				return targetIndex;
			} else if (sourceItem.GetComponent<Armor> () && rogue.armorEquipment == null) {
				rogue.armorEquipment = sourceItem;
				return targetIndex;
			}
		}
		// return the index of the selected item if unable to attach
		return sourceIndex;
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

	// allow inventory to drag-drop switch places of two items
	// NOTE: use in limited way to drop items into free slot at the end of list
	public void SwapSlots(int slot1, int slot2) {
		int clampedSlot1 = this.ClampSlot (slot1);
		int clampedSlot2 = this.ClampSlot (slot2);
		GameObject swappedObject = this.items[clampedSlot2];
		this.items [clampedSlot2] = this.items [clampedSlot1];
		this.items [clampedSlot1] = swappedObject;
		return;
	}

}
