using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	// TODO: give unique names to weapons
	public string name = "Test Weapon";

	// hold visuals for each level to display in inventory
	public Sprite level1Sprite;
	public Sprite level2Sprite;
	public Sprite level3Sprite;
	public Sprite level4Sprite;
	public Sprite level5Sprite;
	private Sprite currentSprite;
	private List<Sprite> levelSprites;

	// weapon stats
	public int level = 1; 	// overall stat
	public int damage = 1; 	// potentially determine other stats individually (added to rogue base)

	void Start() {
		// put together visuals list for updating on level ups
		levelSprites = new List<Sprite> () {
			level1Sprite,
			level2Sprite,
			level3Sprite,
			level4Sprite,
			level5Sprite
		};

		// display the lowest level sprite
		this.GetComponent<SpriteRenderer> ().sprite = levelSprites[0];
	}

	// weapon leveling
	public void LevelUp() {
		// augment stats
		this.level++;

		// select a new visual if available
		if (this.level < levelSprites.Count) {
			this.GetComponent<SpriteRenderer> ().sprite = levelSprites [this.level - 1];
		}
	}

}
