using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decisions/FoundAgent")]
public class FoundDecision : Decision {

    [SerializeField] AgentType LookTypeId;
    
    public override bool Decide(StateController controller)
    {
        bool targetVisible = Found(controller);
        return targetVisible;
    }

    private bool Found(StateController controller)
    {
        RaycastHit hit;

        Debug.DrawRay (controller.eyes.position, controller.eyes.forward.normalized * controller.agentInfo.AgentSettings.lookRange, Color.green);

        if (Physics.SphereCast (controller.eyes.position, controller.agentInfo.AgentSettings.lookSphereCastRadius, controller.eyes.forward, out hit, controller.agentInfo.AgentSettings.lookRange)
            && hit.collider.gameObject.GetComponent<CharacterInfo>() != null
            && hit.collider.gameObject.GetComponent<CharacterInfo>().AgentSettings.AgentId == LookTypeId) {
            controller.chaseTarget = hit.transform;
            float dist = Vector3.Distance(controller.gameObject.transform.position, hit.transform.position);

            if(dist <= 0.5f)
                return true;
            else
            {
                return false;
            }
        } else 
        {
            return false;
        }
    }
}
