using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	public Transform player;
	
	private Camera _camera;
	private new Transform _transform;
	private float _preferredOrthographicSize;
	private float _targetOrthographicSize;
	private float _maxOrthographicSize;
	private float _neutralVerticalOffset = 0;
	
	private float zoom 
	{
		get 
		{ 
			return _preferredOrthographicSize / _camera.orthographicSize;
		}
	}
	
	private bool _pinnedToTop = false;
	//private PlayerController _playerController;
	

	// Use this for initialization
	void Start () {
		_camera = Camera.main;
		_transform = _camera.transform;
		_preferredOrthographicSize = Screen.height / 2;
		_maxOrthographicSize = _preferredOrthographicSize * 2;
		_camera.orthographicSize = _preferredOrthographicSize;
		
		//_playerController = player.GetComponent<PlayerController>();
		
		
	}
	
	// Update is called once per frame
	void LateUpdate () {	
		_transform.Translate(new Vector3(25,0,0));
	}
}
