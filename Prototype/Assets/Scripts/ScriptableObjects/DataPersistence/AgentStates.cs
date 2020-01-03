using UnityEngine;

[CreateAssetMenu]
public class AgentStates : ScriptableObject
{
    public enum movementState {
        Idle,
        Walk,
        Run,
        Jump,
        Climb,
        Swing,
        Glide,
        Fall,
        Die

    }

    public enum actionState {
        Explore,
        Heal,
        TakeDamage,
        Counter,
        MeleeAttack,
        RunRangeAttack

    }

    public movementState movementStateMaching = movementState.Idle;
    public actionState actiontSateMaching = actionState.Explore;


}
