using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmManager : MonoBehaviour {

	// prefab to instantiate and arrange in plots matrix
	public GameObject plot;

	// spatial transforms
	public Vector3 startingPosition;
	public int numColumns = 5;
	public int numRows = 3;
	public float columnSeparation = 1f;
	public float rowSeparation = 1f;
	private float currentX;		// parallel to view
	private float currentZ; 	// depth from view

	// plots in rows
	private List<List<GameObject>> plotsMatrix = new List<List<GameObject>> ();
	// sprite for background behind plots
	private GameObject background;

	void Start () {
		// create and store plots in rows and columns
		currentX = startingPosition.x;
		currentZ = startingPosition.z;
		for (int i = 0; i < numRows; i++) {
			plotsMatrix.Add (new List<GameObject> ());
			for (int j = 0; j < numColumns; j++) {
				GameObject farmPlot = GameObject.Instantiate (plot, new Vector3(currentX, 0f, currentZ), plot.transform.rotation);
				farmPlot.name = string.Format ("{0}.{1}.{2}", farmPlot.name, i, j);
				plotsMatrix [i].Add (farmPlot);
				currentX += columnSeparation;
			}
			currentX = startingPosition.x;
			currentZ += rowSeparation;
		}
	}

}
