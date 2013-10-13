using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Terrain
{
	public TerrainGenerator terrainGenerator;
	public List<Vector3> borderVertices = new List<Vector3>();
	public float startX;
	public float endX;
	
	int _fromKeyPointI;
	int _toKeyPointI;
	int prevFromKeyPointI = -1;
	int prevToKeyPointI = -1;
	
	public int terrainSegmentWidth = 2;
	public int textureSize = 128;
	
	private GameObject _originGameObject;
	
	public Terrain( GameObject originGameObject, TerrainGenerator mterrainGenerator)
	{
		_originGameObject = originGameObject;
		terrainGenerator = mterrainGenerator;	
	}
	
	public void generateMeshWithWidth(float width, MeshFilter meshFilter, MeshFilter[] underGroundFilter, List<GameObject> objs) {
		//terrainGenerator.resetToLastUsedIndex( _toKeyPointI);
		
		int prevFromKeyPointI = _fromKeyPointI;
		int prevToKeyPointI = _toKeyPointI;
		
		_fromKeyPointI = prevToKeyPointI;
		_toKeyPointI = _fromKeyPointI;
		
		if(terrainGenerator[_fromKeyPointI].x > width)
		{
			_fromKeyPointI = 0;
			_toKeyPointI = 0;
		}
		
		while( terrainGenerator[++_toKeyPointI].x < width) {}
		
		var start = DateTime.Now;
		drawMesh(meshFilter, underGroundFilter, objs);
		//Debug.Log(DateTime.Now - start);
	}
	
	private void drawMesh(MeshFilter meshFilter, MeshFilter[] underGroundFilter, List<GameObject> objs)
	{
		borderVertices.Clear();
		
		var undergoundHeight = underGroundFilter.Length;
		
		var terrainVertices = new List<Vector3>();
		var terrainTextCoords = new List<Vector2>();
		var triangles = new List<int>();
		var trianglesIndex = -2;

		var terrainVerticesUnderground = new List<Vector3>[undergoundHeight];
		
		for(int q = 0; q < undergoundHeight; q ++ )
		{
			terrainVerticesUnderground[q] = new List<Vector3>();
		}
		
		GameObject original = GameObject.Find("Objective");
		
		foreach(GameObject go in objs)
		{
			GameObject.Destroy(go);	
		}
		
		objs.Clear ();
		
		Vector3 keyPoint0, keyPoint1, pt0, pt1 = new Vector3(0 ,0 ,terrainGenerator.zPositionOfTerrain);
		keyPoint0 = terrainGenerator[_fromKeyPointI];
		
		for(int i = _fromKeyPointI + 1; i <= _toKeyPointI; i++)
		{
			keyPoint1 = terrainGenerator[i];
			
			int totalSegments = Mathf.CeilToInt( ( keyPoint1.x-keyPoint0.x ) / terrainSegmentWidth );
			float segmentWidth = ( keyPoint1.x - keyPoint0.x) / totalSegments;
			float da = Mathf.PI / totalSegments;
			float ymid = ( keyPoint0.y + keyPoint1.y ) / 2;
			float xmid = ( keyPoint0.x + keyPoint1.x ) / 2;
			float amplitude  = (keyPoint0.y - keyPoint1.y ) / 2;
			pt0 = keyPoint0;
			
			if ( i == _toKeyPointI)
				totalSegments++;
			
			if(i % 5 == 4 || i % 7 == 4 || i % 6 == 4)
			{
				GameObject go = GameObject.Instantiate(original) as GameObject;
				go.transform.Translate(new Vector3(-xmid-1000,-ymid - 70,0));
				go.GetComponent<MeshRenderer>().enabled = true;
				go.GetComponent<BoxCollider>().enabled = true;
				go.name = "generatedObjective";
				objs.Add (go);
			}
			
			for (var j = 0; j <= totalSegments; j++)
			{
				pt1.x = keyPoint0.x + j * segmentWidth;
				pt1.y = ymid + amplitude * Mathf.Cos ( da * j);
				
				var topVert = new Vector3( pt0.x, pt0.y, terrainGenerator.zPositionOfTerrain);
				
				borderVertices.Add(topVert);
				
				terrainVertices.Add(topVert);
				terrainTextCoords.Add( new Vector2 ( pt0.x / textureSize, 1 ) );
				terrainVertices.Add( new Vector3 (pt0.x , pt0.y - textureSize, terrainGenerator.zPositionOfTerrain) );
				terrainTextCoords.Add( new Vector2 ( pt0.x / textureSize, 0) );
				
				for(int q = 0; q < undergoundHeight; q ++ )
				{
					var q1 = q + 1;
					terrainVerticesUnderground[q].Add(new Vector3(pt0.x, pt0.y - (textureSize * q1), terrainGenerator.zPositionOfTerrain));				
					terrainVerticesUnderground[q].Add(new Vector3(pt0.x, pt0.y - (textureSize * (q1+1)), terrainGenerator.zPositionOfTerrain) );
				}
				
				if( trianglesIndex >= 2)
				{
					triangles.Add( trianglesIndex + 2);
					triangles.Add( trianglesIndex + 1);
					triangles.Add( trianglesIndex + 0);
					triangles.Add( trianglesIndex + 3);
					triangles.Add( trianglesIndex + 1);
					triangles.Add( trianglesIndex + 2);
				}
				trianglesIndex += 2;
				
				pt0 = pt1;
			}
			
			keyPoint0 = keyPoint1;
		}
		
		var mesh = meshFilter.mesh;
		mesh.Clear();
		mesh.vertices = terrainVertices.ToArray();
		mesh.uv = terrainTextCoords.ToArray();
		mesh.triangles = triangles.ToArray();
		
		mesh.RecalculateBounds();
		
		addMeshCollider ( meshFilter, borderVertices );
		//*
		for(int q = 0; q < undergoundHeight; q ++ )
		{
			mesh = underGroundFilter[q].mesh;
			mesh.Clear();
			mesh.vertices = terrainVerticesUnderground[q].ToArray();
			mesh.uv = terrainTextCoords.ToArray();
			mesh.triangles = triangles.ToArray();
			
			mesh.RecalculateBounds();
		}
		//*/
	}
	
	private void addMeshCollider( MeshFilter meshFilter, List<Vector3> borderVerts) 
	{
		borderVerts.Insert( 0, new Vector3( borderVerts[0].x, borderVerts[0].y - 2* textureSize + terrainGenerator.deltaIncline, borderVerts[0].z ) );
		borderVerts.Add ( new Vector3( borderVerts[borderVerts.Count - 1].x, 
			borderVerts[borderVerts.Count - 1].y - 2*textureSize + terrainGenerator.deltaIncline, 
			borderVerts[borderVerts.Count - 1].z ) );
		
		List<Vector3> verticesList = new List<Vector3>(borderVerts);
		List<int> indices = new List<int>();
		
		for( var i = 0; i < borderVerts.Count ; i++)
			verticesList.Add(new Vector3(borderVertices[i].x, borderVertices[i].y, terrainGenerator.zPositionOfTerrain + 100) );
		
		int N = borderVerts.Count ;
		
		for( var i = 0; i < borderVerts.Count; i++)
		{
			int i1 = i;
			int i2 = (i1 +1) % N;
			int i3 = i1 + N;
			int i4 = i2 + N;
			
			indices.Add(i1);
			indices.Add(i3);
			indices.Add(i4);
			indices.Add(i1);
			indices.Add(i4);
			indices.Add(i2);			
		}
		
		var mesh = new Mesh();
		mesh.vertices = verticesList.ToArray();
		mesh.triangles = indices.ToArray();
		mesh.RecalculateBounds();
		
		meshFilter.GetComponent<MeshCollider>().sharedMesh = mesh;
	}
	
	private bool calculateVisibleVertices( Camera camera ) 
	{
		return true;
	}
	
	public void renderTerrainVisibleToCamera( Camera camera, MeshFilter meshFilter) 
	{
		
	}

}

