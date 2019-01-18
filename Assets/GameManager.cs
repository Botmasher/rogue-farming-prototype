using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	// prefabs to instantiate - at least for demo
	public GameObject plot;
	public Rogue rogue; 	// placed in plot, fights in castle
	public Castle castle; 	// new one generated for each rogue planting in plot

	// prefabs to instantiate - ongoing
	GameObject farm;

	void Start () {
		// demo single plot - eventually have grim place selected rogues in plots
		GameObject newPlot = GameObject.Instantiate (plot) as GameObject;
		newPlot.GetComponent<Plot> ().rogue = GameObject.Instantiate (rogue) as Rogue;
		newPlot.GetComponent<Plot> ().castle = GameObject.Instantiate (castle) as Castle;

		// full startup
		Instantiate (farm, Vector3.zero, Quaternion.identity); 	// plots matrix
	}
}
