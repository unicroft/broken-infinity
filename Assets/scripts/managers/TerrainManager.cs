using UnityEngine;
using System.Collections;

public class TerrainManager : MonoBehaviour {

	public PhysicMaterial physicsMaterial;
	public Material terrainMaterialBorder;
	public Material terrainMaterialInside;
	public TerrainGenerator terrainGenerator;
	
	private Terrain _terrain;
	private Camera _camera;
	private Transform _cameraTransform;
	
	private const int numberOfSUndergroundTile = 0;
	
	private GameObject[] _terrainGameObjects = new GameObject[2];
	private GameObject[][] _undergroundGameObjects = new GameObject[2][];
	private MeshFilter[][] _undergroundMeshFilter = new MeshFilter[2][];
	private int _currentTerrainGameObjectIndex = -1;
	
	private float _meshMaxX;
	private float _deltaMeshMaxX = 3500;
	private float _meshMinX;
	
	void Awake() {

		transform.position = Vector3.zero;
		
		_camera = Camera.main;
		_camera.orthographicSize = Screen.height / 2;
		_cameraTransform = _camera.transform;
		
		//terrain setup
		terrainGenerator = new TerrainGenerator(_camera);
		_terrain = new Terrain( gameObject, terrainGenerator);
		//_terrain.terrainGenerator.testOtherSetting();
		
		_terrain.textureSize = terrainMaterialBorder.mainTexture.height;
		
		for(var i = 0; i <= 1; i++)
		{
			var go = new GameObject( "mesh " + i);
			go.transform.parent = transform;
			go.AddComponent<MeshFilter>();
			
			var collider = go.AddComponent<MeshCollider>();
			collider.material = physicsMaterial;
			collider.smoothSphereCollisions = true;
			
			var meshRenderer = go.AddComponent<MeshRenderer>();
			meshRenderer.sharedMaterial = terrainMaterialBorder;
			
			_terrainGameObjects[i] = go;
		}
		
		for(var i = 0; i < 2; i++)
		{
			_undergroundGameObjects[i] = new GameObject[numberOfSUndergroundTile];
			_undergroundMeshFilter[i] = new MeshFilter[numberOfSUndergroundTile];
			for(var j = 0; j < numberOfSUndergroundTile; j++)
			{
				var go = new GameObject( "undergound " + i + " " + j);
				go.transform.parent = transform;
				_undergroundMeshFilter[i][j] = go.AddComponent<MeshFilter>();
	
				var meshRenderer = go.AddComponent<MeshRenderer>();
				meshRenderer.sharedMaterial = terrainMaterialInside;
				
				_undergroundGameObjects[i][j] = go;
			}
		}
		
		
	}
	
	// Update is called once per frame
	void Update () {
		var cameraMaxX = _cameraTransform.position.x + _camera.orthographicSize * 2;
		var cameraMinX = _cameraTransform.position.x - _camera.orthographicSize * 2;
		if ( cameraMaxX > _meshMaxX)
		{
			_currentTerrainGameObjectIndex = (_currentTerrainGameObjectIndex+1)%2;
			var go = _terrainGameObjects[_currentTerrainGameObjectIndex];
			
			_meshMaxX += 3500;
			_meshMinX = cameraMinX;
			
			_terrain.generateMeshWithWidth( _meshMaxX, go.GetComponent<MeshFilter>() , _undergroundMeshFilter[_currentTerrainGameObjectIndex]);
		}
		//*
		else if( cameraMinX < _meshMinX)
		{
			_currentTerrainGameObjectIndex = (_currentTerrainGameObjectIndex+1)%2;
			var go = _terrainGameObjects[_currentTerrainGameObjectIndex];
			
			while(cameraMinX < _meshMaxX)
				_meshMaxX = _meshMaxX - 3500;
			
			_terrain.generateMeshWithWidth( _meshMaxX, go.GetComponent<MeshFilter>(), _undergroundMeshFilter[_currentTerrainGameObjectIndex] );
		}
		//*/
	}
	
	void OnGUI()
	{
		//_terrain.renderTerrainVisibleToCamera(Camera.main,_terrainGameObjects[_currentTerrainGameObjectIndex].GetComponent<MeshFilter>());
	}
}
