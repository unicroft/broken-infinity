using UnityEngine;
using System;
using System.Collections;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager mInstance;
    private GameState mGameState;
	private GameState nextState = null;

    public GameObject Game;
    public GameObject Menu;

    public int GameSequenceIndex;
	
	public int taken;
	private DateTime startTime;

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
		
    }

	// Update is called once per frame
	void Update ()
	{
		if(nextState!=null)
		{
			mGameState.ExitState();

	       	mGameState = nextState;
	
	        mGameState.EnterState();	
			
			nextState = null;
		}
	    mGameState.UpdateState();
	}
	
	public void ObjectiveDestroyed()
	{
		taken++;
	}
	
	public void SwitchState(GameState pNewState)
    {
        nextState = pNewState;
    }
}
