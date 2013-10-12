using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//http://www.youtube.com/watch?v=nPX8dw283X4

public class TerrainGenerator : IEnumerable<Vector3>
{
	public float deltaIncline = 0;
	public float minHeight = 30;
	public float maxHeight = Screen.height - 100;
	
	public float minDeltaX = Screen.width / 3;
	public float minDeltaY = Screen.height / 5;
	public float rangeDeltaX = Screen.width / 6;
	public float rangeDeltaY = Screen.width / 8;
	
	public float zPositionOfTerrain = 0;
	
	private IEnumerator<Vector3> _internalEnumerator;
	private Vector3[] _terrainKeyPoints = new Vector3[1000];
	private int _lastGeneratedPointIndex = -1;
	
	public TerrainGenerator()
	{
		_internalEnumerator = this.GetEnumerator();
		
		maxHeight = Camera.main.orthographicSize - 100;
		minDeltaX = Camera.main.orthographicSize / 2;
		minDeltaY = Camera.main.orthographicSize / 4;
		rangeDeltaX = Camera.main.orthographicSize;
		rangeDeltaY = Camera.main.orthographicSize / 4;
	}
	
	public Vector3 this[int index]
	{
		get
		{
			if ( index > _lastGeneratedPointIndex )
			{
				for( var i = 0; i <= index - _lastGeneratedPointIndex; i++)
					_terrainKeyPoints[++_lastGeneratedPointIndex] = nextValue;
			}
			return _terrainKeyPoints[index];
		}
	}
	
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

