using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue : MonoBehaviour {

	// TODO: (? FeedObstacle) use castle int stats for strength and toughness, or for chests as theft
	// - rogue will do one attempt, take a hit to a stat, then do another attempt
	// - hits to armor reduce damage to health, but health still always hit
	// - attempts hit from weapon stat or from thievery stat
	// - successful treasures add to gold stash to be brought back

	// TODO: implement feedback from rogue-castle playtest
	// - gold countup / reporting and logging not incrementing correctly (pile took treasure from 0 to 9)
	// - the names were fun though
	// - bosses (and minibosses?) are evadable exactly as often as enemies
	// - lockout period prohibited clean manual playthrough

	// main stat for rogue life - deplete this to end planting
	public int health = 50;		// updated while adventuring
	int maxHealth = 50; 	// intended to be read when brought back to life - ? updated through stats or items
	public bool isAlive = true;

	// check for currently acting
	// NOTE: if updated after frame, cannot do async obstacles - consider coroutine for life/death actions like taking damage
	public bool isBusy = false;

	// weaponry and armor storage
	public Dictionary <string, GameObject> equipment = new Dictionary<string, GameObject> () {
		{ "armor", null },
		{ "weapon", null }
	};
	// starting equipment
	public GameObject armorEquipment;
	public GameObject weaponEquipment;

	// TODO: generate unique name for each rogue
	public string name = "Asdf";

	// determine skills gained in castle for boosts
	int enemyPoints = 0;
	int treasurePoints = 0;
	int hazardPoints = 0;

	// simulate time-taking countdown for rogue actions
	int actionCounter = 0;

	// starting stats - level up as fight through castles
	int armor = 1;
	int thievery = 1;
	int attack = 1;
	int agility = 1;

	// collected this pass through castle and brought to reaper
	int treasure = 0;

	// factor pushing events in your favor
	// TODO go through lucky events and add to luck meter
	float luck = 0.05f;

	void Start () {
		// store starting equipment
		if (weaponEquipment != null) equipment["weapon"] = weaponEquipment;
		if (armorEquipment != null) equipment ["armor"] = armorEquipment;
	}

	// TODO: make obstacles GameObjects to combine features instead of balancing dictionaries and lists
	public bool FeedObstacle (CastleObstacle obstacle) {

		// TODO: just assign the obstacle, even let it communicate back to call the right methods in rogue

		// assign and react to the obstacle depending on its type
		switch (obstacle.obstacleType) {
			case "hazard":
				EvadeHazard (obstacle);
				break;

			case "enemy":
			case "boss":
			case "miniBoss":
			case "finalBoss":
				FightEnemy (obstacle);
				break;

			case "treasure":
				OpenTreasure (obstacle);
				break;

			default:
				return false;
		}

		return true;
	}

	// attach item to rogue equipment spot - created for Grim inventory management
	public bool Equip (GameObject item, bool switchOut=false) {
		// make sure the item is a piece of weapon or armor equipment
		// and attach if a spot is available within the rogue

		// attach weapon item to behavior and object
		if (item.GetComponent<Weapon> () && (equipment["weapon"] == null || switchOut)) {
			equipment["weapon"] = item;
			item.transform.SetParent (this.transform);
			return true;
		}
		// attach armor item to behavior and object
		else if (item.GetComponent<Armor> () && (equipment["armor"] == null || switchOut)) {
			equipment["armor"] = item;
			item.transform.SetParent (this.transform);
			return true;
		}
		// unequippable item 
		else {
			return false;
		}
	}

	// remove item from rogue equipment spot - for Grim inventory
	public bool Unequip (string itemKey=null) {
		// remove specific equipment entry
		if (equipment.ContainsKey (itemKey) && equipment[itemKey] != null) {
			equipment [itemKey] = null;
			return true;
		}
		// no valid equipment key or equipment entry
		return false;
	}

	// fetch equipment objects - created for Grim inventory
	public GameObject GetWeapon () {
		return equipment ["weapon"];
	}
	public GameObject GetArmor () {
		return equipment ["armor"];
	}

	// 
	public void Live() {
		isAlive = true;
		health = maxHealth;
	}
	public void Die() {
		isAlive = false;
		health = 0;
		Debug.Log (string.Format("Rogue {0} has perished in the Castle!", this.name));
	}

	/* Adventuring through generated obstacles */

	void FightEnemy(CastleObstacle enemy) {
		// reference relevant values from enemy
		string enemyName = enemy.obstacleName;
		string enemyType = enemy.obstacleType;
		int enemyHealth = enemy.obstacleValue;
		int enemyAttack = enemy.obstacleValue;

		Debug.Log (string.Format("Rogue {0} is taking up arms against a {1}", this.name, enemyName));

		// roll for a zero-to-one plus luck chance for immediate evasion
		float rollPlusLuck = Random.Range (0f, 1f + this.luck);

		// attempt to run away from a low-level enemy
		if ((enemyType == "enemy" || enemyType == "miniBoss") && rollPlusLuck > 0.8f) {
			Debug.Log (string.Format("{0} ran away from a {1}", this.name, enemyName));
			this.DefeatEnemy(enemyAttack);
			return;
		}

		// play through encounter taking turns fighting and defending against the enemy
		CycleAttackDefend (enemyName, enemyType, enemyAttack, enemyHealth);
	}

	// handle enemy encounter until encounter resolves (rogue kills/evades enemy, enemy kills rogue)
	void CycleAttackDefend (string enemyName, string enemyType, int enemyAttack, int enemyHealth) {
		// roll again so not all events in a single turn are lucky or unlucky
		float rollPlusLuck = Random.Range (0f, 1f + this.luck);

		// attack enemy - leave a small chance to miss
		if (rollPlusLuck > 0.08f) {
			enemyHealth -= this.attack;
			Debug.Log (string.Format("Did {0} damage to enemy {1} - {2}/{3}", this.attack, enemyName, enemyHealth, enemyAttack));
		} else {
			Debug.Log (string.Format("Missed enemy {0} - {1}/{2}", enemyName, enemyHealth, enemyAttack));
		}

		// finish off dead enemy
		if (enemyHealth <= 0) {
			Debug.Log (string.Format("{0} defeated a {1}!", this.name, enemyName));
			this.DefeatEnemy (enemyAttack);
			return;
		}

		// enemy attacks back - leave a small chance to evade non-bosses
		if (rollPlusLuck > 0.1f || enemyType != "enemy") {
			int attackStrength = Mathf.RoundToInt(enemyAttack - (enemyAttack * this.luck) - this.armor);
			this.health -= attackStrength;
			// chip away at rogue armor over time instead of knocking it all off
			this.armor = Mathf.Clamp(
				Mathf.RoundToInt(this.armor + (this.armor * this.luck) - (attackStrength * 0.1f)),
				0,
				this.armor
			);
			Debug.Log (string.Format("Enemy attacked - rogue health dropped to {0}", this.health));
		} else {
			Debug.Log (string.Format("Evaded enemy attack - rogue health remains at {0}", this.health));
		}

		if (this.health <= 0) {
			Die ();
		}

		// take another turn if enemy still alive
		if (enemyHealth > 0 && this.health > 0) {
			Debug.Log ("Taking another attack-defend turn against enemy " + enemyName);
			CycleAttackDefend (enemyName, enemyType, enemyAttack, enemyHealth);
		}
		return;
	}

	// unassign and reset currently confronted enemy
	void DefeatEnemy (int enemyPoints) {
		// store skills gained
		this.enemyPoints += enemyPoints;
	}

	// unassign and reset treasure being opened
	void OpenTreasure (CastleObstacle treasure) {
		// peel off relevant treasure settings
		string treasureName = treasure.obstacleName;
		int treasureTrickiness = treasure.obstacleValue;

		Debug.Log (string.Format("Rogue {0} is struggling to open a {1}", this.name, treasureName));

		// roll for a zero-to-one plus luck chance
		float rollPlusLuck = Random.Range (0f, 1f + this.luck);

		// be sly or lucky enough to open
		if ((this.thievery >= treasureTrickiness) || (rollPlusLuck > (0.85f + (treasureTrickiness/100)))) {
			this.treasure += treasureTrickiness;
			Debug.Log (string.Format ("Pried it! The {0} gold ups rogue treasure to {1}", treasureTrickiness, this.treasure));
		} else {
			Debug.Log ("Failed to crack it open. Not yet thieverious enough for its secrets.");
		}

		// store loot and skills gained
		this.treasurePoints += treasureTrickiness;
		this.treasure += treasureTrickiness;
	}

	// unassign and reset hazard facing rogue
	void EvadeHazard (CastleObstacle hazard) {
		// peel off relevant hazard values
		string hazardName = hazard.obstacleName;
		int hazardAttack = hazard.obstacleValue;
		
		Debug.Log (string.Format("Rogue {0} is craftily evading a {1}", this.name, hazardName));

		// roll for a zero-to-one plus luck chance
		float rollPlusLuck = Random.Range (0f, 1f + this.luck);

		// be agile or lucky enough to evade
		if ((this.agility >= hazardAttack) || (rollPlusLuck > (0.3f + (hazardAttack/100)))) {
			Debug.Log (string.Format("Smoothly sidestepped it! Health still {0}", this.health));
		} else {
			this.health -= hazardAttack;
			Debug.Log (string.Format("Stumbled into the {0} - health fell to {1}", hazardName, this.health));
		}

		// store skills gained
		this.hazardPoints += hazardAttack;
	}
}
