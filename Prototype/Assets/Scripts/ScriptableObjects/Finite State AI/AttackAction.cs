using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/Attack")]
public class AttackAction : StateAction 
{
    [SerializeField] AgentType LookTypeId;
    
    public override void Act (StateController controller)
    {
        Attack (controller);
    }

    private void Attack(StateController controller)
    {
        RaycastHit hit;
        //Is a agent
        if(controller.agentInfo.AgentSettings != null && controller.eyes != null)
        {
            Debug.DrawRay (controller.eyes.position, controller.eyes.forward.normalized * controller.agentInfo.AgentSettings.attackRange, Color.red);

            Physics.SphereCast (controller.eyes.position, controller.agentInfo.AgentSettings.lookSphereCastRadius, controller.eyes.forward, out hit, controller.agentInfo.AgentSettings.attackRange);
            if (hit.collider != null)
            if (hit.collider.gameObject.GetComponent<CharacterInfo>() != null)
            if(hit.collider.gameObject.GetComponent<CharacterInfo>().AgentSettings.AgentId == LookTypeId) 
            {
                Debug.Log(controller.agentInfo.AgentSettings.AgentId + " Helth: " + controller.agentInfo.AgentSettings.startingHealth);
                controller.agentInfo.currentAction = AgentAction.Attack;
                
                if (controller.CheckIfCountDownElapsed (controller.agentInfo.AgentSettings.attackRate)) 
                {
                    hit.collider.gameObject.GetComponent<CharacterInfo>().TakeDamage(controller.agentInfo.AgentSettings.attackDamage);
                    // hit.collider.gameObject.GetComponent<CharacterInfo>().currentHealth -= controller.agentStats.attackDamage;
                    // controller.tankShooting.Fire (controller.enemyStats.attackForce, controller.enemyStats.attackRate);
                }
            }

        }

    }
}