using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
// [RequireComponent(typeof(Animator))]
public class Player3rdPersonMovement : StateController
{

    [SerializeField] AgentAbility _ability;
	
    private Camera _cam;            //Main Camera 
    private CharacterController _characterController;           //Character Controller, aotu added on script addition
    private CharacterInfo _characterInfo;           //Character Controller, aotu added on script addition
    public Animator _anim;           //Animator
    [SerializeField] private Text _actionText;
    [SerializeField] Slider _speedSlider;
    
    [SerializeField] float _walkSpeed = 2f;
    [SerializeField] float _runSpeed = 6f;
    [SerializeField] float  _jumpHeight = 1.0f;         //Max Jump height
    // [SerializeField] float _jumpSpeed = 1f;
    [Range(0,1)]
	[SerializeField] float _airControlPercent;
    [SerializeField] float _turnSmoothTime = 0.2f;
	[SerializeField] float _speedSmoothTime = 0.1f;
    private Vector2 _input;
    private Vector2  _inputDir;
    private float  _verticalVelocity;         //Gravity applied to player 
    private float  _gravity = -9.8f;         //Gravity Constant
	float _turnSmoothVelocity;
	float _speedSmoothVelocity;
	float _currentSpeed;

	[SerializeField] private Item _GrowthPortion;
	[SerializeField] private Item _Food;
	[SerializeField] private Item _Souls;
	[SerializeField] private ItemFloat _PlayerTime;

    [Header("Player Controles")]

    [SerializeField] private KeyCode _JumpKey = KeyCode.Space;
    // [SerializeField] private KeyCode _ClimbKey = KeyCode.C;
    // [SerializeField] private KeyCode _SwingKey = KeyCode.X;
    // [SerializeField] private KeyCode _SlideKey = KeyCode.Z;

    [SerializeField] private KeyCode _CrouchKey = KeyCode.LeftAlt;
    [SerializeField] private KeyCode _RunKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode _GlideKey = KeyCode.LeftControl;

