using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plot : MonoBehaviour {

	// prefab with script for castle and rogue instantiation and reference
	public Rogue rogue;		// behavior for the rogue planted in this plot
	public Castle castle; 	// behavior for the castle a rogue fights through if planted in this plot

	// sprite rendering scripts for changing up plot visuals
	// expect this plot to have one of each behavior components its children
	PlotStone stoneRenderer; 	// display plot headstone
	PlotDirt dirtRenderer;		// display plot soil
	PlotRoot growthRenderer; 	// display plot plant in growth stages

	// epitaph ui for displaying plot status
	GameObject epitaph;

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

	// sound effects
	AudioSource audio; 				// store reference to audio player component
	public AudioClip sfxPlant;
	public AudioClip sfxGrow;
	public AudioClip sfxHarvest;
	public AudioClip sfxEpitaph;

	// harvest check
	bool isHarvestable = false;

	// storing actions and timing for communicating with day manager
	int plantedHour = -1;
	System.Action growthAction;

	void Awake () {
		// get epitaph ui reference
		epitaph = GameObject.FindGameObjectWithTag("Epitaph");

		// reference to audio player
		audio = GetComponent<AudioSource> ();

		// grab child renderer components
		stoneRenderer = GetComponentInChildren<PlotStone> ();
		dirtRenderer = GetComponentInChildren<PlotDirt> ();
		growthRenderer = GetComponentInChildren<PlotRoot> ();
	}

	void Start () {
		// cyclical behavior for day manager
		growthAction = () => GrowRogue ();
	}


	/* Phases
	 * 
	 * PLANTING
	 * - have grim pass rogue to plot on plant action
	 * - parent rogue, make it invisible, not and store rogue beneath plot
	 * 
	 * GROWING
	 * - castle throws obstacles of certain types at rogue
	 * - rogue decides how to attempt those obstacles
	 * - plot coordinates this, advancing through one level each day until rogue dies or finishes castle
	 * 
	 * HARVESTING
	 * - pop rogue back out into the world (not directly into grim inventory)
	 *
	 */

	void GrowRogue () {
		// already set to be harvested - skip growing
		if (isHarvestable) {
			Debug.Log ("Rogue is now harvestable");
			return;
		}

		// check if already done growing - enable harvestability
		if (growthStageCurrent >= growthStageMax || !rogue.isAlive) {
			isHarvestable = true;

			// show a harvestable mark on the plot headstone
			stoneRenderer.Mark ();

			// play sfx marking epitaph
			audio.clip = sfxEpitaph;
			audio.Play ();

			Debug.Log ("Setting nonalive/finished rogue to be harvestable!");
			return;
		}

		// play sfx
		audio.clip = sfxGrow;
		audio.Play ();

		// advance the growth stage
		growthStageCurrent++;

		// display the current growth sprite
		growthRenderer.SetGrowthSprite (growthStageCurrent);

		// fetch current castle level info then advance castle level
		List<CastleObstacle> obstacles = castle.RunLevel();

		// send rogue obstacles to deal with
		Debug.Log ("Growing the planted rogue - alive and ready for another stage!");
		foreach (CastleObstacle obstacle in obstacles) {
			if (rogue.isAlive) {
				rogue.FeedObstacle (obstacle);
			}
		}

		// TODO: finish growth and set to harvest, including rogue having finished castle or died in castle
		//
		if (!rogue.isAlive) {
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
	// unassign and return unparented, reactivated rogue
	GameObject UnsetRogue () {
		// reactivate rogue in world
		rogue.transform.SetParent (null);
		rogue.gameObject.SetActive (true);

		// save the rogue to return
		GameObject oldRogue = rogue.gameObject;

		// unattach rogue from this plot
		rogue = null;

		// return the rogue
		return oldRogue;
	}

	// place rogue in plot and begin growing each day
	public bool PlantRogue(GameObject newRogue) {
		if (rogue == null && !GameObject.Equals(newRogue.GetComponent<Rogue> (), null)) {
			// attach incoming rogue to this plot
			SetRogue (newRogue);

			// play sfx
			audio.clip = sfxPlant;
			audio.Play ();

			// display the plot headstone
			stoneRenderer.Show ();

			// display the planted root
			growthRenderer.SetGrowthSprite (0);

			// set up day callback to start cycling growth
			plantedHour = DayManager.Day.EveryDay (growthAction);

			// make sure rogue is alive for obstacle progression
			rogue.Revive();

			// initialize obstacles ("castle") to run rogue through
			castle.ResetCastle ();

			isEmpty = false;
			Debug.Log ("Planted rogue in Plot");

			return true;
		}
		return false;
	}

	// pop rogue from plot, reset growth and get rogue runthrough skills text
	public GameObject HarvestRogue () {
		// planted rogue ready for harvest
		if (isHarvestable) {

			Debug.Log("Trying to harvest rogue " + rogue.name + " with health " + rogue.maxHealth);

			// play sfx
			audio.clip = sfxHarvest;
			audio.Play ();

			// stop displaying the plot headstone
			stoneRenderer.Hide ();

			// reset planted rogue and castle scheduling
			growthStageCurrent = 0;
			DayManager.Day.NotAt (plantedHour, growthAction);
			plantedHour = -1;

			// reset plot harvestability
			isHarvestable = false;
			isEmpty = true;

			// reset the growth stage sprite
			growthRenderer.SetGrowthSprite (-1);

			// grab reactivated rogue item
			GameObject rogueObject = UnsetRogue ();

			// have rogue spend stat gains from run performance
			rogueObject.GetComponent<Rogue> ().Upgrade();

			// return harvested rogue
			return rogueObject;
		}

		// no rogue to return
		return null;
	}


	/* Show and hide headstone on plot and sidescreen epitaph with rogue run stats  */

	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "Player") {
			// switch off plot soil highlighting
			dirtRenderer.Deselect();

			// hide epitaph ui
			epitaph.GetComponent<Epitaph> ().Hide ();
		}
	}

	void OnTriggerStay(Collider other) {
		if (other.gameObject.tag == "Player") {

			// player over a plot - highlight the plot soil
			dirtRenderer.Select();

			// player over planted plot - show rogue info
			if (!isEmpty) {
				epitaph.GetComponent<Epitaph> ().Show ();

				// fill and format title
				string epitaphText = "<size=30><b>" + rogue.name + "</b></size>";
				epitaphText += "\n<color=#666666ff>______________</color>\n";

				// fill and format text body
				epitaphText += "<size=25>";

				// text for a rogue after finished with a run

				// rogue actually defeated the entire castle
				if (isHarvestable && rogue.isAlive) {
					epitaphText += "was victorious\n";
					epitaphText += "in the castle of\n";
					epitaphText += "<i>" + castle.castleName + "</i>!";
				}
				// rogue died
				else if (isHarvestable) {
					epitaphText += "perished\n";
					epitaphText += "<size=15>on <b>floor " + GrowthStage + "</b> ";
					epitaphText += "at the hands of a</size>\n";
					epitaphText += "<size=23><i>" + rogue.deathDealerName.ToUpper () + "</i></size>\n";
					epitaphText += "<size=18>(some kind of <i>" + rogue.deathDealerType.ToUpper () + "</i>)</size>";
				}
				// text for a rogue currently mid-run
				else {
					epitaphText += GrowthStage > 0
						? "<size=22>bravely facing <b>stage " + GrowthStage + "</b> within the <i>" + castle.castleName + "</i></size>"
						: "<i>setting out on an adventure</i>";
				}

				epitaphText += "</size>";

				// update ui with text
				epitaph.GetComponentInChildren<Text> ().text = epitaphText;
			}
			// player over plot which is now empty - hide the epitaph
			else {
				epitaph.GetComponent<Epitaph> ().Hide ();
			}
		}
	}

}
