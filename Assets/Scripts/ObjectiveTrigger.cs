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
		gsm.ObjectiveDestroyed();
		gameObject.GetComponent<MeshRenderer>().enabled = false;
		gameObject.GetComponent<BoxCollider>().enabled = false;
	}
}
