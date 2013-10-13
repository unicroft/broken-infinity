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
		Debug.Log("Player collision with objective");
		gsm.ObjectiveDestroyed();
		gameObject.GetComponent<MeshRenderer>().enabled = false;
	}
}
