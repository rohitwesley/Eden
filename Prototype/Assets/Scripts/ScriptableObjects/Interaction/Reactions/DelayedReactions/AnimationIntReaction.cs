using UnityEngine;

public class AnimationIntReaction : DelayedReaction
{
    public Animator animator;   // The Animator that will have its int state parameter set.
    public string stateName;      // The name of the int state parameter to be set.
    public int state;      // The state of the int state parameter to be set.


    private int intHash;    // The hash representing the int state parameter to be set.


    protected override void SpecificInit ()
    {
        intHash = Animator.StringToHash(stateName);
    }


    protected override void ImmediateReaction ()
    {
        animator.SetInteger(intHash, state);
    }
}
