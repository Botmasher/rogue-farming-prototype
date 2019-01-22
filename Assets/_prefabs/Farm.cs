using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : MonoBehaviour {

	// prefab to instantiate and arrange in plots matrix
	public GameObject plot;

	// spatial transforms
	public Vector2 startingXY;
	public int numColumns = 5;
	public int numRows = 3;
	public float columnSeparation = 1f;
	public float rowSeparation = 1f;
	private float currentX;
	private float currentY;

	// plots in rows
	private List<List<GameObject>> plotsMatrix = new List<List<GameObject>> ();
	// sprite for background behind plots
	private GameObject background;

	void Start () {
		// create and store plots in rows and columns
		currentX = startingXY.x;
		currentY = startingXY.y;
		for (int i = 0; i < numRows; i++) {
			plotsMatrix.Add (new List<GameObject> ());
			for (int j = 0; j < numColumns; j++) {
				plotsMatrix [i].Add (GameObject.Instantiate (plot, new Vector2(currentX, currentY), Quaternion.Euler(0f, 0f, 0f)));
				currentX += columnSeparation;
			}
			currentX = startingXY.x;
			currentY += rowSeparation;
		}
	}

}
