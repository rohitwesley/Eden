using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour {

    public CharacterInfo agentInfo;
    public State currentState;
    public Transform eyes;
    public State remainState;
    [HideInInspector] public float stateTimeElapsed;
    [HideInInspector] public Transform chaseTarget;
    protected bool aiActive;

    void Update()
    {
        if (!aiActive)
            return;
        currentState.UpdateState (this);
    }

    void OnDrawGizmos()
    {
        if (currentState != null && eyes != null && agentInfo.AgentSettings != null) 
        {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawWireSphere (eyes.position, agentInfo.AgentSettings.lookSphereCastRadius);
        }
    }

    public void TransitionToState(State nextState)
    {
        if (nextState != remainState) 
        {
            currentState = nextState;
            OnExitState ();
        }
    }

    public bool CheckIfCountDownElapsed(float duration)
    {
        stateTimeElapsed += Time.deltaTime;
        return (stateTimeElapsed >= duration);
    }

    private void OnExitState()
    {
        stateTimeElapsed = 0;
    }

    // public abstract void DoAction();

}
