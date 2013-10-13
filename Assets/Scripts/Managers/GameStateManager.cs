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
	
	public int taken { get; private set;}
	private DateTime startTime;

    public static GameStateManager Instance
    {
        get { return mInstance; }
    }
    
    public GameState gameState
    {
        get { return mGameState; }
        set { SwitchState( value); }
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
		if(nextState != null)
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
		Debug.Log("Objcetive augmented");
		taken++;
		GameObject.Find("StarBar").GetComponent<ProgressBar>().addOneIncrement();
	}
	
	public void resetObjective()
	{
		GameObject.Find("StarBar").GetComponent<ProgressBar>().reset();
		taken = 0;	
	}
	
	public void SwitchState(GameState pNewState)
    {
		
		
        	nextState = pNewState;
			Debug.Log("CHANGE STATE");
		
    }
}
