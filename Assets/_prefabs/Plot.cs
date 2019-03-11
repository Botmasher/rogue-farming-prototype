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

		// NOTE: set at the beginning for early demo before plots built out
		// if (castle) castle.ResetCastle ();

		// cyclical behavior for day manager
		growthAction = () => GrowRogue ();
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

	// TODO: 

	void GrowRogue () {
		// increment growth stage
		if (growthStageCurrent >= growthStageMax) {
			return;
		}

		// advance the growth stage
		growthStageCurrent++;

		// fetch current castle level info then advance castle level
		List<CastleObstacle> obstacles = castle.RunLevel();

		// send rogue obstacles to deal with 
		// TODO: feed rogue a dataful obstacle object living under the castle instead of a string it has to check
		if (rogue.isAlive) {
			foreach (CastleObstacle obstacle in obstacles) {
				rogue.FeedObstacle (obstacle);
			}
		} else {
			Debug.Log ("Planted rogue named '" + rogue.gameObject.name + "' has died!");
		}

		// TODO: track rogue progress and prep for ui display readout
		// example: "rogue conquered 2 levels, opened 3 chests, avoided 18 hazards, defeated 27 enemies and 2 bosses. Now struggling through level 3."
			
		Debug.Log ("Plot " + this.name + " grew its rogue one more day. Growth Stage: " + this.GrowthStage);

		// TODO: rot if too long or weather factors intervene

		return;
	}

	// Handle details of associating or unassociating rogue to this plot
	// - use during planting or harvesting of current rogue
	void SetRogue (GameObject newRogue) {
		rogue = newRogue.GetComponent<Rogue> ();
		rogue.transform.SetParent (this.transform);
		rogue.gameObject.SetActive (false);
	}
	GameObject UnsetRogue () {
		GameObject oldRogue = rogue.gameObject;
		rogue = null;
		rogue.transform.SetParent (null);
		rogue.gameObject.SetActive (true);
		return oldRogue;
	}

	// place rogue in plot and begin growing each day
	public bool PlantRogue(GameObject newRogue) {
		if (rogue == null && !GameObject.Equals(newRogue.GetComponent<Rogue> (), null)) {
			// attach incoming rogue to this plot
			SetRogue (newRogue);

			// set up day callback to start cycling growth
			plantedHour = DayManager.Day.EveryDay (growthAction);

			// make sure rogue is alive for obstacle progression
			rogue.Live();

			// initialize obstacles ("castle") to run rogue through
			castle.ResetCastle ();

			Debug.Log ("Planted rogue in Plot");

			return true;
		}
		return false;
	}

	// retrieve rogue from plot and reset growth
	public GameObject HarvestRogue () {
		// planted rogue ready for harvest
		if (rogue != null && growthStageCurrent == growthStageMax) {
			// reset planted rogue and castle scheduling
			growthStageCurrent = 0;
			DayManager.Day.NotAt (plantedHour, growthAction);
			plantedHour = -1;

			// hand rogue over to harvester
			return UnsetRogue ();
		}
		// no grown rogue to harvest
		return null;
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
