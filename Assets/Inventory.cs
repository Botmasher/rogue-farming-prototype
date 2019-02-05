using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

	// inventory and items data
	List<GameObject> items = new List<GameObject> ();
	int limit = 12;
	int selected = 0;
	bool visible = false;

	// visuals
	public int gridHeight = 2; 	// split listed items into rows
	int gridWidth; 				// number of columns needed for rows

	void Start () {
		this.gridWidth = this.limit / this.gridHeight;
		this.items.Add (this.gameObject);
		this.items.Add (this.gameObject);
		Debug.Log (this.items[1]);
		this.items.RemoveAt (1);
		Debug.Log (this.items[1]);
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
