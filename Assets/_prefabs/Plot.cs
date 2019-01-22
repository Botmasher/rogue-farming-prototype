using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour {

	// TODO methods for adding or updating a rogue and a castle

	// prefab with script for castle and rogue instantiation and reference
	public Rogue rogue;		// behavior for the rogue planted in this plot
	public Castle castle; 	// behavior for the castle a rogue fights through if planted in this plot

	// TODO make a listener class for queuing up callbacks on call, then send this one for "onLevelProgress" rogueId, castleId

	public Sprite spriteEmpty;
	public Sprite spritePlanted;
	public Sprite spriteGrowing;
	public Sprite spriteDone;
	public Sprite spriteCracked;

	// gather assigned sprite images into indexed grow sequence
	List<Sprite> sprites = new List<Sprite>();

	// leeway counter for demoing rogue actions against castle obstacles
	// used as safety valve to avoid passing more than one obstacle at a time to rogue 
	int cooldownTimer = 0;

	// async iterator through castle obstacle sequences
	int obstacleIndex = 0;
	string currentObstacle;
	int currentLevel = 0;

	// growth status
	bool isEmpty = true; 	// availability for planting
	int growthStage = 0; 	// once reaches 3 (Done) ready to pick

	void Start () {
		// add all sprites to the list
		sprites.Add (spriteEmpty);
		sprites.Add (spritePlanted);
		sprites.Add (spriteGrowing);
		sprites.Add (spriteDone);
		sprites.Add (spriteCracked);
		// set current sprite
		//GetComponent<SpriteRenderer> ().sprite = sprites[0];

		// NOTE demo setting rogue and castle from the getgo 
		//this.SetCaslte (castle);
		//this.SetRogue (rogue);
		if (castle) castle.resetCastle ();

		DayManager.Day.At("noon", () => Debug.Log("Logging a noontime message from a Plot outside DayManager"));
	}

	void Update () {
		
		// check if rogue is free for tasks
		if (rogue && !rogue.IsBusy () && cooldownTimer <= 0) {
			// keep iterating through levels of obstacles until castle sequence done
			if (currentLevel < castle.levelObstacles.Count) {
				if (obstacleIndex >= castle.levelObstacles [currentLevel].Count) {
					currentLevel++;
					obstacleIndex = 0;
				}
				if (currentLevel >= castle.levelObstacles.Count) {
					this.EndCastle ();
				}
				currentObstacle = castle.levelObstacles [currentLevel][obstacleIndex];
				obstacleIndex++;
			} else {
				currentObstacle = "None";
				this.EndCastle ();
			}

			// task rogue to interact with obstacle as expected
			if (castle.enemies.ContainsKey (currentObstacle)) {
				// get rogue to fight enemy
				rogue.AssignEnemy (currentObstacle, castle.enemies[currentObstacle]);
			} else if (castle.treasures.ContainsKey (currentObstacle)) {
				// get rogue to open treasure
				rogue.AssignTreasure (currentObstacle, castle.treasures[currentObstacle]);
			} else if (castle.hazards.ContainsKey (currentObstacle)) {
				// get rogue to evade hazard
				rogue.AssignHazard (currentObstacle, castle.hazards[currentObstacle]);
			} else {
				// unknown obstacle
				Debug.Log(string.Format("Castle does not seem to contain any obstacle named {0}", currentObstacle));
			}
			cooldownTimer = 10;
		}

		if (cooldownTimer > 0) {
			cooldownTimer--;
		}

	}

	public void EndCastle () {
		Debug.Log ("Congratulations! Your rogue finished the castle completely and utterly!");
		Application.Quit ();
	}

	public void SetCaslte(Castle castle) {
		this.castle = castle;
	}

	public void SetRogue(Rogue rogue) {
		this.rogue = rogue;
	}
}
