using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotDirt : MonoBehaviour {
	// renderer and sprites
	public Sprite dirtSprite;
	public Sprite dirtSelectedSprite;
	SpriteRenderer renderer;

	void Awake() {
		renderer = this.GetComponent<SpriteRenderer> ();
		Deselect ();
	}

	// turn highlighting on or off
	public void Select () { renderer.sprite = dirtSelectedSprite; }
	public void Deselect () { renderer.sprite = dirtSprite; }

}
