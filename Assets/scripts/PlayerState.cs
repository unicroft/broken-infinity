using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerState : BaseGame
{
    public enum State
    {
        Idle, 
        Running, 
        Jumping, 
    }

//    public BackgroundTranslate mSpeedReference = null;
    
    public AudioSource mJumpSound = null;
    
    Stack<int> mIsDoingAction = new Stack<int>();
    bool mIsOnGround = false;

    void OnCollisionEnter(Collision collision)
    {
//        BackgroundTranslate ground = collision.gameObject.GetComponent<BackgroundTranslate>();
        
        //if (ground != null)
        //{
            mIsOnGround = true;
        //}
    }

    void OnCollisionExit(Collision collision)
    {
  //      BackgroundTranslate ground = collision.gameObject.GetComponent<BackgroundTranslate>();

        //if (ground != null)
        //{
            mIsOnGround = false;
        //}
    }

    protected override void OnStart()
    {
        BaseGame.IsEnvironmentMoving = false;
        SwitchState((int)State.Idle);
    }

    protected override void EnterState()
    {
        switch ((State)mStateID)
        {
            case State.Idle:
                {
                }
                break;

            case State.Running:
                {
                    BaseGame.IsEnvironmentMoving = true;

                    StartCoroutine(coRun());

                }
                break;

            case State.Jumping:
                {
                    mIsOnGround = false;

                    StartCoroutine(coJump());

                    if (mJumpSound != null)
                    {
          //              SoundManager.Instance.Play(mJumpSound);
                    }
                }
                break;

            default: break;
        }
    }

    void OnGUI()
    {
        
    }


    void FixedUpdate()
    {
        
    }
	
	protected override void UpdateState()
    {
     
    }
    
    protected override void ExitState()
    {
   
    }

    protected override void SetSpriteTexture()
    {
        //SpriteManager.Instance.SetSpriteTexture(this.gameObject, GetTexture(), GetSpriteSettings());
    }

    void PopAction()
    {
        if (mIsDoingAction.Count > 0)
        {
            mIsDoingAction.Pop();

            if (mIsDoingAction.Count == 0)
            {
                SwitchState((int)State.Idle);
            }
        }
    }

    IEnumerator coRun()
    {
        float distance = 0.0f;

        if (mIsDoingAction.Contains(mStateID))
        {
            yield break;
        }

        //SoundManager.Instance.Play("Run");

        mIsDoingAction.Push(mStateID);

//        while (distance < (mDistanceToRun / 10.0f))
//        {
//            distance += (Time.deltaTime * mSpeedReference.mTranslationSpeed);
//            
//            yield return new WaitForEndOfFrame();
//        }

        BaseGame.IsEnvironmentMoving = false;
        PopAction();
    }

    IEnumerator coJump()
    {
        if (mIsDoingAction.Contains(mStateID))
        {
            yield break;
        }

        mIsDoingAction.Push(mStateID);

        //SoundManager.Instance.Play("jump");

        this.rigidbody.AddForce((Vector3.up * mJumpForce));
        
        while (!mIsOnGround)
        {
            yield return new WaitForEndOfFrame();
        }

        PopAction();
    }

    void OnAvoid(bool isAvoidCollision = true)
    {
        Physics.IgnoreLayerCollision(this.gameObject.layer, LayerMask.NameToLayer("Shuriken"), isAvoidCollision);
    }
}
