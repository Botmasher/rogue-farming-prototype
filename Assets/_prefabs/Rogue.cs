﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue : MonoBehaviour {

	// TODO: implement feedback from rogue-castle playtest
	// - gold countup / reporting and logging not incrementing correctly (pile took treasure from 0 to 9)
	// - the names were fun though
	// - bosses (and minibosses?) are evadable exactly as often as enemies
	// - lockout period prohibited clean manual playthrough

	// main stat for rogue life - deplete this to end planting
	int health = 50;
	public bool isAlive = true;

	// TODO generate unique name for each rogue
	string name = "Ruffus the Tuffust";

	// determine skills gained in castle for boosts
	int enemyPoints = 0;
	int treasurePoints = 0;
	int hazardPoints = 0;

	// flag when available for obstacle assignment
	bool isFighting = false;
	bool isEvading = false;
	bool isThieving = false;
	// stats for the assigned obstacle
	string enemyName = "";
	int enemyAttack = 0;
	int enemyHealth = 0;
	string treasureName = "";
	int treasureTrickiness = 0;
	string hazardName = "";
	int hazardAttack = 0;

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

	void Update() {
		// assess health
		if (this.health <= 0) {
			this.health = 0;
			this.isAlive = false;
			Debug.Log (string.Format("Rogue {0} has perished in the Castle!\nPress Q to Quit", this.name));
		}

		// take actions over time
		if (this.actionCounter <= 0) {
			if (this.isFighting && Input.GetKeyDown (KeyCode.F)) {
				this.FightEnemy ();
				this.actionCounter = 160;
			} else if (this.isEvading && Input.GetKeyDown (KeyCode.E)) {
				this.EvadeHazard ();
				this.actionCounter = 160;
			} else if (this.isThieving && Input.GetKeyDown (KeyCode.O)) {
				this.OpenTreasure ();
				this.actionCounter = 160;
			} else if (!this.isAlive && Input.GetKeyDown (KeyCode.Q)) {
				Application.Quit ();
			} else {
			}
		}

		if (this.actionCounter > 0) {
			this.actionCounter--;
		}
	}

	// external check if available for assignment
	public bool IsBusy () {
		return (this.isFighting || this.isEvading || this.isThieving);
	}

	public void AssignEnemy (string enemyName, int strength) {
		this.enemyName = enemyName;
		this.enemyAttack = strength;
		this.enemyHealth = strength;
		this.isFighting = true;
		Debug.Log (string.Format("Rogue {0} is taking up arms against a {1}", this.name, enemyName));
		Debug.Log ("Press F to Fight!");
		//this.actionCounter = 120;
	}

	public void AssignTreasure (string treasureName, int trickiness) {
		this.treasureName = treasureName;
		this.treasureTrickiness = trickiness;
		this.isThieving = true;
		Debug.Log (string.Format("Rogue {0} is struggling to open a {1}", this.name, treasureName));
		Debug.Log ("Press O to Open!");
		//this.actionCounter = 120;
	}

	public void AssignHazard (string hazardName, int attack) {
		this.hazardName = hazardName;
		this.hazardAttack = attack;
		this.isEvading = true;
		Debug.Log (string.Format("Rogue {0} is craftily evading a {1}", this.name, hazardName));
		Debug.Log ("Press E to Evade!");
		//this.actionCounter = 120;
	}

	void FightEnemy() {
		// roll for a zero-to-one plus luck chance
		float rollPlusLuck = Random.Range (0f, 1f + this.luck);

		if (rollPlusLuck > 0.8f) {
			Debug.Log (string.Format("{0} ran away from a {1}", this.name, enemyName));
			this.DefeatEnemy();
			return;
		}

		// roll again so not all events in a single turn are lucky or unlucky
		rollPlusLuck = Random.Range (0f, 1f + this.luck);

		// attack enemy - leave a small chance to miss
		if (rollPlusLuck > 0.08f) {
			this.enemyHealth -= this.attack;
			Debug.Log (string.Format("Did {0} damage - enemy {1}/{2}", this.attack, this.enemyHealth, this.enemyAttack));
		} else {
			Debug.Log (string.Format("Missed - enemy {0}/{1}", this.enemyHealth, this.enemyAttack));
		}

		// finish off dead enemy
		if (this.enemyHealth <= 0) {
			Debug.Log (string.Format("{0} defeated a {1}!", this.name, enemyName));
			this.DefeatEnemy ();
			return;
		}

		// enemy attacks back - leave a small chance to evade
		if (rollPlusLuck > 0.1f) {
			int attackStrength = Mathf.RoundToInt(this.enemyAttack - (this.enemyAttack * this.luck) - this.armor);
			this.health -= attackStrength;
			// chip away at the armor over time instead of knocking it all off
			this.armor = Mathf.Clamp(
				Mathf.RoundToInt(this.armor + (this.armor * this.luck) - (attackStrength * 0.1f)),
				0,
				this.armor
			);
			Debug.Log (string.Format("Enemy attacked - rogue health dropped to {0}", this.health));
		} else {
			Debug.Log (string.Format("Evaded enemy attack - rogue health remains at {0}", this.health));
		}

		Debug.Log ("Press F to Fight even more!");
		//this.actionCounter = 60;
	}

	// TODO log progress below in completion methods

	// unassign and reset currently confronted enemy
	void DefeatEnemy () {
		// store skills gained
		this.enemyPoints += this.enemyAttack;
		// reset enemy
		this.enemyName = "";
		this.enemyAttack = 0;
		this.enemyHealth = 0;
		this.isFighting = false;
	}

	// unassign and reset treasure being opened
	void OpenTreasure () {
		// roll for a zero-to-one plus luck chance
		float rollPlusLuck = Random.Range (0f, 1f + this.luck);

		// be sly or lucky enough to open
		if ((this.thievery >= this.treasureTrickiness) || (rollPlusLuck > (0.85f + (this.treasureTrickiness/100)))) {
			this.treasure += this.treasureTrickiness;
			Debug.Log (string.Format ("Pried it! The {0} gold ups rogue treasure to {1}", this.treasureTrickiness, this.treasure));
		} else {
			Debug.Log ("Failed to crack it open. Not yet thieverious enough for its secrets.");
		}

		// store loot and skills gained
		this.treasurePoints += this.treasureTrickiness;
		this.treasure += this.treasureTrickiness;
		// reset treasure
		this.treasureName = "";
		this.treasureTrickiness = 0;
		this.isThieving = false;
	}

	// unassign and reset hazard facing rogue
	void EvadeHazard () {
		// roll for a zero-to-one plus luck chance
		float rollPlusLuck = Random.Range (0f, 1f + this.luck);

		// be agile or lucky enough to evade
		if ((this.agility >= this.hazardAttack) || (rollPlusLuck > (0.3f + (this.hazardAttack/100)))) {
			Debug.Log (string.Format("Smoothly sidestepped it! Health still {0}", this.health));
		} else {
			Debug.Log (string.Format("Stumbled into the {0} - health fell to {1}", this.hazardName, this.health));
		}

		// store skills gained
		this.hazardPoints += this.hazardAttack;
		// reset treasure
		this.hazardName = "";
		this.hazardAttack = 0;
		this.isEvading = false;
	}
}