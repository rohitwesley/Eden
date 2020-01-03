using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/LookForAgent")]
public class LookDecision : Decision {

    [SerializeField] AgentType LookTypeId;
    
    public override bool Decide(StateController controller)
    {
        bool targetVisible = Look(controller);
        return targetVisible;
    }

    private bool Look(StateController controller)
    {
        RaycastHit hit;

        if(controller.agentInfo.AgentSettings != null && controller.eyes != null) 
        {
            Debug.DrawRay (controller.eyes.position, controller.eyes.forward.normalized * controller.agentInfo.AgentSettings.lookRange, Color.green);
        
            if (Physics.SphereCast (controller.eyes.position, controller.agentInfo.AgentSettings.lookSphereCastRadius, controller.eyes.forward, out hit, controller.agentInfo.AgentSettings.lookRange)
                && hit.collider.gameObject.GetComponent<CharacterInfo>() != null
                && hit.collider.gameObject.GetComponent<CharacterInfo>().AgentSettings.AgentId == LookTypeId) {
                controller.chaseTarget = hit.transform;
                return true;
            }
        }
        return false;
    }
}