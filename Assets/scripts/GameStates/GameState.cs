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

    protected abstract void EnterState();
    public abstract void UpdateState();
    public abstract void UpdateStateGUI();
    protected abstract void ExitState();

    public void SwitchState(GameState.State pNewState)
    {
        ExitState();

        switch (pNewState)
        {
            case State.MainMenu:
                GameStateManager.Instance.GameState = new MainMenu();
                break;
            case State.IntroState:
                GameStateManager.Instance.GameState = new IntroState();
                break;
            case State.VersusState:
                GameStateManager.Instance.GameState = new VersusState();
                break;
            case State.InfinityState:
                GameStateManager.Instance.GameState = new InfinityState();
                break;
            case State.PauseState:
                GameStateManager.Instance.GameState = new PauseState();
                break;
            case State.EndState:
                GameStateManager.Instance.GameState = new EndState();
                break;
			case State.GameOverState:
                GameStateManager.Instance.GameState = new GameOverState();
                break;
        }

        EnterState();
    }
}
