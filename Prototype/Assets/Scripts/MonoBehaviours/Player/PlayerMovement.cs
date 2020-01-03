using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterInfo))]
public class PlayerMovement : StateController
{
    [Header("Player Ability")]
    [SerializeField] private Item _GrowthPortion;
	[SerializeField] private Item _Food;
	[SerializeField] private Item _Souls;
	[SerializeField] private ItemFloat _PlayerTime;
    [SerializeField] List<AgentAbility> _abilities;
    
    [Header("Player Controles")]
    [SerializeField] private KeyCode _JumpKey = KeyCode.Space;
    [SerializeField] private KeyCode _CrouchKey = KeyCode.LeftAlt;
    [SerializeField] private KeyCode _RunKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode _GlideKey = KeyCode.Mouse1;

    [SerializeField] private KeyCode _EatFoodKey = KeyCode.Q;
    [SerializeField] private KeyCode _GrowTreesKey = KeyCode.E;
    [SerializeField] private KeyCode _MeeleAttackKey = KeyCode.Mouse0;

    public Animator playerAnim;                                              // Reference to the animator component.
    private Camera _cam;            //Main Camera 
    private CharacterController _characterController;           //Character Controller, aotu added on script addition
    private CharacterInfo _characterInfo;           //Character Controller, aotu added on script addition
    private Rigidbody playerRigidbody;                                  // Reference to the player's rigidbody.
    private UnityEngine.AI.NavMeshAgent navAgent;                               // Reference to the nav mesh agent.
    
    [SerializeField] float  _camSensitivity = 100f;
    Vector2 _inputCam;
    Vector2  _inputMovement;
    float  _verticalVelocity;         //Gravity applied to player 
    float  _gravity = -9.8f;         //Gravity Constant
	float _turnSmoothVelocity;
	float _speedSmoothVelocity;
	float _currentSpeed;
    
    // How fast the tank turns in degrees per second.
    [Tooltip("Movment Audio Source")]
    [SerializeField] AudioSource movementAudioSource;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
    [Tooltip("Idle Audio clip")]
    [SerializeField] AudioClip playerIdling;            // Audio to play when the tank isn't moving.
    [Tooltip("Driving Audio clip")]
    [SerializeField] AudioClip playerDriving;           // Audio to play when the tank is moving.
    [Tooltip("Engin Audio Pitch Range")]
    [SerializeField] 
    private float pitchRange = 0.2f;            // The amount by which the pitch of the engine noises can vary.
    private float originalPitch;              // The pitch of the audio source at the start of the scene.
 
    void Awake ()
    {
        // Set up references.
        if(!playerAnim)playerAnim = GetComponent <Animator> ();
        // if(!playerRigidbody)playerRigidbody = GetComponent <Rigidbody> ();
        // if(!navAgent)navAgent = GetComponent <UnityEngine.AI.NavMeshAgent> ();
        if(!_characterController)_characterController = GetComponent <CharacterController> ();
        if(!_characterInfo)_characterInfo = GetComponent <CharacterInfo> ();
        _cam = Camera.main;
    }

    private void Start()
    {
        // Store the original pitch of the audio source.
        originalPitch = movementAudioSource.pitch;
        //TODO reset player values at start of the game.
		_Food.Value = 100;
		_Souls.Value = 10;
		_GrowthPortion.Value = 10;
		_PlayerTime.Amount = 0;
    }

    void FixedUpdate ()
    {
        // Move the player around the scene.
        bool IsRunning = Input.GetKey (_RunKey);
		bool IsCrouching = Input.GetKey (_CrouchKey);
		bool IsFire = Input.GetKey(_MeeleAttackKey);
		bool IsGlide = Input.GetKey(_GlideKey);
        if(playerAnim != null)playerAnim.SetBool("Crouch", IsCrouching);

		string Action = "Move";

		if (Input.GetKeyDown(_JumpKey)) {
			StartCoroutine(Jump(_inputMovement, IsRunning));
            Action = "Jump";
		}
        else if (IsGlide) {
			StartCoroutine(Glide(_inputMovement, IsFire));
            if(!_characterController.isGrounded)Action = "Glide";
            else Action = "Move";
		}
        else if (IsCrouching) {
            StartCoroutine(Crouch(_inputMovement, !IsCrouching));
            Action = "Crouch";
            // _currentSpeed = _walkSpeed;
        }
        else {
            StartCoroutine(Move (_inputMovement, IsRunning));
            Action = "Move";
        }

        if (Input.GetKeyDown (_GrowTreesKey)) {
			StartCoroutine(GrowTrees());
            Action = "GrowTrees";
		}
        else if (Input.GetKeyDown (_EatFoodKey)) {
			StartCoroutine(EatFood());
            Action = "EatFood";
		}
        
		if(!IsGlide)
        {
			StartCoroutine(DeactivateGlide(_inputMovement, IsFire));
            StartCoroutine(MeeleAttack(_inputMovement, IsFire));
            if(IsFire)Action = "MeeleAttack";		
        }
        
        // Animate the player.
        Animating(IsRunning, Action);
        MovementAudio();
    }

