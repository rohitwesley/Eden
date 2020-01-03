using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class AgentAbility : MonoBehaviour
{
    public string Name;
    protected GameObject _target;
    [SerializeField] protected GameObject _projectile;
    [SerializeField] protected GameObject _projectileHolder;                                            // Holds a reference to the gun end object, marking the muzzle location of the gun

    [SerializeField] protected int _impact = 5;                                            // Set the number of hitpoints that this gun will take away from shot objects with a health script
    [SerializeField] protected float _ratePer = 0.25f;                                        // Number in seconds which controls how often the player can fire
    [SerializeField] protected float _range = 50f;
    [SerializeField] protected float hitForce = 100f;                                        // Amount of force which will be added to objects with a rigidbody shot by the player
   
    [SerializeField] protected Camera playerCam;                                                // Holds a reference to the first person camera
    [SerializeField] protected float shotDuration = 0.7f;    // WaitForSeconds object used by our ShotEffect coroutine, determines time laser line will remain visible
    [SerializeField] protected AudioSource gunAudio;     

    public abstract void PlayAbility();

}