    [SerializeField] private KeyCode _EatFoodKey = KeyCode.Q;
    [SerializeField] private KeyCode _GrowTreesKey = KeyCode.E;
    [SerializeField] private KeyCode _MeeleAttackKey = KeyCode.Mouse0;
	
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
        _characterInfo = GetComponent<CharacterInfo>();
        _cam = Camera.main;
		_characterInfo.currentHealth -= 30;
		_Food.Value = 100;
		_Souls.Value = 10;
		_GrowthPortion.Value = 10;
		_PlayerTime.Amount = 0;
        
    }
    // Update is called once per frame
    void Update()
    {
		// if (aiActive)
        //     return;
        // currentState.UpdateState (this);
		
        // input
		_input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		_inputDir = _input.normalized;
		bool running = Input.GetKey (KeyCode.LeftShift);
		bool crouching = Input.GetKey (_CrouchKey);
        if(_anim != null)_anim.SetBool("Crouch", crouching);

		if (Input.GetKeyDown (_JumpKey)) {
			Jump();
            if(_anim != null)_anim.SetTrigger("Jump");
		}
        else if (crouching) {
			Move (_inputDir, !crouching);
			// _currentSpeed = _walkSpeed;
		}
        // else if (Input.GetKeyDown (_ClimbKey)) {
		// 	Climb();
        //     if(_anim != null)_anim.SetTrigger("Climb");
		// }
        // else if (Input.GetKeyDown (_SwingKey)) {
		// 	Swing();
        //     if(_anim != null)_anim.SetTrigger("Swing");
		// }
        else if (Input.GetKeyDown (_GlideKey)) {
			Glide();
            if(_anim != null)_anim.SetTrigger("Glide");
		}
        // else if (Input.GetKeyDown (_SlideKey)) {
		// 	Slide();
        //     if(_anim != null)_anim.SetTrigger("Slide");
		// }
        else {
			Move (_inputDir, running);
		}
        
		if (Input.GetKeyDown (_GrowTreesKey)) {
			GrowTrees();
            if(_anim != null)_anim.SetTrigger("GrowTrees");
		}
        else if (Input.GetKeyDown (_EatFoodKey)) {
			EatFood();
            if(_anim != null)_anim.SetTrigger("EatFood");
		}
		else if (Input.GetKeyDown (_MeeleAttackKey)) {
			MeeleAttack();
            if(_anim != null)_anim.SetTrigger("MeleeAttack");
		}
		// animator
		float animationSpeedPercent = ((running) ? _currentSpeed / _runSpeed : _currentSpeed / _walkSpeed * .5f);
		if(_anim != null)_anim.SetFloat ("Speed", animationSpeedPercent, _speedSmoothTime, Time.deltaTime);

		// Debug.Log("_currentSpeed: " + _currentSpeed);
		// Debug.Log("animationSpeedPercent: " + animationSpeedPercent);

    }

    #region PlayerMovement

    void Move(Vector2 inputDir, bool running) {
		if (inputDir != Vector2.zero) {
			float targetRotation = Mathf.Atan2 (inputDir.x, inputDir.y) * Mathf.Rad2Deg + _cam.transform.eulerAngles.y;
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref _turnSmoothVelocity, GetModifiedSmoothTime(_turnSmoothTime));
		}
			
		float targetSpeed = ((running) ? _runSpeed : _walkSpeed) * inputDir.magnitude;
		_currentSpeed = Mathf.SmoothDamp (_currentSpeed, targetSpeed, ref _speedSmoothVelocity, GetModifiedSmoothTime(_speedSmoothTime));
		_speedSlider.value = _currentSpeed;

		_verticalVelocity += Time.deltaTime * _gravity;
		Vector3 velocity = transform.forward * _currentSpeed + Vector3.up * _verticalVelocity;

		_characterController.Move (velocity * Time.deltaTime);
		_currentSpeed = new Vector2 (_characterController.velocity.x, _characterController.velocity.z).magnitude;

		if (_characterController.isGrounded) {
			_verticalVelocity = 0;
		}
	}

    void Jump() {
		if (_characterController.isGrounded) {
			float jumpVelocity = Mathf.Sqrt (-2 * _gravity * _jumpHeight);
			_verticalVelocity = jumpVelocity;
		}
	}

    void Climb() {
		// if (_characterController.isGrounded) {
			float jumpVelocity = Mathf.Sqrt (-2 * _gravity * _jumpHeight);
			_verticalVelocity = jumpVelocity;
		// }
	}

    void Swing() {
		// if (_characterController.isGrounded) {
			float jumpVelocity = Mathf.Sqrt (-2 * _gravity * _jumpHeight);
			_verticalVelocity = jumpVelocity * 2;
		// }
	}
    
    void Glide() {
		// if (_characterController.isGrounded) {
			float jumpVelocity = Mathf.Sqrt (-2 * _gravity * _jumpHeight);
			_verticalVelocity = jumpVelocity * 2;
		// }
	}
    
    void Slide() {
		// if (_characterController.isGrounded) {
			float jumpVelocity = Mathf.Sqrt (-2 * _gravity * _jumpHeight);
			_verticalVelocity = jumpVelocity;
		// }
	}

    void MeeleAttack() {
		// _characterInfo.TakeDamage();
	}

    void EatFood() {
		Debug.Log("Eat Food");
		if(_Food.Value > 0){
			if(_characterInfo.currentHealth < _characterInfo.AgentSettings.startingHealth - 10)_characterInfo.currentHealth += 10;
			else
			{
				_characterInfo.currentHealth = _characterInfo.AgentSettings.startingHealth;
			}
			_Food.Value--;
		}
	}

    void GrowTrees() {
		Debug.Log("Grow Trees");
		// Souls.Value--;
		if(_GrowthPortion.Value > 0)
		{
			_GrowthPortion.Value--;
			if(_PlayerTime.Amount > 0)
				_PlayerTime.Amount -= 10;
			else
				_PlayerTime.Amount = 0;

		}
	}

    float GetModifiedSmoothTime(float smoothTime) {
		if (_characterController.isGrounded) {
			return smoothTime;
		}

		if (_airControlPercent == 0) {
			return float.MaxValue;
		}
		return smoothTime / _airControlPercent;
	}

    #endregion

}
