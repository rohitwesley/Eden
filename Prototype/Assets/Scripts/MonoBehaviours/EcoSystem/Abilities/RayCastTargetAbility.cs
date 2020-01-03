using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RayCastTargetAbility : AgentAbility
{
    private LineRenderer laserLine;                                        // Reference to the LineRenderer component which will display our laserline
    private float nextFire;                                                // Float to store the time the player will be allowed to fire again, after firing


    void Start () 
    {
        Name = "RayCastTargetAbility";
        // Get and store a reference to our LineRenderer component
        laserLine = GetComponent<LineRenderer>();
        // Turn off our line renderer by default
        laserLine.enabled = false;
        // Get and store a reference to our AudioSource component
        gunAudio = GetComponent<AudioSource>();
    }


    public override void PlayAbility() 
    {
        // Check if the player has pressed the fire button and if enough time has elapsed since they last fired
        // if (Input.GetButtonDown("Fire1") && 
        if(Time.time > nextFire) 
        {
            // Update the time when our player can fire next
            nextFire = Time.time + _ratePer;

            // Start our ShotEffect coroutine to turn our laser line on and off
            StartCoroutine (ShotEffect());

            // Create a vector at the center of our camera's viewport
            Vector3 rayOrigin = playerCam.ViewportToWorldPoint (new Vector3(0.5f, 0.5f, 0.0f));

            // Draw a line in the Scene View  from the point lineOrigin in the direction of fpsCam.transform.forward * weaponRange, using the color green
            Debug.DrawRay(rayOrigin, playerCam.transform.forward * _range, Color.green);
            
            // Declare a raycast hit to store information about what our raycast has hit
            RaycastHit hit;

            // Set the start position for our visual effect for our laser to the position of gunEnd
            laserLine.SetPosition (0, _projectileHolder.transform.position);

            // Check if our raycast has hit anything
            if (Physics.Raycast (rayOrigin, playerCam.transform.forward, out hit, _range))
            {
                
                // Set the end position for our laser line 
                laserLine.SetPosition (1, hit.point);

                // Get a reference to a health script attached to the collider we hit
                CharacterInfo health = hit.collider.GetComponent<CharacterInfo>();

                // If there was a health script attached
                if (health != null)
                {
                    // Call the damage function of that script, passing in our gunDamage variable
                    health.TakeDamage (_impact);
                }

                // Check if the object we hit has a rigidbody attached
                if (hit.rigidbody != null)
                {
                    // Add force to the rigidbody we hit, in the direction from which it was hit
                    hit.rigidbody.AddForce (-hit.normal * hitForce);
                }
            }
            else
            {
                // If we did not hit anything, set the end of the line to a position directly in front of the camera at the distance of weaponRange
                laserLine.SetPosition (1, rayOrigin + (playerCam.transform.forward * _range));
            }
        }
    }


    private IEnumerator ShotEffect()
    {
        // Play the shooting sound effect
        gunAudio.Play ();

        // Turn on our line renderer
        laserLine.enabled = true;

        //set it up with fixed values
        laserLine.startWidth = 0.05f;
        laserLine.endWidth = 0.05f;
        laserLine.startColor = Color.yellow;
        laserLine.endColor = Color.yellow;

        //Wait for .07 seconds
        yield return new WaitForSeconds(shotDuration);

        // Deactivate our line renderer after waiting
        laserLine.enabled = false;
    }

}
