using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour {

	// TODO make a listener class for queuing up callbacks on call, then send this one for "onLevelProgress" rogueId, castleId

	public Sprite spriteEmpty;
	public Sprite spritePlanted;
	public Sprite spriteGrowing;
	public Sprite spriteDone;
	public Sprite spriteCracked;

	// prefab for castle setup and reset
	public GameObject castle;
	private Castle castleSettings;

	List<Sprite> sprites = new List<Sprite>();

	void Start () {
		// add all sprites to the list
		sprites.Add (spriteEmpty);
		sprites.Add (spritePlanted);
		sprites.Add (spriteGrowing);
		sprites.Add (spriteDone);
		sprites.Add (spriteCracked);
		// set current sprite
		GetComponent<SpriteRenderer> ().sprite = sprites[0];

		castleSettings = castle.GetComponent<Castle> ();

		castleSettings.resetCastle ();
	}

	void Update() {
		
	}

	public void SetCaslte() {
		// TODO decide attributes for castle and create new one
		//castleSettings.runSomething()
		return;
	}
}
