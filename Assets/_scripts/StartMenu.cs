using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour {

	public string mainSceneName;
	public AudioClip sfxHover;
	public AudioClip sfxClick;
	AudioSource audio;

	void Awake () {
		audio = GetComponent<AudioSource> ();
	}

	void Hover() {
		audio.clip = sfxHover;
		audio.Play ();
	}

	void StartGame() {
		audio.clip = sfxClick;
		audio.Play ();
		StartCoroutine ("WaitThenLoad");
	}

	IEnumerator WaitThenLoad() {
		yield return new WaitForSeconds (0.6f);
		UnityEngine.SceneManagement.SceneManager.LoadScene (mainSceneName);
	}
}
