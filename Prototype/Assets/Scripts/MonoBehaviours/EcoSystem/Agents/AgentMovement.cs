using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentMovement : StateController
{

    public Transform playerAgent;
    //used to place objects and when ever needed
    Rigidbody _rigidBodyAgent;
    NavMeshAgent _navMeshAgent;
    
    // [HideInInspector] public Complete.TankShooting tankShooting;
    [SerializeField] bool _UseLocalWaypoints = false;
    [SerializeField] WaypointPatrol _wayPointsForAI;
    int _nextWayPoint;

    private void Start()
    {
        // if we using waypoints localy from component and not generated
        if(_wayPointsForAI != null && _UseLocalWaypoints)
        {
            //TODO see if this is bad
            SetupAI(true, _wayPointsForAI);
        }
    }

    public void SetupAI(bool aiActivationFromAgentInfo, WaypointPatrol wayPointsList)
    {
        if(!_UseLocalWaypoints) _wayPointsForAI = wayPointsList;
        aiActive = aiActivationFromAgentInfo;
        StartCoroutine(ActivateNavmeshAgent());
        if (aiActive) 
        {
            _navMeshAgent.enabled = true;
        } else 
        {
            _navMeshAgent.enabled = false;
        }
        _navMeshAgent.baseOffset = 0f;
    }

    void Update()
    {
        
        if(agentInfo.currentAction == AgentAction.Patrol)
            Patrol();
        if(agentInfo.currentAction == AgentAction.Scan)
            Scan();
        if(agentInfo.currentAction == AgentAction.Chase)
            Chase();

        if (!aiActive)
            return;
        currentState.UpdateState (this);
        
    }

    private void Patrol()
    {
        if(_wayPointsForAI != null)
        {
            _navMeshAgent.destination = _wayPointsForAI.waypoints [_nextWayPoint].position;
            _navMeshAgent.isStopped = false;

            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance && !_navMeshAgent.pathPending) 
            {
                _nextWayPoint = (_nextWayPoint + 1) % _wayPointsForAI.waypoints.Count;
            }
        }
    }

    private bool Scan()
    {
        _navMeshAgent.isStopped = true;
        transform.Rotate (0, agentInfo.AgentSettings.searchingTurnSpeed * Time.deltaTime, 0);
        return CheckIfCountDownElapsed (agentInfo.AgentSettings.searchDuration);
    }

    private void Chase()
    {
        _navMeshAgent.destination = chaseTarget.position;
        _navMeshAgent.isStopped = false;
    }

    public void ActivateRigidBody()
    {
        if(_rigidBodyAgent == null)
        {
            _rigidBodyAgent = gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
                
        }
    }
    
    public void DeactivateRigidBody()
    {
        if(_rigidBodyAgent != null)
        {
            Destroy(_rigidBodyAgent);
                
        }
    }

    public IEnumerator ActivateNavmeshAgent()
    {
        _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        if(_navMeshAgent == null)
        {
            yield return _navMeshAgent = gameObject.AddComponent(typeof(NavMeshAgent)) as NavMeshAgent;
                
        }
    }
    
    public void DeactivateNavmeshAgent()
    {
        if(_navMeshAgent != null)
        {
            Destroy(_navMeshAgent);
                
        }
    }

}
