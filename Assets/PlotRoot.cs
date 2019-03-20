using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotRoot : MonoBehaviour {

	// sprites for various growth states
	public Sprite growthSprite0;
	public Sprite growthSprite1;
	public Sprite growthSprite2;
	public Sprite growthSprite3;
	public Sprite growthSprite4;
	List<Sprite> growthSprites = new List<Sprite> ();

	// reference to sprite renderer
	SpriteRenderer renderer;

	void Awake () {
		renderer = this.GetComponent<SpriteRenderer> ();

		// build list of state sprites
		growthSprites.Add (growthSprite0);
		growthSprites.Add (growthSprite1);
		growthSprites.Add (growthSprite2);
		growthSprites.Add (growthSprite3);
		growthSprites.Add (growthSprite4);

		// hide the sprite for starters
		SetGrowthSprite (-1);
	}

	// render sprite from sprites list based on growth stage index
	/// <summary>
	/// Renders a sprite from growth sprites list based on growth stage index. Negative index disables sprite (invisible). Index out of range clamps to last index.
	/// </summary>
	/// <param name="growthStage">Growth stage index of sprite in list.</param>
	public void SetGrowthSprite (int growthStage) {
		// set sprite invisible for negative growth stages
		// example: set to -1 in order to render no sprite
		if (growthStage < 0) {
			renderer.enabled = false;
			return;
		}

		// render the current growth stage sprite or the 
		renderer.enabled = true;
		renderer.sprite = growthSprites [Mathf.Clamp(growthStage, 0, growthSprites.Count)];
	}

}
