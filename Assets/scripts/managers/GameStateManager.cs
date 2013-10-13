using UnityEngine;
using System;
using System.Collections;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager mInstance;
    private GameState mGameState;

    public GameObject Game;
    public GameObject Menu;

    public int GameSequenceIndex;
	
	private int taken;
	private DateTime startTime;
	
	private GameObject player;

    public static GameStateManager Instance
    {
        get { return mInstance; }
        set { mInstance = value; }
    }
    
    public GameState GameState
    {
        get { return mGameState; }
        set { mGameState = value; }
    }
  
    void Awake()
    {
        mInstance = this.gameObject.GetComponent<GameStateManager>();
		player = GameObject.Find ("Player");
		player.GetComponent<PlayerController>().enabled = false;
        //mGameState = new ActionState();
        GameSequenceIndex = 0;
		
		mGameState = new VersusState();
		mGameState.EnterState();
    }

	// Use this for initialization
	void Start () 
    {

	}
	
    void OnGUI()
    {
       	mGameState.UpdateStateGUI();
		GUI.Box (new Rect (0,0,100,50),"Power: " + taken + "/20");
		
    }

	// Update is called once per frame
	void Update ()
	{
	    mGameState.UpdateState();
	}
	
	public void ObjectiveDestroyed()
	{
		taken++;
	}
	
	public void SwitchState(GameState pNewState)
    {
        mGameState.ExitState();

       	mGameState = pNewState;

        mGameState.EnterState();
    }
}
