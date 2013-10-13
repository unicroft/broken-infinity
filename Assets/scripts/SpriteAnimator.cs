using UnityEngine;
using System.Collections;

public class SpriteAnimator : MonoBehaviour
{
 	public int Columns = 5;
    public int Rows = 5;
    public float FramesPerSecond = 10f;
    public bool RunOnce = true;
	public bool IsPaused = true;
	
	private bool isAnimating = false;
	public bool hasPlayed = false;
	public bool reverseSprite = false;
 
    public float RunTimeInSeconds
    {
        get
        {
            return ( (1f / FramesPerSecond) * (Columns * Rows) );
        }
    }
 
    private Material materialCopy = null;
 
    void Start()
    {
        // Copy its material to itself in order to create an instance not connected to any other
        materialCopy = new Material(renderer.sharedMaterial);
        renderer.sharedMaterial = materialCopy;
 
        Vector2 size = new Vector2(1f / Columns, 1f / Rows);
        renderer.sharedMaterial.SetTextureScale("_MainTex", size);
    }
 
	void Update()
	{
		if (!isAnimating && !IsPaused)
		{
			if (RunOnce)
			{
				if (!hasPlayed)
					StartCoroutine(UpdateTiling());
			}
			else 
				StartCoroutine(UpdateTiling());
		}
	}
	
    private IEnumerator UpdateTiling()
    {
		isAnimating = true;
        float x = 0f;
        float y = 0f;
        Vector2 offset = Vector2.zero;
 
        while (true)
        {
            for (int i = Rows-1; i >= 0; i--) // y
            {
                y = (float) i / Rows;
 
                for (int j = 0; j <= Columns-1; j++) // x
                {
					if (!IsPaused)
					{
	                    x = (float) j / Columns;
	 
						if (reverseSprite)
	                    	offset.Set(x, y);
						else
							offset.Set(x, y);
	 
	                    renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
					}
					else j--;
                    yield return new WaitForSeconds(1f / FramesPerSecond);
                }
            }
			hasPlayed = true;
 
            if (RunOnce)
            {
				isAnimating = false;
                yield break;
            }
        }
    }
}
