using UnityEngine;
using XboxCtrlrInput;
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
    }

//    public BackgroundTranslate mSpeedReference = null;
    public List<TextureSettings> mSpriteList = new List<TextureSettings>();
    
    public AudioSource mJumpSound = null;
    
    // Stack<int> mIsDoingAction = new Stack<int>();
    // bool mIsOnGround = false;

    // player handling
	public int joystick_id = 1;
	public float gravity = 500;
    public float speed = 150;
    public float acceleration = 500;
	public float jumpHeight = 300;
	
    private float currentSpeed;
    private float targetSpeed;
	private Vector2 amountToMove;
	private PlayerPhysics playerPhysics;
	
	private bool falling = false;
	private bool jumpKeyReleased = true;
	
	private GameObject player;
	private Rigidbody rigidbody;
	private SpriteAnimator anim;
	
	void Awake(){
		player = gameObject;
		rigidbody = gameObject.GetComponent<Rigidbody>();
		playerPhysics = GetComponent<PlayerPhysics>();	
		anim = GetComponentInChildren<SpriteAnimator>();
		
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
		targetSpeed = XCI.GetAxisRaw(XboxAxis.LeftStickX, joystick_id) * speed;
        currentSpeed = IncrementTowards(currentSpeed, targetSpeed, acceleration);
		
		if(rigidbody.velocity.magnitude > 10)
			anim.mFramesPerSecond = rigidbody.velocity.magnitude/5;
		else
			anim.mFramesPerSecond = 0.01f;
		
		float axis = 0;
		if(Input.GetKey(KeyCode.RightArrow))
		{
			axis = 1.0f;
		}
		else if (Input.GetKey(KeyCode.LeftArrow))
		{
			axis = -1.0f;
		}
		axis = XCI.GetAxisRaw(XboxAxis.LeftStickX, joystick_id);

		RaycastHit hit = new RaycastHit();

		var castPos = new Vector3(transform.position.x,transform.position.y-0.25f,transform.position.z);
		
		if (Physics.Raycast (castPos, -Vector3.up,out hit) && hit.distance < 55) {
			
			
			transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
			
    		rigidbody.AddForce(gameObject.transform.right.normalized * axis * speed);
			//if(XCI.GetButton(XboxButton.A, joystick_id))
			if((Input.GetKey(KeyCode.Space))||((XCI.GetButton(XboxButton.A, joystick_id))))
			{
				rigidbody.AddForce((axis*transform.right/3+transform.up).normalized * jumpHeight);
			}
    		rigidbody.AddForce(gameObject.transform.right.normalized * axis * speed);
			

		}
		else
		{
			//transform.rotation = Quaternion.FromToRotation (Vector3.up, Vector3.up);
		}
		
		if (playerPhysics.grounded) {
			if (!XCI.GetButton(XboxButton.A, joystick_id)) {
				jumpKeyReleased = true;
			}
			
			amountToMove.y = 0;
			if (XCI.GetButton(XboxButton.A, joystick_id) && jumpKeyReleased) {
				amountToMove.y = jumpHeight;
				falling = false;
				jumpKeyReleased = false;
			}
		} // en l'air
		else {
			if (!falling) {
				if (!XCI.GetButton(XboxButton.A, joystick_id)) {
					falling = true;
					amountToMove.y = 0;
				}
			}	
		}
		
		amountToMove.x = currentSpeed;
		amountToMove.y -= gravity * Time.deltaTime;
		if (amountToMove.y < 0) { falling = true; }
		//playerPhysics.Move(amountToMove * Time.deltaTime);
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
}
