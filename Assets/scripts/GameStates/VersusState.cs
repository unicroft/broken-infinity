using UnityEngine;
using System.Collections;

public class VersusState :  GameState  
{
 	bool started = false;
	
    public VersusState()
    {
        mCurrentState = State.VersusState;
    }

    public override void EnterState()
    {
        GameObject go = new GameObject();
		go.AddComponent<MeshFilter>();
		
		var mesh = go.AddComponent<MeshRenderer>();
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateStateGUI()
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState()
    {
        throw new System.NotImplementedException();
    }
}
