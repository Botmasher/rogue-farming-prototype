using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : MonoBehaviour {

	// day-passing singleton hookable throughout game
	public static DayManager Day;

	// lighting object to control based on timing
	public GameObject light;

	// time tracking
	int days; 				// number of days passed since game start
	int hour; 				// number of hours passed this day
	float secondsCounter; 	// keep track of real seconds to divide into game hours and days

	// time passage factor
	[Range(0f,10f)]public float speed = 1f; 	// slow or speed up time

	// light rotation around axis
	Quaternion lightRotationTarget;
	float degreesPerHour; 			// number of sunlight rotation degrees for each in-game hour

	// time constants
	// TODO pin more to deltaTime instead of tracking only frames
	float secondsPerHour = 4f; 		// how long a game hour lasts in realtime
	int hoursPerDay = 12; 			// divide day into 0-n (n exclusive) hours range

	// interface for displaying time info
	public GameObject dayUI;

	// listener events to subscribe/announce and callbacks to run
	Dictionary<int, List<Action>> events = new Dictionary<int, List<Action>> ();

	void Awake () {
		// static singleton for global public access
		if (Day != null) {
			GameObject.Destroy (Day);
		} else {
			Day = this;
		}
		DontDestroyOnLoad (this);
	}

	void Start () {
		// start with full counters to trigger new day actions on first day
		secondsCounter = secondsPerHour;
		days = -1;
		hour = hoursPerDay;

		// slice up daylight rotation
		degreesPerHour = 360f / hoursPerDay;

		// initialize all hours with empty actions lists for callbacks
		// times run from 0:00 to n-1:99 so hours per day limit is never struck
		for (int i = 0; i < hoursPerDay; i++) {
			events [i] = new List<Action> ();
		}

		// test subscribing for day events
		//this.At(0, () => Debug.Log("It's a brand new day! Day " + days + " in fact."));
		this.EveryHour(() => Debug.Log("The time is hour " + hour));

	}

	void Update() {
		// in-game time per frame
		secondsCounter += Time.deltaTime * speed;

		// an hour has passed
		if (secondsCounter >= secondsPerHour) {
			PassHour ();

			// set the new angle for sunlight
			lightRotationTarget = Quaternion.Euler(Vector3.left * degreesPerHour * hour);
		}

		// rotate light towards new hourly angle
		if (light.transform.rotation != lightRotationTarget) {
			light.transform.rotation = Quaternion.Lerp(light.transform.rotation, lightRotationTarget, Time.deltaTime * speed);
		}
	}

	// count up one hour, run hourly events, cycle day if necessary
	void PassHour() {
		// pass one hour and reset seconds
		hour++;
		secondsCounter = 0f;

		// pass a full day
		if (hour >= hoursPerDay) {
			days++;
			hour = 0;
		}

		// run hourly callbacks
		Announce (hour);

		// update the daytimer UI
		dayUI.GetComponentInChildren<UnityEngine.UI.Text> ().text = "Day " + (days + 1) + "  hour " + (hour + 1);
	}

	// event notification
	private void Announce(int eventHour) {
		// run through all actions at this time
		events[eventHour].ForEach(cb => cb());
	}

	// unsubscribe from event
	public void NotAt(int eventHour, Action callback) {
		if (events.ContainsKey(eventHour) && events[eventHour].Contains(callback)) {
			events[eventHour].Remove(callback);
		} else {
			Debug.Log (string.Format("Failed to unsubscribe from Day event - unknown event {0} or callback {1}", eventHour, callback));
		}
	}

	// listener subscription
	public void At(int eventHour, Action callback) {
		if (events.ContainsKey(eventHour)) {
			events[eventHour].Add (callback);
		} else {
			Debug.Log (string.Format("Failed to subscribe to Day event - invalid event \"{0}\"", eventHour));
		}
	}

	// subscribe for actions run every hour
	public void EveryHour(Action callback) {
		foreach (KeyValuePair<int, List<Action>> hourlyActions in this.events) {
			hourlyActions.Value.Add (callback);
		}
	}

	// count relative days from the current time
	// NOTE: subscribe to 0 hour for fixed new day
	public int EveryDay(Action callback) {
		At(hour, callback);
		return hour;
	}
}
