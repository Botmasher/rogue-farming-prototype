using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	// give unique name to weapon
	public string name;
	public string weaponType = "Sword";

	// hold visuals for each level to display in inventory
	public Sprite level1Sprite;
	public Sprite level2Sprite;
	public Sprite level3Sprite;
	public Sprite level4Sprite;
	public Sprite level5Sprite;
	public Sprite level6Sprite;
	private Sprite currentSprite;
	private List<Sprite> levelSprites;

	// weapon stats
	public int level = 1; 	// overall stat
	List<int> damageLevels = new List<int> () {
		1,
		3,
		6,
		12,
		24,
		48
	};
	public int damage; 		// potentially determine other stats individually (added to rogue base)

	// weapon level up
	public int currentXp = 0; 	// current xp on this weapon lvl
	public int levelXp; 		// total xp required to advance to next weapon lvl
	List<int> levelXps = new List<int> () {
		0,
		100,
		500,
		1200,
		3000,
		6000,
	};

	void Start() {
		// initialize weapon level and level-up data
		damage = damageLevels [level - 1]; 	// current level damage done
		levelXp = levelXps [level]; 		// next level XP requirement

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

		// build a name for the weapon
		Rename ();
	}

	// build armor name based on its level
	List<string> names = new List<string> () {
		"Cardboard",
		"Stone",
		"Silver",
		"Gold",
		"Diamond",
		"Carbon"
	};
	void Rename() {
		name = names [level - 1] + " " + weaponType;
	}
		
	// weapon leveling
	public void AddXP (int addedXp) {
		currentXp += addedXp;

		// TODO: handle leveling up multiple times with large dose of XP

		if (currentXp > levelXps[level]) LevelUp ();
	}
	public void LevelUp() {
		// augment weapon level unless reached level cap
		level = Mathf.Min(level + 1, levelXps.Count);

		// spend the XP cost to level up
		currentXp -= levelXps[level];
		// raise level XP cost to next level (read incl in inspector)
		levelXp = levelXps[level];

		// increase stats
		damage = damageLevels[level - 1];

		// select a new visual
		this.GetComponent<SpriteRenderer> ().sprite = levelSprites [level - 1];

		// build a new name
		Rename ();
	}

}
