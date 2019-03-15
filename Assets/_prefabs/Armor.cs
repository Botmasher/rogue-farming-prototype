﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: determine how behavior differs from Weapon or consolidate both scripts
// 	- alternatively inherit/inject a base Equipment
public class Armor : MonoBehaviour {

	// give unique names to armor
	public string name;
	public string armorType = "Armor";

	// hold visuals for each level to display in inventory
	public Sprite level1Sprite;
	public Sprite level2Sprite;
	public Sprite level3Sprite;
	public Sprite level4Sprite;
	public Sprite level5Sprite;
	public Sprite level6Sprite;
	private Sprite currentSprite;
	private List<Sprite> levelSprites;

	// armor stats
	public int level = 1; 		// overall stat
	public int defense = 1; 	// potentially determine other stats individually (added to rogue base)

	void Start() {
		// put together visuals list for updating on level ups
		levelSprites = new List<Sprite> () {
			level1Sprite,
			level2Sprite,
			level3Sprite,
			level4Sprite,
			level5Sprite,
			level6Sprite
		};

		// display the lowest level sprite
		this.GetComponent<SpriteRenderer> ().sprite = levelSprites[0];

		// give the armor a name
		Rename();
	}

	// build armor name based on its level
	List<string> names = new List<string> () {
		"Cardboard",
		"Wood",
		"Stone",
		"Silver",
		"Gold",
		"Silk",
		"Carbon"
	};
	void Rename() {
		name = names[level - 1] + " " + armorType;
	}

	// armor leveling
	public void LevelUp() {
		// augment stats
		this.level++;

		// select a new visual if available
		if (this.level < levelSprites.Count) {
			this.GetComponent<SpriteRenderer> ().sprite = levelSprites [this.level - 1];
		}

		Rename ();
	}

}
