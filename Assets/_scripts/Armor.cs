using System.Collections;
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
	public int defense; 		// potentially determine other stats individually (added to rogue base)
	List<int> defenseLevels = new List<int> () {
		1,
		3,
		6,
		12,
		24,
		48
	};

	// armor level up
	public int currentXp = 0; 	// current xp on this weapon lvl
	public int levelXp; 		// total xp required to advance to next weapon lvl
	List<int> levelXps = new List<int> () {
		0,
		100,
		500,
		1200,
		3000,
		6000
	};

	void Start() {
		// initialize levelup
		defense = defenseLevels[level - 1];		// protection offered at current level
		levelXp = levelXps [level]; 			// XP required to reach the next level

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
	public void AddXP (int addedXp) {
		currentXp += addedXp;

		// TODO: handle leveling up multiple times with large dose of XP

		if (currentXp > levelXps [level]) {
			LevelUp ();
		}
	}
	public void LevelUp() {
		// augment armor unless reached level cap
		level = Mathf.Min(level + 1, levelXps.Count);

		// XP management
		//
		// spend the XP cost to level up
		currentXp -= levelXps[level];
		// raise level XP cost to next level (read incl in inspector)
		levelXp = levelXps[level];

		// Data and visuals
		//
		// augment the stats to current level
		defense = defenseLevels[level - 1];
		// select a new visual
		this.GetComponent<SpriteRenderer> ().sprite = levelSprites [level - 1];
		// get the new name
		Rename ();
	}

}
