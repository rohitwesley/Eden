using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    // [SerializeField] private Interactable _currentInteractable;   // The interactable that is currently being headed towards.

    void Update()
    {   
        // // If the player is stopping at an interactable...
        // if (_currentInteractable)
        // {
        //     // ... set the player's rotation to match the interactionLocation's.
        //     transform.rotation = _currentInteractable.interactionLocation.rotation;

        //     // Interact with the interactable and then null it out so this interaction only happens once.
        //     _currentInteractable.Interact();
        //     _currentInteractable = null;

        //     // Start the WaitForInteraction coroutine so that input is ignored briefly.
        //     // StartCoroutine (WaitForInteraction ());
        // }
    
    }

    public void OnInteractableClick(Interactable interactable)
    {
        // If the handle input flag is set to false then do nothing.
        // if(!handleInput)
        //     return;

        // Store the interactble that was clicked on.
        // _currentInteractable = interactable;

        // Set the destination to the interaction location of the interactable.
        // destinationPosition = _currentInteractable.interactionLocation.position;

        // Set the destination of the nav mesh agent to the found destination position and start the nav mesh agent going.
        // agent.SetDestination(destinationPosition);
        // agent.isStopped = false;

        Debug.Log("InteractClick");
        

    }

    // private IEnumerator WaitForIneraction()
    // {
    //     // As soon as the wait starts, input should no longer be accepted.
    //     handleInput = false;

    //     // Wait for the normal pause on interaction.
    //     yield return inputHoldWait;

    //     // Until the animator is in a state with the Locomotion tag, wait.
    //     while (animator.GetCurrentAnimatorStateInfo (0).tagHash != hashLocomotionTag)
    //     {
    //         yield return null;
    //     }

    //     // Now input can be accepted again.
    //     handleInput = true;
    // }

}
