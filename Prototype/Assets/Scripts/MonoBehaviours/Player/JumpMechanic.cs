using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpMechanic : MonoBehaviour
{

    // [SerializeField] AgentStates playerStates;           // The Items that are carried by the player.
    
    [Header("Controler Key Setup")]
    [SerializeField] private KeyCode _JumpKey = KeyCode.Space;
    // [SerializeField] private KeyCode _SwingKey = KeyCode.LeftShift;

    private float _potentialEnergy;
    [Range(1f, 100f)] [SerializeField] float _minLaunchForce = 2f;        // The force given to the shell if the fire button is not held.
    [Range(1f, 100f)] [SerializeField] float _maxLaunchForce = 5f;        // The force given to the shell if the fire button is held for the max charge time.
    private float _maxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.
    private float _currentLaunchForce;         // The force that will be given to the shell when the fire button is released.
    private float _chargeSpeed;                // How fast the launch force increases, based on the max charge time.
    private bool _launch;                       // Whether or not the shell has been launched with this button press.
    private float  _verticalVelocity;         //Gravity applied to player 

    // Start is called before the first frame update
    void Start()
    {
        // When the tank is turned on, reset the launch force and the UI
        _currentLaunchForce = _minLaunchForce;
        _potentialEnergy = _minLaunchForce;
        
    }

    // Update is called once per frame
    void Update()
    {
        JumpManager();
    }


    private void JumpManager()
    {
        // The slider should have a default value of the minimum launch force.
        _potentialEnergy = _minLaunchForce;
        // The rate that the launch force charges up is the range of possible forces by the max charge time.
        _chargeSpeed = (_maxLaunchForce - _minLaunchForce) / _maxChargeTime;

        // If the max force has been exceeded and the shell hasn't yet been launched...
        if (_currentLaunchForce >= _maxLaunchForce && !_launch)
        {
            // ... use the max force and launch the shell.
            _currentLaunchForce = _maxLaunchForce;
            
            _launch = true;
            // playerStates.movementStateMaching = AgentStates.movementState.Idle;
            _verticalVelocity = _currentLaunchForce;
        }
        // Otherwise, if the fire button has just started being pressed...
        else if (Input.GetKeyDown(_JumpKey))
        {
            // ... reset the fired flag and reset the launch force.
            _launch = false;
            _currentLaunchForce = _minLaunchForce;
            // playerStates.movementStateMaching = AgentStates.movementState.Jump;

        }
        // Otherwise, if the fire button is being held and the shell hasn't been launched yet...
        else if (Input.GetKey(_JumpKey) && !_launch)
        {
            // Increment the launch force and update the slider.
            _currentLaunchForce += _chargeSpeed * Time.deltaTime;

            _potentialEnergy = _currentLaunchForce;
            // playerStates.movementStateMaching = AgentStates.movementState.Jump;

        }
        // Otherwise, if the fire button is released and the shell hasn't been launched yet...
        else if (Input.GetKeyUp(_JumpKey) && !_launch)
        {
            // ... launch the shell.
            _launch = true;
            // playerStates.movementStateMaching = AgentStates.movementState.Idle;
            _verticalVelocity = _currentLaunchForce;

        }

        // Debug.Log("State:" + playerStates.movementStateMaching);

    }

}
