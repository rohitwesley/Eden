using System.Collections;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] bool _moveCamera = true;                      // Whether the camera should be moved by this script.    
    [SerializeField] float _smoothing = 7f;                        // Smoothing applied during Slerp, higher is smoother but slower.
    [SerializeField] Vector3 _offset = new Vector3 (0f, 1.5f, 0f); // The offset from the player's position that the camera aims at.
    [SerializeField] Transform _playerPosition;                    // Reference to the player's Transform to aim at.


    private IEnumerator Start ()
    {
        // If the camera shouldn't move, do nothing.
        if(!_moveCamera)
            yield break;

        // Wait a single frame to ensure all other Starts are called first.
        yield return null;

        // Set the rotation of the camera to look at the player's position with a given offset.
        transform.rotation = Quaternion.LookRotation(_playerPosition.position - transform.position + _offset);
    }


    // LateUpdate is used so that all position updates have happened before the camera aims.
    private void LateUpdate ()
    {
        // If the camer shouldn't move, do nothing.
        if (!_moveCamera)
            return;

        // Find a new rotation aimed at the player's position with a given offset.
        Quaternion newRotation = Quaternion.LookRotation (_playerPosition.position - transform.position + _offset);

        // Spherically interpolate between the camera's current rotation and the new rotation.
        transform.rotation = Quaternion.Slerp (transform.rotation, newRotation, Time.deltaTime * _smoothing);
    }
}
