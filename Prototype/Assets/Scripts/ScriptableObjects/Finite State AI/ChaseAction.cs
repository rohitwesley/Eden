using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/Chase")]
public class ChaseAction : StateAction 
{
    public override void Act (StateController controller)
    {
        Chase (controller);    
    }

    private void Chase(StateController controller)
    {
        controller.agentInfo.currentAction = AgentAction.Chase;
        // controller.navMeshAgent.destination = controller.chaseTarget.position;
        // controller.navMeshAgent.isStopped = false;
    }
}