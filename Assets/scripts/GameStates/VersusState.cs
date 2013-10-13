using UnityEngine;
using System.Collections;

public class VersusState :  GameState  
{
	bool init = false;
 	bool started = false;
	bool ended = false;
	float timeremaining;
	int currentplayer = 0;
	
	public int nbObjective = 1;
	
	float[] result = new float[2];
	
	private GameObject _player;
	private Vector3 savedVelocity;
	
    public VersusState()
    {
        mCurrentState = State.VersusState;
		_player = GameObject.Find("player");
    }
	
	public VersusState(int startPlayer) :this()
	{
		currentplayer = startPlayer;
		//_player.GetComponent<PlayerController>().joystick_id = currentplayer + 1;
	}

    public override void EnterState()
    {
		if(!started)
		{
			_player.GetComponent<PlayerController>().enabled = false;
			_player.GetComponent<Rigidbody>().useGravity = false;
			_player.GetComponentInChildren<SpriteAnimator>().IsPaused = true;
			timeremaining = 3;
		}
		else if(!ended)
		{
			_player.GetComponent<PlayerController>().enabled = true;
			_player.GetComponent<Rigidbody>().useGravity = true;
			_player.GetComponentInChildren<SpriteAnimator>().IsPaused = false;
			
			_player.GetComponent<Rigidbody>().velocity = savedVelocity;
		}
    }

    public override void UpdateState()
    {
		if(!started)
		{
	        timeremaining -= Time.deltaTime;
			if(timeremaining < 0)
			{
				_player.GetComponent<PlayerController>().enabled = true;
				_player.GetComponent<Rigidbody>().useGravity = true;
				_player.GetComponentInChildren<SpriteAnimator>().IsPaused = false;
				MasterAudio.PlaySound("MonsterGrowl",_player.transform,"MonsterGrowl",true, 0f);
				started = true;
			}
		}
		else if(!ended)
		{
			timeremaining += Time.deltaTime;
			if(GameStateManager.Instance.taken >= nbObjective)
			{
				
				MasterAudio.PlaySound("Teleport",_player.transform,"Teleport",true, 0f);
				
				_player.GetComponent<PlayerController>().enabled = false;
				result[currentplayer] = timeremaining;
				
				currentplayer = (currentplayer+1)%2;
				//_player.GetComponent<PlayerController>().joystick_id = currentplayer + 1;
				
				Camera.main.transform.position = new Vector3(0,1,-10);
				
				_player.transform.position = new Vector3(80,0,20);
				_player.transform.rotation = Quaternion.identity;
				
				_player.GetComponent<Rigidbody>().useGravity = false;
				_player.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
				_player.GetComponentInChildren<SpriteAnimator>().IsPaused = true;
				GameStateManager.Instance.resetObjective();
				
				GameObject.Find("TerrainManager").GetComponent<TerrainManager>().ResetTerrain();
				
				if(result[currentplayer] == 0)
				{
					started = false;
					timeremaining = 3;
				}
				else
				{
					ended = true;
					GameStateManager.Instance.SwitchState(new ChangeTerrainState(result));
				}
			}
		}
    }

    public override void UpdateStateGUI()
    {
		GUI.Box (new Rect (0,0,100,50),"Power: " + GameStateManager.Instance.taken + "/" + nbObjective);
		GUI.Box (new Rect (0,20,100,50),"Time: " + timeremaining);
		GUI.Box (new Rect (0,40,100,50),"Player: " + currentplayer);
		GUI.Box (new Rect (0,60,100,50),"Started: " + started);
		GUI.Box (new Rect (0,80,100,50),"Ended: " + ended);
		
		for(int i = 0; i < 2; i++)
		{
			GUI.Box(new Rect(0,100+20*i,100,50),"Player " + i +": " + result[i]);
		}
		
		if(GUI.Button(new Rect(120,0,100,50),"Pause"))
			GameStateManager.Instance.SwitchState(new PauseState(this));
    }

    public override void ExitState()
    {
        _player.GetComponent<PlayerController>().enabled = false;
		_player.GetComponent<Rigidbody>().useGravity = false;
		_player.GetComponentInChildren<SpriteAnimator>().IsPaused = true;
		savedVelocity = _player.GetComponent<Rigidbody>().velocity;
		_player.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
    }
}
