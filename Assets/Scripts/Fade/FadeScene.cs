using UnityEngine;
using System.Collections;

public class FadeScene : MonoBehaviour {

    public Texture2D tex;
    public Color color;
    public float fadeTime = 1f;
    public bool playOnStart = true;

    private bool opaque = false, fireEvent = false;
    private GameObject target;
    private string action;

    private static FadeScene _fadeScene;

    public static void Out() { Out(null, ""); }
    public static void Out(GameObject target, string action) { ChangeState(false, target, action); }
    public static void In() { In(null, ""); }
    public static void In(GameObject target, string action) { ChangeState(true, target, action); }

    public static void ChangeState(bool opaque, GameObject target, string action)
    {
        if (_fadeScene == null)
        {
            if ((_fadeScene = GameObject.FindObjectOfType(typeof(FadeScene)) as FadeScene) == null)
            {
                Debug.LogError("Fade not found : add FadeScene.prefab in your hierarchy");
                return;
            }
        }
        _fadeScene.action = action;
        _fadeScene.target = target;
        _fadeScene.opaque = opaque;
        _fadeScene.fireEvent = (target != null && action != "");
    }

	void Start () {
        if (this.playOnStart)
            this.color.a = 1f;
        else
            this.color.a = 0f;
	}
	
	void Update () {
        if ((this.color.a <= 0f && !opaque) || (this.color.a >= 1f && opaque))
        {
            if (fireEvent && this.target != null)
            {
                this.fireEvent = false;
                this.target.SendMessage(this.action, SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            if (opaque)
                this.color.a += (float)Time.deltaTime / (float)fadeTime;
            else
                this.color.a -= (float)Time.deltaTime / (float)fadeTime;
        }
	}

    void OnGUI()
    {
        if (this.color.a <= 0) return;
        GUI.color = this.color;
        GUI.depth = -9999;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.tex, ScaleMode.StretchToFill);
    }

}
