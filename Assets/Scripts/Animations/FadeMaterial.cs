using UnityEngine;
using System.Collections;

public enum FadeType {
	fadeIn, // will fade in untill its opaque
	fadeOut, // will fade out untill its invisible
	fadeInfadeOut // Will loop animation
}

public class FadeMaterial : MonoBehaviour {
	// In the target object
	
	public FadeType anim = FadeType.fadeIn;
	
    public float fadeTime = 1f;
    public bool visibleOnStart = true;
	
    private bool opaque = false, fireEvent = false, finished = false, hasEndedAnimation = false, fadingOut;
    
	private GameObject target;
    
	private string action;
	public bool paused = false;

    private static FadeMaterial _fadeMaterial;

    public static void Out() { Out(null, ""); }
    public static void Out(GameObject target, string action) { ChangeState(false, target, action); }
    public static void In() { In(null, ""); }
    public static void In(GameObject target, string action) { ChangeState(true, target, action); }

    public static void ChangeState(bool opaque, GameObject target, string action)
    {
        if (_fadeMaterial == null)
        {
            if ((_fadeMaterial = GameObject.FindObjectOfType(typeof(FadeMaterial)) as FadeMaterial) == null)
            {
                Debug.LogError("Fade not found : add FadeMaterial.prefab in your hierarchy");
                return;
            }
        }
        _fadeMaterial.action = action;
        _fadeMaterial.target = target;
        _fadeMaterial.opaque = opaque;
        _fadeMaterial.fireEvent = (target != null && action != "");
    }

	void Start () {
        if (this.visibleOnStart)
		{
       		renderer.material.SetAlpha(1f);
			fadingOut = true;
		}
        else
		{
       		renderer.material.SetAlpha(0f);
			fadingOut = false;
		}
	}
	
	void Update () {
        if ((renderer.material.color.a <= 0f && !opaque) || (renderer.material.color.a >= 1f && opaque))
        {
            if (fireEvent && this.target != null)
            {
                this.fireEvent = false;
                this.target.SendMessage(this.action, SendMessageOptions.DontRequireReceiver);
            }
        }
		if (!finished || !paused)
		{
			if (anim == FadeType.fadeIn)
			{
				if (!isOpaque())
				{
					fadeIn();
				}
				else
				{
					finished = true;
				}
			}
			else if (anim == FadeType.fadeOut)
			{
				if (isVisible())
				{
					fadeOut();
				}
				else
				{
					finished = true;
				}
			}
			else if (anim == FadeType.fadeInfadeOut)
			{
				if (fadingOut)
				{
					if (isVisible())
					{
						fadeOut();
					}
					else
					{
						fadingOut = false;
					}
				}
				else
				{
					if (isOpaque())
					{
						fadingOut = true;
					}
					else
					{
						fadeIn();
					}
				}
			}
		}
		else if (finished)
		{
			hasEndedAnimation = true;
		}
    }
	
	void fadeIn()
	{
		float modifier = renderer.material.color.a + Time.deltaTime / fadeTime;
		renderer.material.SetAlpha(modifier);
		
	}
	void fadeOut()
	{
		float modifier = renderer.material.color.a - Time.deltaTime / fadeTime;
		renderer.material.SetAlpha(modifier);
	}
	
	public bool isOpaque()
	{
		return (renderer.material.color.a >= 1.0f);
	}
	public bool isVisible()
	{
		return (renderer.material.color.a >= 0.0f);
	}

}
