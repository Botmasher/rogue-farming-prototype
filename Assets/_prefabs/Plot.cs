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

	// leeway secs for demoing rogue actions against castle obstacles
	// used as safety valve to avoid passing more than one obstacle at a time to rogue 
	float cooldownTimer = 0f;

	// async iterator through castle obstacle sequences
	int obstacleIndex = 0;
	string currentObstacle;
	int currentLevel = 0;

	// plant and harvest status
	bool isEmpty = true; 		// availability for planting
	public bool IsEmpty {
		get {
			return this.isEmpty;
		}
	}
	int growthStageCurrent = 0;
	int growthStageMax = 3;
	public int GrowthStage {
		get {
			return this.growthStageCurrent;
		}
	}

	// storing actions and timing for communicating with day manager
	int plantedHour = -1;
	System.Action growthAction;

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

		// cyclical behavior for day manager
		growthAction = () => GrowRogue ();
	}

	void Update () {

		// TODO: simplify and abstract rogue castle runthrough
		// check if rogue is free for tasks
		if (rogue && !rogue.IsBusy () && cooldownTimer <= 0f) {
			// keep iterating through levels of obstacles until castle sequence done
			if (currentLevel < castle.levelObstacles.Count) {
				if (obstacleIndex >= castle.levelObstacles [currentLevel].Count) {
					currentLevel++;
					obstacleIndex = 0;
				}
//				if (currentLevel >= castle.levelObstacles.Count) {
//					this.EndCastle ();
//				}
				currentObstacle = castle.levelObstacles [currentLevel][obstacleIndex];
				obstacleIndex++;
			} else {
				currentObstacle = "None";
				//this.EndCastle ();
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
			cooldownTimer = 0.1f;
		}

		if (cooldownTimer > 0f) {
			cooldownTimer -= Time.deltaTime;
		}

	}

	// TODO: have rogue run through obstacles each day - see Update
	/*
	 * PLANTING
	 * - have grim pass rogue to plot on plant action
	 * - parent rogue, make it invisible, not and store rogue beneath plot
	 * GROWING
	 * - have castle throw obstacles of certain types at rogue
	 * - have rogue decide how to attempt those obstacles
	 * - advance through one level each day until dying or finished
	 * - consider: what should happen (storywise and growth/harvest) if rogue makes it all the way?
	 * HARVESTING
	 * - pop rogue back out into the world (not directly into grim inventory)
	 */
	private void GrowRogue () {
		// should rogue run through the whole level on growth stage? or perform actions every hour?
		// like: "rogue made it through level 1, opening W chests, defeating X enemies, Y treasures, Z bosses. Currently on level 2."
		growthStageCurrent++;
		return;
	}

	public GameObject HarvestRogue () {
		// planted rogue ready for harvest
		if (rogue != null && growthStageCurrent == growthStageMax) {
			// reset planted rogue and castle scheduling
			growthStageCurrent = 0;
			DayManager.Day.NotAt (plantedHour, growthAction);
			plantedHour = -1;

			// hand rogue over to harvester
			rogue = null;
			return rogue.gameObject;
		}
		// no grown rogue to harvest
		return null;
	}

	// place rogue in plot and begin growing each day
	public bool PlantRogue(GameObject newRogue) {
		if (rogue == null && !GameObject.Equals(newRogue.GetComponent<Rogue> (), null)) {
			rogue = newRogue.GetComponent<Rogue> ();

			// TODO: remove rogue object from inventory and place within plot

			plantedHour = DayManager.Day.EveryDay (growthAction);

			// initialize obstacles ("castle") to run rogue through
			castle.resetCastle ();

			return true;
		}
		return false;
	}

	// NOTE: first-pass rogue demo method for ending game once rogue runs through castle
//	public void EndCastle () {
//		Debug.Log ("Congratulations! Your rogue finished the castle completely and utterly!");
//		Application.Quit ();
//	}

	// TODO: just keep one castle through inspector/prefab for whenever plot instantiated
//	public void SetCaslte(Castle castle) {
//		this.castle = castle;
//	}

}
