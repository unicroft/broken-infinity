using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerPhysics))]

public class PlayerController : BaseGame
{
    public enum State
    {
        Idle, 
        Running, 
        Jumping, 
    }

    [System.Serializable]
    public class TextureSettings
    {
        public State mState = State.Idle;
        public Texture mSpriteSheet = null;

        public SpriteSettings mSpriteSettings = new SpriteSettings();
    }

//    public BackgroundTranslate mSpeedReference = null;
    public List<TextureSettings> mSpriteList = new List<TextureSettings>();
    
    public AudioSource mJumpSound = null;
    
    // Stack<int> mIsDoingAction = new Stack<int>();
    // bool mIsOnGround = false;

    // player handling
	public float gravity = 20;
    public float speed = 62;
    public float acceleration = 84;
	public float jumpHeight = 62;
	
    private float currentSpeed;
    private float targetSpeed;
	private Vector2 amountToMove;
	private PlayerPhysics playerPhysics;
	
	void Start(){
		playerPhysics = GetComponent<PlayerPhysics>();	
	}
	
    void OnCollisionEnter(Collision collision)
    {
//        BackgroundTranslate ground = collision.gameObject.GetComponent<BackgroundTranslate>();
        
        //if (ground != null)
        //{
            // mIsOnGround = true;
        //}
    }

    void OnCollisionExit(Collision collision)
    {
  //      BackgroundTranslate ground = collision.gameObject.GetComponent<BackgroundTranslate>();

        //if (ground != null)
        //{
            // mIsOnGround = false;
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
                    // BaseGame.IsEnvironmentMoving = true;

                    // StartCoroutine(coRun());

                }
                break;

            case State.Jumping:
                {

          //           StartCoroutine(coJump());

          //           if (mJumpSound != null)
          //           {
          // //              SoundManager.Instance.Play(mJumpSound);
          //           }
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
        targetSpeed = Input.GetAxisRaw("Horizontal") * speed;
        currentSpeed = IncrementTowards(currentSpeed, targetSpeed, acceleration);
		
		if (playerPhysics.grounded) {
			amountToMove.y = 0;
			if (Input.GetButtonDown("Jump")) {
				amountToMove.y = jumpHeight;
			}
		}
		
		amountToMove.x = currentSpeed;
		amountToMove.y -= gravity * Time.deltaTime;
		playerPhysics.Move(amountToMove * Time.deltaTime);
    }

    private float IncrementTowards(float n, float target, float a) {
        if (n == target) {
            return n;
        }
        else {
            float dir = Mathf.Sign(target - n);
			n += a * Time.deltaTime * dir;
			return (dir == Mathf.Sign(target - n))? n:target;
        }
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

    Texture GetTexture()
    {
        foreach (TextureSettings state in mSpriteList)
        {
            if (state.mState == (State)mStateID)
            {
                return state.mSpriteSheet;
            }
        }

        return null;
    }

    SpriteSettings GetSpriteSettings()
    {
        foreach (TextureSettings state in mSpriteList)
        {
            if (state.mState == (State)mStateID)
            {
                return state.mSpriteSettings;
            }
        }

        return new SpriteSettings();
    }
}