    private void Update ()
    {
        // input
		var _input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		_inputCam = new Vector2 (Input.GetAxisRaw ("Mouse X"), Input.GetAxisRaw ("Mouse Y")) * _camSensitivity * Time.deltaTime;
		_inputMovement = _input.normalized;

    }

    void Animating (bool IsRunning, string Action)
    {
        // animator
		float animationSpeedPercent = ((IsRunning) ? _currentSpeed / _characterInfo.AgentSettings.runSpeed : _currentSpeed / _characterInfo.AgentSettings.walkSpeed * .5f);
		if(playerAnim != null)playerAnim.SetFloat ("Speed", animationSpeedPercent, _characterInfo.AgentSettings.speedSmoothTime, Time.deltaTime);

        if(playerAnim != null)playerAnim.SetTrigger(Action);

		// Debug.Log("_currentSpeed: " + _currentSpeed);
		// Debug.Log("animationSpeedPercent: " + animationSpeedPercent);
    }

    private void MovementAudio ()
    {
        // If there is no input (the tank is stationary)...
        // if (Mathf.Abs (moveHorizontal) < 0.1f && Mathf.Abs (moveVertical) < 0.1f)
        if (Mathf.Abs (_currentSpeed) < 0.1f)
        {
            // Debug.Log("Engine Noise");
            // ... and if the audio source is currently playing the driving clip...
            if (movementAudioSource.clip == playerDriving)
            {
                // ... change the clip to idling and play it.
                movementAudioSource.clip = playerIdling;
                movementAudioSource.pitch = Random.Range (originalPitch - pitchRange, originalPitch + pitchRange);
                movementAudioSource.Play ();
            }
        }
        else
        {
            // Otherwise if the tank is moving and if the idling clip is currently playing...
            if (movementAudioSource.clip == playerIdling)
            {
                // ... change the clip to driving and play.
                movementAudioSource.clip = playerDriving;
                movementAudioSource.pitch = Random.Range(originalPitch - pitchRange, originalPitch + pitchRange);
                movementAudioSource.Play();
            }
        }
    }

#region PlayerMovement

    IEnumerator Move(Vector2 inputDir, bool IsRunning) {
		if (inputDir != Vector2.zero) {
			float targetRotation = Mathf.Atan2 (inputDir.x, inputDir.y) * Mathf.Rad2Deg + _cam.transform.eulerAngles.y;
			_characterController.transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref _turnSmoothVelocity, GetModifiedSmoothTime(_characterInfo.AgentSettings.turnSmoothTime));
		}
			
		float targetSpeed = ((IsRunning) ? _characterInfo.AgentSettings.runSpeed : _characterInfo.AgentSettings.walkSpeed) * inputDir.magnitude;
		_currentSpeed = Mathf.SmoothDamp (_currentSpeed, targetSpeed, ref _speedSmoothVelocity, GetModifiedSmoothTime(_characterInfo.AgentSettings.speedSmoothTime));
		
		_verticalVelocity += Time.deltaTime * _gravity;
		if (_characterController.isGrounded) {
			_verticalVelocity = -1;
		}

		Vector3 velocity = Vector3.up * _verticalVelocity;
        velocity += transform.forward * _currentSpeed;

		_characterController.Move (velocity * Time.deltaTime);
		// _currentSpeed = new Vector2 (_characterController.velocity.x, _characterController.velocity.z).magnitude;

