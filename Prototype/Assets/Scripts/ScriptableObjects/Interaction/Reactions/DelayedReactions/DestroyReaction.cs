using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyReaction  : DelayedReaction
{
    public GameObject gameObject;       // The gameobject to be destroyed
    public float lifetime;            // The time that the gameobject will take before getting destroyed


    protected override void ImmediateReaction()
    {
        Destroy (gameObject, lifetime);
    }
}