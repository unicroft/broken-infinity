using System;
using UnityEngine;
using System.Collections;

public abstract class GameState 
{
	public enum State
	{
		MainMenu = 0,
		IntroState = 1,
		VersusState = 2,
		InfinityState = 3,
		PauseState = 4,
		EndState = 5,
		GameOverState = 6
	}
	
	protected State mCurrentState;

    public State CurrentState
    {
        get { return mCurrentState; }
    }

    void Awake()
    {
    }

    void Update()
    {
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void UpdateStateGUI();
    public abstract void ExitState();

    
}