        //Wait for .7 seconds
        yield return new WaitForSeconds(0.7f);

	}

    IEnumerator Jump(Vector2 inputDir, bool IsRunning) {
		if (inputDir != Vector2.zero) {
			float targetRotation = Mathf.Atan2 (inputDir.x, inputDir.y) * Mathf.Rad2Deg + _cam.transform.eulerAngles.y;
			_characterController.transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref _turnSmoothVelocity, GetModifiedSmoothTime(_characterInfo.AgentSettings.turnSmoothTime));
		}
			
		float targetSpeed = ((IsRunning) ? _characterInfo.AgentSettings.runSpeed : _characterInfo.AgentSettings.walkSpeed) * inputDir.magnitude;
		_currentSpeed = Mathf.SmoothDamp (_currentSpeed, targetSpeed, ref _speedSmoothVelocity, GetModifiedSmoothTime(_characterInfo.AgentSettings.speedSmoothTime));
		
		Vector3 velocity = new Vector3(0,0,0);
        float jumpVelocity = Mathf.Sqrt (-2 * _gravity * _characterInfo.AgentSettings.jumpHeight);

		// jump if on the ground
        if (_characterController.isGrounded) {
		    _verticalVelocity =  jumpVelocity;
            velocity = Vector3.up * _verticalVelocity;
		}

        velocity += transform.forward * _currentSpeed;

        Debug.Log("Jumping");

		_characterController.Move (velocity * Time.deltaTime);

        //Wait for .7 seconds
        yield return new WaitForSeconds(0.7f);

	}

    IEnumerator Crouch(Vector2 inputDir, bool IsCrouching) {
		if (inputDir != Vector2.zero) {
			float targetRotation = Mathf.Atan2 (inputDir.x, inputDir.y) * Mathf.Rad2Deg + _cam.transform.eulerAngles.y;
			_characterController.transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref _turnSmoothVelocity, GetModifiedSmoothTime(_characterInfo.AgentSettings.turnSmoothTime));
		}
			
		float targetSpeed = ((IsCrouching) ? _characterInfo.AgentSettings.crouchSpeed : _characterInfo.AgentSettings.walkSpeed) * inputDir.magnitude;
		_currentSpeed = Mathf.SmoothDamp (_currentSpeed, targetSpeed, ref _speedSmoothVelocity, GetModifiedSmoothTime(_characterInfo.AgentSettings.speedSmoothTime));
		
		_verticalVelocity += Time.deltaTime * _gravity;
		if (_characterController.isGrounded) {
			_verticalVelocity = -1;
		}

		Vector3 velocity = Vector3.up * _verticalVelocity;
        velocity += transform.forward * _currentSpeed;

		_characterController.Move (velocity * Time.deltaTime);

        //Wait for .7 seconds
        yield return new WaitForSeconds(0.7f);
	}
    
    IEnumerator Glide(Vector2 inputDir, bool IsGrapple) {
        foreach(AgentAbility ability in _abilities)
        {
            if(ability.Name == "GrappleTargetAbility")
            {
                
                // Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
                GrappleTargetAbility grap = (GrappleTargetAbility)ability;
                grap.ViewHooks();
                
                if(IsGrapple)
                {
                    grap.IsGrapple = true;
                    grap.PlayAbility();
                }

                // //Wait for .07 seconds
                // yield return new WaitForSeconds(0.7f);

                float targetSpeed = 0.0f;//inputDir.magnitude;
                if(grap.IsGrapple && grap.GetHook() != null)
                {
                    targetSpeed = _characterInfo.AgentSettings.swingSpeed * inputDir.magnitude;
                    // move towards grappling target
                    _currentSpeed = Mathf.SmoothDamp (_currentSpeed, targetSpeed, ref _speedSmoothVelocity, GetModifiedSmoothTime(_characterInfo.AgentSettings.speedSmoothTime));
                    transform.position = Vector3.MoveTowards(transform.position, grap.GetHook().position, _currentSpeed * Time.deltaTime);
                    if(Vector3.Distance(transform.position, grap.GetHook().position) <= 0.01)
                    {
                        grap.IsGrapple = false;
                    }
                    // else
                    // {
                        // transform.position = Vector3.MoveTowards(transform.position, grap.ProjectileLaunch(), _currentSpeed * Time.deltaTime);
                        // _characterController.Move(grap.ProjectileLaunch());
                    // }
                }
                else 
                if(!_characterController.isGrounded)    
                {
                    if(grap.IsGrapple)
                    {
                        // try and see if there is a hook above
                        if(grap.GetHook() == null)
                        {
                            grap.FindElivatedHooks();
                        }
                        if(grap.GetHook())
                        {
                            targetSpeed = _characterInfo.AgentSettings.swingSpeed * inputDir.magnitude;
                            // move towards grappling target
                            _currentSpeed = Mathf.SmoothDamp (_currentSpeed, targetSpeed, ref _speedSmoothVelocity, GetModifiedSmoothTime(_characterInfo.AgentSettings.speedSmoothTime));
                            transform.position = Vector3.MoveTowards(transform.position, grap.GetHook().position, _currentSpeed * Time.deltaTime);
                            if(Vector3.Distance(transform.position, grap.GetHook().position) <= 0.01)
                                grap.IsGrapple = false;

                        }
                    }
                    else
                    {
                        // glide till you reach the ground
                        if (inputDir != Vector2.zero) {
                            float targetRotation = Mathf.Atan2 (inputDir.x, inputDir.y) * Mathf.Rad2Deg + _cam.transform.eulerAngles.y;
                            targetRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref _turnSmoothVelocity, GetModifiedSmoothTime(_characterInfo.AgentSettings.turnSmoothTime));
                            _characterController.transform.eulerAngles = Vector3.up * targetRotation;
                        }
                            
                        targetSpeed = _characterInfo.AgentSettings.glideSpeed * inputDir.magnitude;
                        _currentSpeed = Mathf.SmoothDamp (_currentSpeed, targetSpeed, ref _speedSmoothVelocity, GetModifiedSmoothTime(_characterInfo.AgentSettings.speedSmoothTime));
                        
                        // _verticalVelocity += Time.deltaTime * _gravity; 
                        _verticalVelocity = -10 * _characterInfo.AgentSettings.airControlPercent;

                        Vector3 velocity = Vector3.up * _verticalVelocity;
                        velocity += transform.right * inputDir.x;
                        velocity += transform.forward * inputDir.y;

                        _characterController.Move(velocity * Time.deltaTime * _currentSpeed);

                    }


                }
                
                //Wait for .07 seconds
                yield return new WaitForSeconds(0.07f);

            }
        }

	}

    IEnumerator DeactivateGlide(Vector2 inputDir, bool IsGrapple) {
        foreach(AgentAbility ability in _abilities)
        {
            if(ability.Name == "GrappleTargetAbility")
            {
                // Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
                GrappleTargetAbility grap = (GrappleTargetAbility)ability;
                grap.DeactivateGrappleHook();
                //Wait for .07 seconds
                yield return new WaitForSeconds(0.07f);
            }
        }

	}

    IEnumerator MeeleAttack(Vector2 inputDir, bool IsFire) {
		foreach(AgentAbility ability in _abilities)
        {
            if(ability.Name == "RayCastTargetAbility")
            {
                if(IsFire)ability.PlayAbility();

                //Wait for .7 seconds
                yield return new WaitForSeconds(0.7f);
            }
        }
	}

    IEnumerator EatFood() {
		Debug.Log("Eat Food");
		if(_Food.Value > 0){
			if(_characterInfo.currentHealth < _characterInfo.AgentSettings.startingHealth - 10)_characterInfo.currentHealth += 10;
			else
			{
				_characterInfo.currentHealth = _characterInfo.AgentSettings.startingHealth;
			}
			_Food.Value--;
            //Wait for .7 seconds
            yield return new WaitForSeconds(0.7f);
		}
	}

    IEnumerator GrowTrees() {
		Debug.Log("Grow Trees");
		// Souls.Value--;
		if(_GrowthPortion.Value > 0)
		{
			_GrowthPortion.Value--;
			if(_PlayerTime.Amount > 0)
				_PlayerTime.Amount -= 10;
			else
				_PlayerTime.Amount = 0;

            //Wait for .7 seconds
            yield return new WaitForSeconds(0.7f);

		}
	}

    float GetModifiedSmoothTime(float smoothTime) {
		if (_characterController.isGrounded) {
			return smoothTime;
		}

		if (_characterInfo.AgentSettings.airControlPercent == 0) {
			return float.MaxValue;
		}
		return smoothTime / _characterInfo.AgentSettings.airControlPercent;
	}

#endregion

}