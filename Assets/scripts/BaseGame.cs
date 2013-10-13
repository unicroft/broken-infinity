using UnityEngine;
using System.Collections;
using System.Text;

public abstract class BaseGame : MonoBehaviour
{
    public static bool IsEnvironmentMoving = false;

    public float mJumpForce = 100.0f;

    protected int mStateID = -1;

    void Awake()
    {
        mStateID = -1;
    }

    void Start()
    {
        OnStart();
    }

    void Update()
    {
        UpdateState();
    }

    protected abstract void OnStart();
    protected abstract void EnterState();
    protected abstract void SetSpriteTexture();
    protected abstract void UpdateState();
    protected abstract void ExitState();

    protected void SwitchState(int newStateID)
    {
        ExitState();
        mStateID = newStateID;

        SetSpriteTexture();
        EnterState();
    }
}

public static class Extensions
{
    public static string GetName(this string currentName)
    {
        StringBuilder name = new StringBuilder();
        string[] spaceSplit = currentName.Split(' ');

        foreach (string split in spaceSplit)
        {
            name.Append(split.ToUpper());
        }

        return name.ToString();
    }
}
