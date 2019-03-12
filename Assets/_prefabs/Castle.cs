using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour {

	// sequence of hazards and rewards to be met each level
	public List<List <CastleObstacle>> levelObstacles = new List<List<CastleObstacle>> ();

	// obstacle prefab to instantiate and fill with level obstacle (enemy/treasure/hazard/boss) data
	public GameObject castleObstacle;

	// progress pointer to position within level obstacles
	int currentLevel = 0;
	int currentObstacle = 0;

	// number of levels to build during castle setup
	public int levelCount = 3;

	// TODO: calc commonness to populate castle sequence - make some appear less/more on low/high levels
	// 	- see ResetCastle function for 

	// Types of obstacles to populate, including their types, names and interaction values
	// { 'obstacleType', {{ 'obstacleName', n }, ... }, ... }
	//
	// TODO: accept and parse json input data instead of hardcoding obstacles here
	//
	Dictionary<string, Dictionary<string, int>> obstacles = new Dictionary<string, Dictionary<string, int>> () {
		// grunts within a level - value doubles as both energy and strength
		{ "enemy", new Dictionary<string, int> {
			{ "tiny", 1 },
			{ "minion", 2 },
			{ "brute", 3 },
			{ "guard", 5 },
			{ "warrior", 8 }
		}},
		// treasures to open - value is amount
		{ "treasure", new Dictionary<string, int> {
			{ "pile", 1 },
			{ "stash", 3 },
			{ "chest", 5 },
			{ "warchest", 10 },
			{ "vault", 25 }
		}},
		// hazards to avoid - value is luck needed to thwart
		{ "hazard", new Dictionary<string, int> {
			{ "small pit", 1 },			// small but maybe deliver pain in bulk
			{ "spike", 3 },
			{ "big bottomless pit", 5 },
			{ "lava", 8 },
			{ "ice", 10 },
			{ "poison trap", 14 },
			{ "death trap", 25 }	 	// rare but excruciating
		}},
		// minibosses within a level - value is energy and strength
		{ "miniBoss", new Dictionary<string, int> {
			{ "bull miniboss", 8 },
			{ "snake miniboss", 10 },
			{ "knight miniboss", 14 },
			{ "shapeshifter miniboss", 17 },
			{ "legendary miniboss", 20 }
		}},
		// bosses for a single level - value is both energy and strength
		{ "boss", new Dictionary<string, int> {
			{ "boss Snailzasaur", 25 },
			{ "boss Feaglecon", 35 },
			{ "boss Dragon", 50 }
		}},
		// final bosses for an entire run - value is energy and strength
		{ "finalBoss", new Dictionary<string, int> {
			{"Shinosaur the Strong", 150 },
			{"Saggawar the Cunning", 180 },
			{"Fellafunt the Wise", 200 },
			{"Sprabbit the Swift", 150 }
		}},
	};

	// log entire sequence of obstacles generated for current castle run
	void LogCastle () {
		// title for output string to be logged
		string o = " -- Castle " + this.name + " Obstacles -- \n";

		// traverse all obstacles in all levels
		for (int level=0; level < levelObstacles.Count; level++) {
			// start the level list
			o = string.Concat (o, "Level ", level, " obstacles: [");

			// list out every obstacle in the level
			foreach (CastleObstacle obstacle in levelObstacles[level]) {
				o = string.Concat (o, obstacle.obstacleName, ", ");
			}

			// ditch the last comma-space
			o = o.Remove (o.Length - 2);
			// add bracket for end of level list
			o = string.Concat (o, "]\n");
		}

		// log output
		Debug.Log (o);
	}


	/* Traverse through level obstacles - for advancing rogue through castle */

	// advance current position one level and return sequence of traversed level obstacle keys
	public List<CastleObstacle> RunLevel() {
		// catch level index beyond existing levels
		if (currentLevel >= levelObstacles.Count) {
			Debug.Log ("Cannot advance levels beyond " + currentLevel + " in Castle " + this.name + ". All levels complete!");
			return new List<CastleObstacle> ();
		}

		// store the current level obstacle objects and advance the level pointer
		List<CastleObstacle> currentLevelObstacles = levelObstacles[currentLevel];
		currentLevel++;

		return currentLevelObstacles;
	}

	// advance current position one obstacle and return traversed obstacle key
	public CastleObstacle RunObstacle(bool advanceLevel=false) {
		// catch obstacle index beyond current level obstacles
		if (currentObstacle >= levelObstacles [currentLevel].Count) {
			Debug.Log ("Reached the end of the obstacles list for level " + currentLevel);
			// optionally update the level index
			if (advanceLevel) {
				currentLevel++;
			}
			return null;
		}

		// store the current obstacle reference and advance the obstacle pointer
		CastleObstacle currentObstacleBehavior = levelObstacles[currentLevel][currentObstacle];
		currentObstacle++;

		return currentObstacleBehavior;
	}


	// TODO: abstract a simpler level builder and get rid of the repetitive adds below
	// 	- see notes about calculating per-level commonness and storing in obstacles data
	List<CastleObstacle> BuildLevel () { return new List<CastleObstacle> (); }


	// iterate through one level's obstacle behaviors and randomly swap obstacles
	// modified from: https://stackoverflow.com/questions/273313/randomize-a-listt
	List<CastleObstacle> ShuffleList(List<CastleObstacle> l) {
		for (int i = l.Count-1; i > 1; i--) {
			int random_i = Random.Range (0, i + 1);
			CastleObstacle value = l[random_i];
			l [random_i] = l [i];
			l [i] = value;
		}
		return l;
	}

	// add castle obstacle to level list a bounded random number of times
	void AddRandomTimes (List<CastleObstacle> levelList, string obstacleType, string obstacleName, int minTimes, int maxTimes) {
		// check if entry exists for obstacle matching the given type and name
		if (!obstacles.ContainsKey(obstacleType) || !obstacles[obstacleType].ContainsKey(obstacleName)) {
			return;
		}

		// locate the obstacle data and store relevant info
		int obstacleStat = obstacles[obstacleType][obstacleName];

		// add obstacle a bounded random number of times
		int times = Random.Range(minTimes, maxTimes + 1);

		// back out if didn't roll a positive integer
		if (times < 1) return;

		// spawn and add obstacle as many times as rng rolled
		for (int i=0; i < times; i++) {
			// spawn the obstacle within this castle
			CastleObstacle obstacle = Instantiate (castleObstacle, this.transform).GetComponent<CastleObstacle> ();

			// pass along the basic obstacle attributes
			obstacle.obstacleName = obstacleName;
			obstacle.obstacleType = obstacleType;
			obstacle.obstacleValue = obstacleStat;

			// append the new obstacle to the level
			levelList.Add (obstacle);
		}
	}

	// build new list of levels listing castle obstacles encountered this playthrough
	// these are sequential to simulate action order not to imply a linear level
	public void ResetCastle() {
		
		// TODO: town/merchants level between or before level forays

		// reset current progress
		currentLevel = 0;
		currentObstacle = 0;

		/*
		 * Level 1
		 * easy difficulty floor
		 */
		List<CastleObstacle> level1 = new List<CastleObstacle> ();

		// input everything that belongs in the level before level boss
		// enemies
		this.AddRandomTimes (level1, "enemy", "tiny", 4, 6);
		this.AddRandomTimes (level1, "enemy", "minion", 2, 3);
		this.AddRandomTimes (level1, "enemy", "brute", 0, 2);
		this.AddRandomTimes (level1, "enemy", "guard", 0, 0);
		this.AddRandomTimes (level1, "enemy", "warrior", 0, 0);
		// treasures
		this.AddRandomTimes (level1, "treasure", "pile", 4, 6);
		this.AddRandomTimes (level1, "treasure", "stash", 2, 3);
		this.AddRandomTimes (level1, "treasure", "chest", 1, 2);
		// hazards
		this.AddRandomTimes (level1, "hazard", "small pit", 4, 6);
		this.AddRandomTimes (level1, "hazard", "spike", 2, 3);
		this.AddRandomTimes (level1, "hazard", "big bottomless pit", 1, 2);

		// add one of two minibosses
		List<string> miniBosses = new List<string>() { "bull miniboss",  "snake miniboss" };
		string bossName = miniBosses [Random.Range (0, miniBosses.Count)];
		AddRandomTimes (level1, "miniBoss", bossName, 1, 1);

		// shuffle the level
		this.ShuffleList (level1);

		// input level boss (chance not to have one?)
		AddRandomTimes (level1, "boss", "boss Snailzasaur", 1, 1);

		/*
		 * Level 2
		 * medium difficulty floor
		 */
		List<CastleObstacle> level2 = new List<CastleObstacle> ();

		// enemies
		this.AddRandomTimes (level2, "enemy", "tiny", 4, 6);
		this.AddRandomTimes (level2, "enemy", "minion", 4, 6);
		this.AddRandomTimes (level2, "enemy", "brute", 4, 6);
		this.AddRandomTimes (level2, "enemy", "guard", 0, 1);
		this.AddRandomTimes (level2, "enemy", "warrior", 0, 0);
		// treasures
		this.AddRandomTimes (level2, "treasure", "pile", 1, 3);
		this.AddRandomTimes (level2, "treasure", "stash", 2, 3);
		this.AddRandomTimes (level2, "treasure", "chest", 2, 3);
		this.AddRandomTimes (level2, "treasure", "warchest", 0, 1);
		this.AddRandomTimes (level2, "treasure", "vault", 0, 0);
		// hazards
		this.AddRandomTimes (level2, "hazard", "small pit", 0, 2);
		this.AddRandomTimes (level2, "hazard", "spike", 1, 3);
		this.AddRandomTimes (level2, "hazard", "big bottomless pit", 2, 5);
		this.AddRandomTimes (level2, "hazard", "lava", 1, 2);
		this.AddRandomTimes (level2, "hazard", "ice", 1, 2);
		this.AddRandomTimes (level2, "hazard", "poison trap", 0, 2);
		this.AddRandomTimes (level2, "hazard", "death trap", 0, 0);
		// miniboss
		miniBosses = new List<string>() { "shapeshifter miniboss",  "knight miniboss" };
		bossName = miniBosses [Random.Range (0, miniBosses.Count)];
		AddRandomTimes (level2, "miniBoss", bossName, 1, 1);

		// shuffle the non-boss
		this.ShuffleList (level2);

		// boss
		AddRandomTimes (level2, "boss", "boss Feaglecon", 1, 1);

		/*
		 * Level 3
		 * hard difficulty floor
		 */
		List<CastleObstacle> level3 = new List<CastleObstacle> ();

		// enemies
		this.AddRandomTimes (level3, "enemy", "tiny", 0, 0);
		this.AddRandomTimes (level3, "enemy", "minion", 2, 3);
		this.AddRandomTimes (level3, "enemy", "brute", 3, 5);
		this.AddRandomTimes (level3, "enemy", "guard", 5, 8);
		this.AddRandomTimes (level3, "enemy", "warrior", 3, 5);
		// treasures
		this.AddRandomTimes (level3, "treasure", "pile", 0, 0);
		this.AddRandomTimes (level3, "treasure", "stash", 0, 3);
		this.AddRandomTimes (level3, "treasure", "chest", 1, 3);
		this.AddRandomTimes (level3, "treasure", "warchest", 1, 2);
		this.AddRandomTimes (level3, "treasure", "vault", 0, 1);
		// hazards
		this.AddRandomTimes (level3, "hazard", "small pit", 0, 0);
		this.AddRandomTimes (level3, "hazard", "spike", 0, 3);
		this.AddRandomTimes (level3, "hazard", "big bottomless pit", 1, 5);
		this.AddRandomTimes (level3, "hazard", "lava", 2, 5);
		this.AddRandomTimes (level3, "hazard", "ice", 3, 5);
		this.AddRandomTimes (level3, "hazard", "poison trap", 1, 3);
		this.AddRandomTimes (level3, "hazard", "death trap", 0, 1);
		// miniboss
		miniBosses = new List<string>() { "legendary miniboss",  "shapeshifter miniboss" };
		bossName = miniBosses [Random.Range (0, miniBosses.Count)];
		AddRandomTimes (level3, "miniBoss", bossName, 1, 1);

		// shuffle the non-boss
		this.ShuffleList (level3);

		// boss
		AddRandomTimes (level3, "boss", "boss Dragon", 1, 1);

		/*
		 * Level 4
		 * final boss floor
		 */
		List<CastleObstacle> level4 = new List<CastleObstacle> ();

		// add just one of the final bosses
		List<string> finalBosses = new List<string>(obstacles["finalBoss"].Keys);
		bossName = finalBosses [Random.Range (0, finalBosses.Count)];
		AddRandomTimes (level4, "finalBoss", bossName, 1, 1);

		/* New Level List */
		// remove past levels and add new ones
		levelObstacles.Clear ();
		levelObstacles.Add (level1);
		levelObstacles.Add (level2);
		levelObstacles.Add (level3);
		levelObstacles.Add (level4);

		// Log the obstacles in each level
		this.LogCastle ();

		return;
	}
}
