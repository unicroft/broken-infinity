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
    public float acceleration = 700;
	public float jumpHeight = 800;
	
    private float currentSpeed;
    private float targetSpeed;
	private Vector2 amountToMove;
	private PlayerPhysics playerPhysics;
	
	private bool falling = false;
	private bool jumpKeyReleased = true;
	
	private GameObject player;
	private Rigidbody rigidbody;
	private SpriteAnimator anim;
	
	private GameObject anima;
	private GameObject idle;
	private GameObject jump;
	
	void Awake(){
		player = gameObject;
		rigidbody = gameObject.GetComponent<Rigidbody>();
		playerPhysics = GetComponent<PlayerPhysics>();	
		anim = GetComponentInChildren<SpriteAnimator>();
		anim.RunOnce = false;
		
		anima = GameObject.Find("AnimatedCharacter");
		idle = GameObject.Find ("character-idle");
		jump = GameObject.Find ("character-jump");
		
	}

    private void setSprite(GameObject ob)
	{
		anima.GetComponent<MeshRenderer>().enabled = false;
		idle.GetComponent<MeshRenderer>().enabled = false;
		jump.GetComponent<MeshRenderer>().enabled = false;
		
		ob.GetComponent<MeshRenderer>().enabled = true;
	
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
	
	
	Vector3 forceJump;
	float jumpRemaining = 0;
	bool ground = false;

    void FixedUpdate()
    {
		jumpRemaining -= Time.fixedDeltaTime;
		targetSpeed = XCI.GetAxisRaw(XboxAxis.LeftStickX, joystick_id) * speed;
        currentSpeed = IncrementTowards(currentSpeed, targetSpeed, acceleration);
		
		if(rigidbody.velocity.magnitude > 5)
		{
			if(ground)
				setSprite(anima);
			anim.FramesPerSecond = Mathf.Sqrt(rigidbody.velocity.magnitude);
		}
		else
		{
			setSprite (idle);
		}
		
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
		Debug.DrawRay(castPos,-Vector3.up*40, Color.red);
		rigidbody.AddForce(gameObject.transform.right.normalized * axis * speed/10);
		if (Physics.Raycast (castPos, -Vector3.up,out hit) && hit.distance < 40) {
			
			
			transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
			
    		rigidbody.AddForce(gameObject.transform.right.normalized * axis * 1.5f * speed);
			

		}
		
		if((Input.GetKey(KeyCode.Space))||((XCI.GetButton(XboxButton.A, joystick_id))))
			{
				if(ground){
					forceJump = (axis*transform.right/5+transform.up).normalized * jumpHeight * 15;
					jumpRemaining = 1;
					rigidbody.AddForce(forceJump);
					jump.GetComponent<SpriteAnimator>().hasPlayed = false;
					setSprite(jump);
				}

				
			}

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
	
	void OnCollisionEnter(Collision collision) {
		
       Debug.Log ("Enter: " + collision.relativeVelocity.magnitude);
        if (collision.relativeVelocity.magnitude > 10 && collision.gameObject.name != "generatedobjective")
					{
           	MasterAudio.PlaySound("Land",transform,"land",true,0f);
					forceJump = new Vector3(0,0,0);
						jumpRemaining = 0;
			ground = true;
					}
        
    }
	
	void OnCollisionExit(Collision collision)
    {
		Debug.Log ("Exit: " + collision.relativeVelocity.magnitude );
		if (collision.relativeVelocity.magnitude > 10 && collision.gameObject.name != "generatedobjective"){
           	MasterAudio.PlaySound("Jump",transform,"jump",true,0f);
			ground = false;	
		}
    }
	
	void OnCollisionStay(Collision collision)
    {
		/*
		Debug.Log (collision.relativeVelocity.magnitude);
		if (collision.relativeVelocity.magnitude > 10 && collision.gameObject.name != "generatedobjective")
           	MasterAudio.PlaySound("Walk",transform,"Walk",true,0f);
        //*/
    }
}
