using UnityEngine;

public class MovementStateReaction : DelayedReaction
{
    public AgentStates agent;   // The Animator that will have its int state parameter set.
    public AgentStates.movementState agentState;

    protected override void ImmediateReaction ()
    {
        agent.movementStateMaching = agentState;
    }
}
