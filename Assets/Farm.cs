using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : MonoBehaviour {

	public GameObject plot; 	// prefab to instantiate and arrange in plots matrix

	// spatial transforms
	public Vector2 startingXY;
	public int numColumns;
	public int numRows;
	public float columnSeparationX;
	public float rowSeparationY;
	private float currentX;
	private float currentY;

	private List<List<GameObject>> plotsMatrix;	// plots in rows
	private GameObject background; 				// sprite for background behind plots

	void Start () {
		for (int i = 0; i < numRows; i++) {
			plotsMatrix.Add (new List<GameObject> ());
			for (int j = 0; i < numColumns; j++) {
				plotsMatrix [i].Add (Instantiate (plot, new Vector2(currentX, currentY)));
				currentX += columnSeparationX;
			}
			currentY += rowSeparationY;
		}
	}
}
