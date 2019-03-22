using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotStone : MonoBehaviour {
	// sprites and renderer
	public Sprite headstoneSprite;
	public Sprite headstoneMarkedSprite;
	SpriteRenderer renderer;

	void Awake() {
		renderer = this.GetComponent<SpriteRenderer> ();
		Hide ();
	}

	// turn finished mark on or off
	public void Mark () {
		renderer.enabled = true;
		renderer.sprite = headstoneMarkedSprite;
	}
	public void Show () {
		renderer.enabled = true;
		renderer.sprite = headstoneSprite;
	}

	// set invisible
	public void Hide () {
		renderer.enabled = false;
	}
}
