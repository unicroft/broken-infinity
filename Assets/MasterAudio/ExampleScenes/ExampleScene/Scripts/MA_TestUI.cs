using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MA_TestUI : MonoBehaviour {
	public Transform[] spheres;
	
	void OnGUI() {
		GUI.Label(new Rect(20,40,450,120), "Use left/right arrow keys and left mouse button to play. " +
			"Music ducks (gets quieter) for explosions, then ramps back up soon after. Sound FX have " +
			"variations. No code needed to be written for any of the sound triggering or ducking. See ReadMe.pdf for more information on how to set things up." +
			"There are new playlist controls that support multiple playlists!" + 
			"\n\nHappy gaming - DarkTonic, Inc.");
	}
}
