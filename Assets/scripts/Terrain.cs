using UnityEngine;
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
	
	public int terrainSegmentWidth = 10;
	public int textureSize = 1024;
	
	private GameObject _originGameObject;
	
	public Terrain( GameObject originGameObject) {
		_originGameObject = originGameObject;
		terrainGenerator = new TerrainGenerator();
	}
	
	public void generateMeshWithWidth(float width, MeshFilter meshFilter) {
		terrainGenerator.resetToLastUsedIndex( _toKeyPointI);
		
		_fromKeyPointI = 0;
		_toKeyPointI = 0;
		
		while( terrainGenerator[++_toKeyPointI].x < width) {}
		
		drawMesh(meshFilter);
	}
	
	private void drawMesh(MeshFilter meshFilter)
	{
		borderVertices.Clear();
		
		var terrainVertices = new List<Vector3>();
		var terrainTextCoords = new List<Vector2>();
		var triangles = new List<int>();
		var trianglesIndex = -2;
		
		Vector3 keyPoint0, keyPoint1, pt0, pt1 = new Vector3(0 ,0 ,terrainGenerator.zPositionOfTerrain);
		keyPoint0 = terrainGenerator[_fromKeyPointI];
		
		for(int i = _fromKeyPointI + 1; i <= _toKeyPointI; i++)
		{
			keyPoint1 = terrainGenerator[i];
			
			int totalSegments = Mathf.CeilToInt( ( keyPoint1.x-keyPoint0.x ) / terrainSegmentWidth );
			float segmentWidth = ( keyPoint1.x - keyPoint0.x) / totalSegments;
			float da = Mathf.PI / totalSegments;
			float ymid = ( keyPoint0.y + keyPoint1.y ) / 2;
			float amplitude  = (keyPoint0.y - keyPoint1.y ) / 2;
			pt0 = keyPoint0;
			
			if ( i == _toKeyPointI)
				totalSegments++;
			
			for (var j = 0; j <= totalSegments; j++)
			{
				pt1.x = keyPoint0.x + j * segmentWidth;
				pt1.y = ymid + amplitude * Mathf.Cos ( da * j);
				
				var topVert = new Vector3( pt0.x, pt0.y, terrainGenerator.zPositionOfTerrain);
				
				borderVertices.Add(topVert);
				
				terrainVertices.Add(topVert);
				terrainTextCoords.Add( new Vector2 ( pt0.x / textureSize, 1 ) );
				terrainVertices.Add( new Vector3 (pt0.x , pt0.y -textureSize, terrainGenerator.zPositionOfTerrain) );
				terrainTextCoords.Add( new Vector2 ( pt0.x / textureSize, 0) );
				
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
		mesh.Clear ();
		mesh.vertices = terrainVertices.ToArray();
		mesh.uv = terrainTextCoords.ToArray();
		mesh.triangles = triangles.ToArray();
		
		mesh.RecalculateBounds();
		
		addMeshCollider ( meshFilter, borderVertices );
	}
	
	private void addMeshCollider( MeshFilter meshFilter, List<Vector3> borderVerts) 
	{
		borderVerts.Insert( 0, new Vector3( borderVerts[0].x, borderVerts[0].y - textureSize + terrainGenerator.deltaIncline, borderVerts[0].z ) );
		borderVerts.Add ( new Vector3( borderVerts[borderVerts.Count - 1].x, 
			borderVerts[borderVerts.Count - 1].y - textureSize + terrainGenerator.deltaIncline, borderVerts[borderVerts.Count - 1].z ) );
		
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

