using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainManager : MonoBehaviour {

	public PhysicMaterial physicsMaterial;
	public Material terrainMaterialBorder;
	public Material terrainMaterialInside;
	public TerrainGenerator terrainGenerator;
	
	private Terrain _terrain;
	private Camera _camera;
	private Transform _cameraTransform;
	
	private const int numberOfSUndergroundTile = 2;
	
	private GameObject[] _terrainGameObjects = new GameObject[2];
	private GameObject[][] _undergroundGameObjects = new GameObject[2][];
	private MeshFilter[][] _undergroundMeshFilter = new MeshFilter[2][];
	private List<GameObject>[] _objs = new List<GameObject>[2];
	
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
		ResetTerrain();
		ChangeTerrainColor(new Color(Random.value/2+0.5f,Random.value/2+0.5f,Random.value/2+0.5f,1.0f));
		
		for(var i = 0; i <= 1; i++)
		{
			var go = new GameObject( "mesh " + i);
			go.transform.parent = transform;
			go.isStatic = true;
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
			_objs[i] = new List<GameObject>();
			
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
	
	public void ResetTerrain()
	{
			
		_terrain = new Terrain( gameObject, terrainGenerator);
		_terrain.textureSize = terrainMaterialBorder.mainTexture.height;
		_meshMaxX = 0;
		
		foreach(List<GameObject> lgo in _objs)
		{
			if(lgo != null)
			{
				foreach(GameObject go in lgo)
				{
					GameObject.Destroy(go);	
				}
				lgo.Clear ();
			}
		}
	}
	
	public void ChangeTerrainColor(Color color)
	{
		terrainMaterialBorder.color = color;
		terrainMaterialInside.color = color;
	}
	
	public void changeWorld()
	{
		foreach(GameObject go in _terrainGameObjects)
		{
			go.GetComponent<MeshCollider>().enabled = false;
			go.GetComponent<MeshRenderer>().enabled = false;
		}
		
		foreach(GameObject[] goa in _undergroundGameObjects)
		{
			foreach(GameObject go in goa)
			{
				go.GetComponent<MeshRenderer>().enabled = false;	
			}
		}
		
		terrainGenerator = new TerrainGenerator(_camera);
		ResetTerrain();
		ChangeTerrainColor(new Color(Random.value/2+0.5f,Random.value/2+0.5f,Random.value/2+0.5f,1.0f));
	}
	
	// Update is called once per frame
	void Update () {
		var cameraMaxX = _cameraTransform.position.x + _camera.orthographicSize * 2;
		var cameraMinX = _cameraTransform.position.x - _camera.orthographicSize * 2;
		if ( cameraMaxX > _meshMaxX)
		{
			_currentTerrainGameObjectIndex = (_currentTerrainGameObjectIndex+1)%2;
			var go = _terrainGameObjects[_currentTerrainGameObjectIndex];
			
			go.GetComponent<MeshCollider>().enabled = true;
			go.GetComponent<MeshRenderer>().enabled = true;
			
			foreach(GameObject gob in _undergroundGameObjects[_currentTerrainGameObjectIndex])
			{
				gob.GetComponent<MeshRenderer>().enabled = true;	
			}
			
			_meshMaxX += 3500;
			_meshMinX = cameraMinX;
			
			_terrain.generateMeshWithWidth( _meshMaxX, go.GetComponent<MeshFilter>() , _undergroundMeshFilter[_currentTerrainGameObjectIndex], _objs[_currentTerrainGameObjectIndex]);
		}
		/*
		else if( cameraMinX < _meshMinX)
		{
			_currentTerrainGameObjectIndex = (_currentTerrainGameObjectIndex+1)%2;
			var go = _terrainGameObjects[_currentTerrainGameObjectIndex];
			
			while(cameraMinX < _meshMaxX)
				_meshMaxX = _meshMaxX - 3500;
			
			_terrain.generateMeshWithWidth( _meshMaxX, go.GetComponent<MeshFilter>(), _undergroundMeshFilter[_currentTerrainGameObjectIndex], _objs[_currentTerrainGameObjectIndex] );
		}
		//*/
	}
	
	void OnGUI()
	{
		//_terrain.renderTerrainVisibleToCamera(Camera.main,_terrainGameObjects[_currentTerrainGameObjectIndex].GetComponent<MeshFilter>());
	}
}
