using UnityEngine;
using System.Collections;

public class TerrainManager : MonoBehaviour {

	public PhysicMaterial physicsMaterial;
	public Material terrainMaterial;
	
	private Terrain _terrain;
	private Camera _camera;
	private Transform _cameraTransform;
	
	private GameObject[] _terrainGameObjects = new GameObject[2];
	private int _currentTerrainGameObjectIndex = -1;
	private float _meshMaxX;
	
	void Awake() {

		transform.position = Vector3.zero;
		_camera = Camera.main;
		_cameraTransform = _camera.transform;
		
		//terrain setup
		_terrain = new Terrain( gameObject );
		//_terrain.terrainGenerator.testOtherSetting();
		
		Texture2D mainTexture = new Texture2D(32,32);
		
		Color[] colors = new Color[32*32];
		
		for(var i = 0; i < 32 * 32; i++)
		{
			colors[i] = Color.green;
		}
		
		mainTexture.SetPixels(colors);
		mainTexture.Apply();
		
		terrainMaterial.mainTexture = Resources.Load("GreenSpeckledTile") as Texture;
		
		for(var i = 0; i <= 1; i++)
		{
			var go = new GameObject( "mesh " + i);
			go.transform.parent = transform;
			go.AddComponent<MeshFilter>();
			
			var collider = go.AddComponent<MeshCollider>();
			collider.material = physicsMaterial;
			collider.smoothSphereCollisions = true;
			
			var meshRenderer = go.AddComponent<MeshRenderer>();
			meshRenderer.sharedMaterial = terrainMaterial;
			
			_terrainGameObjects[i] = go;
		}
	}
	
	// Update is called once per frame
	void Update () {
		var cameraMaxX = _cameraTransform.position.x + _camera.orthographicSize * 2;
		if ( cameraMaxX > _meshMaxX)
		{
			_currentTerrainGameObjectIndex = (_currentTerrainGameObjectIndex+1)%2;
			var go = _terrainGameObjects[_currentTerrainGameObjectIndex];
			
			_meshMaxX += 3500;
			
			_terrain.generateMeshWithWidth( _meshMaxX, go.GetComponent<MeshFilter>() );
		}
		Camera.main.transform.Translate(new Vector3(7,0,0));
	}
	
	void OnGUI()
	{
		//_terrain.renderTerrainVisibleToCamera(Camera.main,_terrainGameObjects[_currentTerrainGameObjectIndex].GetComponent<MeshFilter>());
	}
}
