using UnityEngine;
using System.Collections;

public class PauseState :  GameState  
{
	
	private GameState _gs;
	
    public PauseState(GameState gs)
    {
        mCurrentState = State.PauseState;
		_gs = gs;
    }

    public override void EnterState()
    {
        
    }

    public override void UpdateState()
    {
       
    }

    public override void UpdateStateGUI()
    {
        if(GUI.Button(new Rect(120,0,100,50),"Resume"))
			GameStateManager.Instance.SwitchState(_gs);
    }

    public override void ExitState()
    {
        
    }
}
