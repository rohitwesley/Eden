using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingMechanic : MonoBehaviour
{
    [SerializeField] LineRenderer rope;
    [SerializeField] GameObject hook;
    [SerializeField] GameObject hookHolder;
    [SerializeField] CharacterController _characterController;           //Character Controller, aotu added on script addition
    [SerializeField] float hookTravelSpeed;
    [SerializeField] float playerTravelSpeed;
    bool fired;
    public bool hooked;
    public GameObject hookedObj;
    [SerializeField] float maxDistance;
    float currentDistance;
    private bool grounded;
    [Header("Controler Key Setup")]
    [SerializeField] private KeyCode _SwingKey = KeyCode.Space;
    
    float speed = 5;
    // [SerializeField] private AgentStates state;

    private void Update()
    {
        // Fire hook
        if(Input.GetKey(_SwingKey) && fired == false)
        {
            fired = true;
        }

        // move line
        if(fired)
        {
            // LineRenderer rope = GetComponent<LineRenderer>();
            rope.positionCount = 2;
            rope.SetPosition(0, hookHolder.transform.position);
            rope.SetPosition(1, hook.transform.position);

            hook.GetComponent<Collider>().enabled = true;
        }

        // move hook
        if(fired == true && hooked == false)
        {
            hook.transform.Translate(Vector3.forward * Time.deltaTime * hookTravelSpeed);
            currentDistance = Vector3.Distance(_characterController.transform.position, hook.transform.position);

            if(currentDistance >= maxDistance)
            ReturnHook();
        }

        // move player
        if(hooked == true && fired == true)
        {
            Debug.Log("isHooked " + hookedObj.transform.position);
            // hook.transform.parent = hookedObj.transform;
            // transform.Translate(Vector3.forward * Time.deltaTime * hookTravelSpeed);
            Vector3 moveDirection = hookedObj.transform.position;//new Vector3(24.5f,6.5f, 6.0f);
            moveDirection = new Vector3(moveDirection.x, hookedObj.transform.position.y + 1f, moveDirection.z);
            _characterController.Move(moveDirection);

            // Vector3 displacmentFromTarget = hookedObj.transform.position - _characterController.transform.position;
            // Vector3 directionToTarget = displacmentFromTarget.normalized;
            // Vector3 velocity = directionToTarget * speed;

            // float distanceToTarget = displacmentFromTarget.magnitude;

            // if(distanceToTarget > 1.5f)
            // {
            //     _characterController.Move(velocity * Time.deltaTime);
            //     _characterController.Move(new Vector3(0.0f, Mathf.Sin(Time.deltaTime) * (hookedObj.transform.position.y + 1f), 0.0f));
            //     // moveDirection = new Vector3(_characterController.transform.position.x, hookedObj.transform.position.y + 10f, _characterController.transform.position.z);
            //     // _characterController.Move(moveDirection);
            // }
            // StartCoroutine(teleport(moveDirection));
            
            // if(distanceToTarget <= 0.1 || distanceToTarget >= maxDistance)
            // ReturnHook();

        }

    }

    IEnumerator teleport(Vector3 moveDirection)
    {
        _characterController.enabled = false;
        yield return new WaitForSeconds(0.1f);
        _characterController.transform.position = moveDirection;//hookedObj.transform.position;//.Move(moveDirection);
        
        // Vector3 displacmentFromTarget = hookedObj.transform.position - transform.position;
        // Vector3 directionToTarget = displacmentFromTarget.normalized;
        // Vector3 velocity = directionToTarget * speed;

        // float distanceToTarget = displacmentFromTarget.magnitude;

        // if(distanceToTarget > 1.5f)
        // {
        //     _characterController.transform.Translate(velocity * Time.deltaTime);
        //     _characterController.transform.Translate(0.0f, Mathf.Sin(Time.deltaTime), 0.0f);
        // }

        yield return new WaitForSeconds(0.1f);
        _characterController.enabled = true;
        yield return new WaitForSeconds(0.1f);
        ReturnHook();

    }

    IEnumerator Climb()
    {
        yield return new WaitForSeconds(0.1f);
        ReturnHook();
    }

    private void ReturnHook()
    {
        hook.transform.rotation = hookHolder.transform.rotation;
        hook.transform.position = hookHolder.transform.position;
        fired = false;
        hooked = false;
        hook.GetComponent<Collider>().enabled = false;
        // LineRenderer rope = GetComponent<LineRenderer>();
        rope.positionCount = 0;
        // state.movementStateMaching = AgentStates.movementState.Idle;
    }

    private void CheckIfGround()
    {
        RaycastHit hit;
        float distance = 1f;
        Vector3 dir = new Vector3(0,-1);

        if(Physics.Raycast(_characterController.transform.position, dir, out hit, distance))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }

}
