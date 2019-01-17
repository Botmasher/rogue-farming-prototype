using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : MonoBehaviour {
	
	// time tracking
	int days = 0; 				// number of days passed since game start
	int hour = 0; 				// number of hours passed this day
	int framesCounter = 0; 		// keep track of frames to divide into hours and days

	// time constants
	// TODO pin more to deltaTime instead of tracking only frames
	int framesPerHour = 200; 	// divide frames counter into hours (3600 frames is 1 min at 60 fps)
	int hoursPerDay = 10; 		// divide day into 0-n (n exclusive) hours range
	int morningHour = 1; 		// strikes on the first hour (one hour after day reset)
	int noonHour = 4;
	int eveningHour = 6;
	int nightHour = 7;

	// listener events to subscribe/announce and callbacks to run
	Dictionary<string, List<Action>> events = new Dictionary<string, List<Action>> () {
		{ "day", new List<Action> () },
		{ "morning", new List<Action> () },
		{ "noon", new List<Action> () },
		{ "evening", new List<Action> () },
		{ "night", new List<Action> () }
	};

	void Start () {
		// test subscribing for day events
		this.At("day", () => Debug.Log("it's a brand new day!"));
		this.At("morning", () => Debug.Log("it's morning!"));
		this.At("noon", () => Debug.Log("it's noon!"));
		this.At("evening", () => Debug.Log("it's evening!"));
		this.At("night", () => Debug.Log("it's night!"));
	}

	void Update() {
		this.framesCounter++;
		// an hour has passed
		if (this.framesCounter >= this.framesPerHour) {
			this.hour++;
			this.framesCounter = 0;
			// a full day has passed
			if (this.hour >= this.hoursPerDay) {
				this.days++;
				this.hour = 0;
				this.Announce("day");
			// clock struck an event hour
			} else if (this.hour == morningHour) {
				this.Announce("morning");
			} else if (this.hour == noonHour) {
				this.Announce("noon");
			} else if (this.hour == eveningHour) {
				this.Announce("evening");
			} else if (this.hour == nightHour) {
				this.Announce("night");
			// no events
			} else {
				Debug.Log("Another regular hour passed. Nothing to announce.");
			}
		}
	}

	// event notification
	private void Announce(string eventName) {
		// run through 
		this.events[eventName].ForEach(cb => cb());
	}

	// unsubscribe from event
	public void NotAt(string eventName, Action callback) {
		if (this.events.ContainsKey(eventName) && this.events[eventName].Contains(callback)) {
			this.events[eventName].Remove(callback);
		} else {
			Debug.Log (string.Format("Failed to unsubscribe from Day event - unknown event {0} or callback {1}", eventName, callback));
		}
	}

	// listener subscription
	public void At(string eventName, Action callback) {
		if (this.events.ContainsKey(eventName)) {
			this.events[eventName].Add (callback);
		} else {
			Debug.Log (string.Format("Failed to subscribe to Day event - invalid event \"{0}\"", eventName));
		}
	}

}
