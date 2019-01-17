using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour {

	// sequence of hazards and rewards to be met each level
	public List<List <string>> levelObstacles = new List<List<string>> ();

	// TODO use the int as stats for strength and toughness, or for chests as theft
	// - rogue will do one attempt, take a hit to a stat, then do another attempt
	// - hits to armor reduce damage to health, but health still always hit
	// - attempts hit from weapon stat or from thievery stat
	// - successful treasures add to gold stash to be brought back

	// obstacles dictionaries for populating the castle sequence
	// TODO also calculate commonness - some show up more/less on low/high levels
	public Dictionary<string, int> enemies = new Dictionary<string, int> () {
		// grunts
		{ "tiny", 1 },
		{ "minion", 2 },
		{ "brute", 3 },
		{ "guard", 5 },
		{ "warrior", 8 },
		// minibosses
		{ "bull miniboss", 8 },
		{ "snake miniboss", 10 },
		{ "knight miniboss", 14 },
		{ "shapeshifter miniboss", 17 },
		{ "legendary miniboss", 20 },
		// bosses
		{"boss Snailzasaur", 25},
		{"boss Feaglecon", 35},
		{"boss Dragon", 50},
		// final bosses
		{"Shinosaur the Strong", 150},
		{"Saggawar the Cunning", 180},
		{"Fellafunt the Wise", 200},
		{"Sprabbit the Swift", 150}
	};

	public Dictionary<string, int> treasures = new Dictionary<string, int> () {
		{ "pile", 1 },
		{ "stash", 3 },
		{ "chest", 5 },
		{ "warchest", 10 },
		{ "vault", 25 }
	};

	public Dictionary<string, int> hazards = new Dictionary<string, int> () {
		{ "small pit", 1 },			// small but maybe deliver pain in bulk
		{ "spike", 3 },
		{ "big bottomless pit", 5 },
		{ "lava", 8 },
		{ "ice", 10 },
		{ "poison trap", 14 },
		{ "death trap", 25 }	 	// rare but excruciating
	};

	// iterate through string list swapping values randomly
	// modified from: https://stackoverflow.com/questions/273313/randomize-a-listt
	List<string> shuffleList(List<string> l) {
		for (int i = l.Count-1; i > 1; i--) {
			int random_i = Random.Range (0, i + 1);
			string value = l[random_i];
			l [random_i] = l [i];
			l [i] = value;
		}
		return l;
	}

	// add string to list a bounded random number of times
	void addRandomTimes (List<string> l, string s, int minTimes, int maxTimes) {
		int times = Random.Range(minTimes, maxTimes + 1);
		for (int i=0; i < times; i++) {
			l.Add (s);
		}
	}

	// log the sequence of obstacles
	void printList (List<string> l) {
		Debug.Log (l.Count);
		string o = "[";
		for (int i = 0; i < l.Count; i++) {
			o = string.Concat (o, l[i], ",");
		}
		Debug.Log (string.Concat(o, "]"));
	}
	 
	// build new list of levels listing castle obstacles encountered this playthrough
	// these are sequential to simulate action order not to imply a linear level
	public void resetCastle() {
		
		// TODO town/merchants level?

		/*
		 * Level 1
		 * easy difficulty floor
		 */
		List<string> level1 = new List<string> ();

		// input everything that belongs in the level before level boss
		// enemies
		this.addRandomTimes (level1, "tiny", 4, 6);
		this.addRandomTimes (level1, "minion", 2, 3);
		this.addRandomTimes (level1, "brute", 0, 2);
		this.addRandomTimes (level1, "guard", 0, 0);
		this.addRandomTimes (level1, "warrior", 0, 0);
		// treasures
		this.addRandomTimes (level1, "pile", 4, 6);
		this.addRandomTimes (level1, "stash", 2, 3);
		this.addRandomTimes (level1, "chest", 1, 2);
		// hazards
		this.addRandomTimes (level1, "small pit", 4, 6);
		this.addRandomTimes (level1, "spike", 2, 3);
		this.addRandomTimes (level1, "big bottomless pit", 1, 2);

		// miniboss
		this.addRandomTimes (level1, "bull miniboss", 0, 1);
		if (!level1.Contains ("bull miniboss")) {
			level1.Add ("snake miniboss");
		}

		// shuffle the level
		this.shuffleList (level1);

		// input level boss (chance not to have one?)
		level1.Add ("boss Snailzasaur");

		/*
		 * Level 2
		 * medium difficulty floor
		 */
		List<string> level2 = new List<string> ();

		// enemies
		this.addRandomTimes (level2, "tiny", 4, 6);
		this.addRandomTimes (level2, "minion", 4, 6);
		this.addRandomTimes (level2, "brute", 4, 6);
		this.addRandomTimes (level2, "guard", 0, 1);
		this.addRandomTimes (level2, "warrior", 0, 0);
		// treasures
		this.addRandomTimes (level2, "pile", 1, 3);
		this.addRandomTimes (level2, "stash", 2, 3);
		this.addRandomTimes (level2, "chest", 2, 3);
		this.addRandomTimes (level2, "warchest", 0, 1);
		this.addRandomTimes (level2, "vault", 0, 0);
		// hazards
		this.addRandomTimes (level2, "small pit", 0, 2);
		this.addRandomTimes (level2, "spike", 1, 3);
		this.addRandomTimes (level2, "big bottomless pit", 2, 5);
		this.addRandomTimes (level2, "lava", 1, 2);
		this.addRandomTimes (level2, "ice", 1, 2);
		this.addRandomTimes (level2, "poison trap", 0, 2);
		this.addRandomTimes (level2, "death trap", 0, 0);
		// miniboss
		this.addRandomTimes (level2, "knight miniboss", 0, 1);
		if (!level2.Contains ("knight miniboss")) {
			level2.Add ("shapeshifter miniboss");
		}

		// shuffle the non-boss
		this.shuffleList (level2);

		// boss
		level2.Add ("boss Feaglecon");

		/*
		 * Level 3
		 * hard difficulty floor
		 */
		List<string> level3 = new List<string> ();

		// enemies
		this.addRandomTimes (level3, "tiny", 0, 0);
		this.addRandomTimes (level3, "minion", 2, 3);
		this.addRandomTimes (level3, "brute", 3, 5);
		this.addRandomTimes (level3, "guard", 5, 8);
		this.addRandomTimes (level3, "warrior", 3, 5);
		// treasures
		this.addRandomTimes (level3, "pile", 0, 0);
		this.addRandomTimes (level3, "stash", 0, 3);
		this.addRandomTimes (level3, "chest", 1, 3);
		this.addRandomTimes (level3, "warchest", 1, 2);
		this.addRandomTimes (level3, "vault", 0, 1);
		// hazards
		this.addRandomTimes (level3, "small pit", 0, 0);
		this.addRandomTimes (level3, "spike", 0, 3);
		this.addRandomTimes (level3, "big bottomless pit", 1, 5);
		this.addRandomTimes (level3, "lava", 2, 5);
		this.addRandomTimes (level3, "ice", 3, 5);
		this.addRandomTimes (level3, "poison trap", 1, 3);
		this.addRandomTimes (level3, "death trap", 0, 1);
		// miniboss
		this.addRandomTimes (level3, "legendary miniboss", 0, 1);
		if (!level3.Contains ("legendary miniboss")) {
			level3.Add ("shapeshifter miniboss");
		}

		// shuffle the non-boss
		this.shuffleList (level3);

		// boss
		level3.Add ("boss Dragon");

		/*
		 * Level 4
		 * final boss floor
		 */
		List<string> level4 = new List<string> ();

		// add just one of the final bosses
		this.addRandomTimes (level4, "Fellafunt the Wise", 0, 1);
		if (!level4.Contains ("Fellafunt the Wise")) {
			this.addRandomTimes (level4, "Saggawar the Cunning", 0, 1);
			if (!level4.Contains ("Saggawar the Cunning")) {
				level4.Add ("Shinosaur the Strong");
			}
		}

		/* New Level List */
		// remove past levels and add new ones
		levelObstacles.Clear ();
		levelObstacles.Add (level1);
		levelObstacles.Add (level2);
		levelObstacles.Add (level3);
		levelObstacles.Add (level4);

		this.printList (level1);
		this.printList (level2);
		this.printList (level3);
		this.printList (level4);

		return;
	}
}
