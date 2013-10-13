using UnityEngine;
using System.Collections;


public class PlayerPhysics : MonoBehaviour {
	
	public LayerMask collisionMask;
	
	private Collider collider;
	private Vector3 size;
    private Vector3 center;
	private float skin = .005f;
	
	[HideInInspector]
	public bool grounded;
	
	
	Ray ray;
	RaycastHit hit;
	
	public void Start() {
		collider = GetComponent<BoxCollider>();
		size = this.renderer.bounds.size;
		//center = collider.center;
	}
	
	public void Move(Vector2 moveAmount) {
		float deltaX = moveAmount.x;
		float deltaY = moveAmount.y;
		Vector2 p = gameObject.transform.position;
		
		grounded = false;
		for (int i = 0; i < 3; i++) {
			float dir = Mathf.Sign(deltaY);
			float x = (p.x + center.x - size.x / 2) + size.x / 2 * i;
			float y = p.y + center.y + size.y/2 * dir;
			
			ray = new Ray(new Vector2(x,y), new Vector2(0, dir * size.y));
			if (Physics.Raycast(ray, out hit, Mathf.Abs(deltaY), collisionMask)) {
				float dst = Vector3.Distance(ray.origin, hit.point);
				
				if (dst > skin) {
					deltaY = (dst * dir) + skin;
				}
				else {
					deltaY = 0;
				}
				
				grounded = true;
				break;
			}
		}
		
		Vector2 finalTransform = new Vector2(deltaX, deltaY);
		transform.Translate(finalTransform);
    }
}
