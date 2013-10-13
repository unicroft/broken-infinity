using UnityEngine;
using System.Collections;
using XboxCtrlrInput;

public class ChangeTerrainState : GameState{
	
	static float[] scores = new float[2];
	
	float[] newScores;
	
	bool asked = false;

	public ChangeTerrainState(float[] _scores)
    {
        mCurrentState = State.ChangeTerrainState;
		newScores = _scores;
		
		for (int i = 0 ; i < 2; i++)
		{
			scores[i] += newScores[i];	
		}
    }

    public override void EnterState()
    {
        
    }

    public override void UpdateState()
    {    
		if(XCI.GetButton(XboxButton.B, 1) || XCI.GetButton(XboxButton.B, 2))
		{
			if(!asked){
				GameStateManager.Instance.SwitchState(new VersusState(scores[0]<scores[1]?0:1));
				asked = true;
			}
		}
    }

    public override void UpdateStateGUI()
    {
        for(int i = 0; i < 2; i++)
		{
			int j  = i +1 ;
			GUI.Box(new Rect(0,50*i,200,50),"Derniere course " + j +": " + newScores[i]);
			GUI.Box(new Rect(0,100+50*i,200,50),"Total " + j +": " + scores[i]);
		}
		GUI.Box(new Rect(100,100,200,50),"Joueur " + ((scores[0]<scores[1]?0:1)+1) + " commence.");
    }

    public override void ExitState()
    {
        GameObject.Find("TerrainManager").GetComponent<TerrainManager>().changeWorld();
    }
}
