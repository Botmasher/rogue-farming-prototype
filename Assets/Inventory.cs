using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

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
	int limit = 12;
	int selected = 0;
	bool visible = false;

	// visuals
	public int gridHeight = 2; 	// split listed items into rows
	int gridWidth; 				// number of columns needed for rows

	void Start () {
		// count inventory columns for rough visual balance of ui spacing given current number of rows
		this.gridWidth = this.limit / this.gridHeight;
	}

	// NOTE: remove automatically drops the element from the list; keeping it null could facilitate spacing/unspacing option
	void UnspaceItems() {
		List<GameObject> unspacedItems = new List<GameObject> ();
		//items.ForEach (item => if (item));
	}

	GameObject DropItemAt(int slot) {
		int clampedSlot = this.ClampSlot (slot);
		items.RemoveAt (clampedSlot);
		GameObject item = items [clampedSlot];
		return item;
	}

	void DropItem(GameObject item) {
		items.Remove (item);
	}

	void AddItem(GameObject item) {
		if (items.Count < limit) {
			this.items.Add (item);
		}
	}

	// TODO: handle messages in individual item slot objects
	void SelectedByGrim() {
		this.selected = 1;
	}

	void ToggleVisibility() {
		this.visible = !this.visible;
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
