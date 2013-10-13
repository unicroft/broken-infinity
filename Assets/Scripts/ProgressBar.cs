using UnityEngine;
using System.Collections;
	
public class ProgressBar : MonoBehaviour {
	
	public int numberOfIncrements = 2;
	public float IncrementDelay = 2.0f;
	
	public Texture BaseTexture = null;
	public Texture ExtremityTexture = null;
	public Texture BodyTexture = null;
	
	public int BaseWidth = 0;
	public int BaseHeight = 0;
	public int ExtremityWidth = 0;
	public int ExtremityHeight = 0;
	private float BodyWidth = 1;
	public int BodyHeight = 0;
	
	public float BodyRelativePosX = 0;
	public float BodyRelativePosY = 0;
	public float MaxBodyLenght = 10;
	
	public bool upperCorner = true;
	public bool rightSide = true;
	public float MarginX = 5;
	public float MarginY = 5;
	
	private int currentIncrement = 0;
	private float BarPosX;
	private float BarPosY;
	private float deltaTest = 0;
	private bool isUpToDate = false;
	private float incrementSize = 0;
	
	private bool isAnimating = false;
	private float animStartTime = 0;
	private float startingWidth = 0;
	private float endingPos = 0;
	private float endingWidth = 0;
	private float addon = 0;
	
	private Rect Base;
	private Rect Body;
	private Rect Extremity;
	private int i = 0;
	
	private bool isFull = false;
	
	// Use this for initialization
	void Start () 
	{
		// Setup sizes for lbel creation
		if(BaseWidth == 0){
			BaseWidth = BaseTexture.width;
		}
		if(BaseHeight == 0){
			BaseHeight = BaseTexture.height;
		}
		if(ExtremityWidth == 0){
			ExtremityWidth = ExtremityTexture.width;
		}
		if(ExtremityHeight == 0){
			ExtremityHeight = ExtremityTexture.height;
		}
		if(BodyHeight == 0){
			BodyHeight = BodyTexture.height;
		}
		
		//Setup positionning
		if(rightSide){
			BarPosX = Screen.width - BaseWidth - MarginX;
		}
		else{
			BarPosX = MarginX;
		}		
		if(upperCorner){
			BarPosY = MarginY;
		}
		else{
			BarPosY = Screen.height - BaseHeight - MarginY;
		}	
		
		incrementSize = MaxBodyLenght/numberOfIncrements;
		
		Base = new Rect(BarPosX, BarPosY, BaseWidth, BaseHeight);
		Body = new Rect(Base.x + BodyRelativePosX, Base.y + BodyRelativePosY, BodyWidth, BodyHeight);
		Extremity = new Rect(Body.x + Body.width + 1, Body.y, ExtremityWidth, ExtremityHeight);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Body.width >= MaxBodyLenght)
		{
			isFull = true;
		}
		if(Input.GetKeyDown(KeyCode.T)){
			//Debug.Log (Body.width);
			i = i + 1;
			addOneIncrement();
			Debug.Log (isFulled());
		}
		
		endingPos = incrementSize * currentIncrement;
		
		if(Body.width < endingPos){
			//(incrementSize * currentIncrement)
			
			if(isAnimating == false){
				//animStartTime = Time.time;
				startingWidth = BodyWidth;
				endingWidth = endingPos - Body.width;
				isAnimating = true;
			}
			
			if(isAnimating == true){
				float elapsed = Time.time - animStartTime;
				
				if(elapsed > IncrementDelay){
					BodyWidth = endingPos;
					isAnimating = false;
				}
				else{
					addon = EaseOut(elapsed, 0, endingWidth, IncrementDelay);
					BodyWidth = startingWidth + addon;
				}
			}
			Body.width = BodyWidth;
			Extremity.x = Body.width + Body.x;
		}
		
		
	}
	
	void OnGUI()
	{
		GUI.DrawTexture(Base, BaseTexture, ScaleMode.ScaleToFit);
		if(BodyWidth != 1){
			GUI.DrawTexture(Body, BodyTexture, ScaleMode.StretchToFill);
		}
		else{
			GUI.DrawTexture(Body, BodyTexture, ScaleMode.ScaleAndCrop);
		}
		GUI.DrawTexture(Extremity, ExtremityTexture, ScaleMode.ScaleAndCrop);
	}
	
	public void addOneIncrement(){
		if(currentIncrement < numberOfIncrements)
		{
			isAnimating	= false;
			animStartTime = Time.time;
			currentIncrement = currentIncrement + 1;
		}
	}
	
	public bool isFulled(){
		return isFull;
	}
	
	// t = Current time in seconds (since start?).
    // b = Starting value.
    // c = Final value.
    // d = Duration of animation.
	private float EaseOut( float t, float b, float c, float d )
    {
		t /= d;
		
		float temp = -c * t*(t-2) + b;
		if(temp > c){
			return c;
		}
		else {
			return temp;
		}

    }

	
}
