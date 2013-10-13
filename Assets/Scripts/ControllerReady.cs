using UnityEngine;
using XboxCtrlrInput;
using System.Collections;
using System.Collections.Generic;

public enum nextSceneReq {
	once,
	toggle,
	hold,
}


// This class is a peace of Magic, makes it possible to catch some qte
// By default, we're using it to load the next level by both holding
// A Button for 4 seconds ;)
public class ControllerReady : MonoBehaviour {
	
	public nextSceneReq condition = nextSceneReq.toggle;
	public int numberOfPlayers = 2;
	public XboxButton buttonToClick = XboxButton.A;
	public float timeToHold = 4;
	public float timeBeforeAction = 0;
	public int levelToLoad = 1;
	
	private bool[] playerReady;
	private float holdTimer;
	
	// Use this for initialization
	void Start () {
		playerReady = new bool[numberOfPlayers];
		for (int i = 0; i < numberOfPlayers; i++)
		{
			this.playerReady[i] = false;
		}
		holdTimer = timeToHold;
	}
	
	// Update is called once per frame
	// lets wait for both player to be ready :)
	void Update () {
		
		// Are they Ready? :)
		if (areAllPlayersReady())
		{
			// So we can perform our action here :)
			Application.LoadLevel(levelToLoad);
		}
		
	}
	
	bool areAllPlayersReady()
	{
		if (condition == nextSceneReq.once)
		{
			return didTheyClickOnce();
		}
		else if (condition == nextSceneReq.toggle)
		{
			return areTheyToggled();
		}
		
		else if (condition == nextSceneReq.hold)
		{
			return areTheyHoldin();
		}
		else
		{
			// Something went wrong
			return false;
		}
	}
	
	private bool didTheyClickOnce()
	{
		bool retVal = true;
		for (int i = 0; i < numberOfPlayers; i++)
		{
			if (XCI.GetButton(buttonToClick, i+1))
			{
				this.playerReady[i] = true;
			}
			else if (!playerReady[i])
			{
				retVal = false;
			}
		}
		return retVal;
	}
	
	private bool areTheyToggled()
	{
		bool retVal = true;
		for (int i = 0; i < numberOfPlayers; i++)
		{
			if (XCI.GetButton(buttonToClick, i+1))
			{
				if (this.playerReady[i])
				{
					this.playerReady[i] = false;
					retVal = false;
				}
				else
				{
					this.playerReady[i] = true;
				}
			}
			else if (!this.playerReady[i])
			{
				retVal = false;
			}
			Debug.Log(i + " :: " + this.playerReady[i]);
		}
		return retVal;
	}
	
	private bool areTheyHoldin()
	{
		bool reduceTimer = true;
		for (int i = 0; i < numberOfPlayers; i++)
		{
			if (XCI.GetButton(buttonToClick, i+1))
			{
				this.playerReady[i] = true;
			}
			else
			{
				reduceTimer = false;
				this.playerReady[i] = false;
				holdTimer = timeToHold;
				return false;
			}
		}
		if (reduceTimer = true)
			holdTimer -= Time.deltaTime;
		if (holdTimer <= 0)
			return true;
		else
		{
			return false;
		}
	}
		
}
