using UnityEngine;
using System.Collections;

public class ObjectiveTrigger : MonoBehaviour {
	
	private GameStateManager gsm;

	// Use this for initialization
	void Start () {
		gsm = GameStateManager.Instance;
	}
	
	void OnTriggerEnter(Collider coll)
	{
		if(coll.gameObject.name == "player")
		{
			gsm.ObjectiveDestroyed();
			gameObject.GetComponent<MeshRenderer>().enabled = false;
			gameObject.GetComponent<BoxCollider>().enabled = false;
			MasterAudio.PlaySound("Powerup",coll.gameObject.transform,"powerup",true,0f);
		}
		
	}
}
