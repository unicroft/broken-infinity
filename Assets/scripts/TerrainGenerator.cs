using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//http://www.youtube.com/watch?v=nPX8dw283X4

[System.Serializable]
public class TerrainGenerator : IEnumerable<Vector3>
{

	public float deltaIncline = 0;
	public float minHeight = 30;
	public float maxHeight = Screen.height - 100;
	
	public float minDeltaX = Screen.width / 3;
	public float minDeltaY = Screen.height / 4;
	public float rangeDeltaX = Screen.width / 6;
	public float rangeDeltaY = Screen.height / 4;

	public float zPositionOfTerrain = 0;
	
	private IEnumerator<Vector3> _internalEnumerator;
	private List<Vector3> _terrainKeyPoints = new List<Vector3>();
	private int _lastGeneratedPointIndex = -1;
	
	public TerrainGenerator(Camera cam)
	{
		_internalEnumerator = this.GetEnumerator();
		
		minHeight = -150;
		maxHeight = cam.orthographicSize - 100;
		minDeltaX = cam.orthographicSize / 2;
		minDeltaY = cam.orthographicSize / 4;
		rangeDeltaX = cam.orthographicSize;
		rangeDeltaY = cam.orthographicSize / 4;
	}
	
	public Vector3 this[int index]
	{
		get
		{
			if ( index > _lastGeneratedPointIndex )
			{
				var numberIteration = index - _lastGeneratedPointIndex;
				for( var i = 0; i < numberIteration; i++)
				{
					++_lastGeneratedPointIndex;
					_terrainKeyPoints.Add(nextValue);
				}
			}
			return _terrainKeyPoints[index];
		}
	}
	
	public int lastGeneratedIndex { get { return _lastGeneratedPointIndex; } }
	
	public Vector3 nextValue
	{
		get
		{
			var val = _internalEnumerator.Current;
			_internalEnumerator.MoveNext();
			
			return val; 
		}
	}
	
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
	
	public IEnumerator<Vector3> GetEnumerator()
	{
		var sign = -1;
		var y = 0f;
		var x = 0f;
		float dx, dy, newY = 0;
		
		yield return new Vector3( -Screen.width, y, zPositionOfTerrain);
		
		while( true )
		{
			Debug.Log("Delta Incline" + deltaIncline);
			dx = Random.Range ( minDeltaX, rangeDeltaX + minDeltaX);
			x += dx;
			
			dy = Random.Range ( minDeltaY, rangeDeltaY + minDeltaY);
			newY = y + dy * sign;
			
			newY = Mathf.Clamp ( newY, minHeight, maxHeight) + deltaIncline;
			y = newY;
			
			if ( deltaIncline != 0)
			{
				if( deltaIncline > 0)
					maxHeight += deltaIncline;
				else if (deltaIncline < 0)
					minHeight += deltaIncline;
			}
			
			sign *= -1;
			
			yield return new Vector3(x, y , zPositionOfTerrain );
		}
	}
	
	public void resetToLastUsedIndex( int lastUsedIndex )
	{
		if ( lastUsedIndex == 0 || _lastGeneratedPointIndex <= 0)
			return;
		
		var numValuesToShift = _lastGeneratedPointIndex - lastUsedIndex + 1;
		for(var i = 0; i < numValuesToShift; i++)
		{
			_terrainKeyPoints[i] = _terrainKeyPoints[lastUsedIndex + i];
		}
		
		_lastGeneratedPointIndex = numValuesToShift - 1;
	}
}

