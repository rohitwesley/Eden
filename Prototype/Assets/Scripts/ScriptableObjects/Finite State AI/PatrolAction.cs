using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/Patrol")]
public class PatrolAction : StateAction
{
    public override void Act(StateController controller)
    {
        Patrol (controller);
    }

    private void Patrol(StateController controller)
    {
        controller.agentInfo.currentAction = AgentAction.Patrol;
        // controller.navMeshAgent.destination = controller.wayPointsForAI.waypoints [controller.nextWayPoint].position;
        // controller.navMeshAgent.Resume ();

        // if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance && !controller.navMeshAgent.pathPending) 
        // {
        //     controller.nextWayPoint = (controller.nextWayPoint + 1) % controller.wayPointsForAI.waypoints.Count;
        // }
    }
}