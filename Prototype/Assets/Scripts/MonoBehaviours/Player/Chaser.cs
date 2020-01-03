using UnityEngine;

public class Chaser : MonoBehaviour
{

    [SerializeField] Transform targetTransform;
    [SerializeField] float speed = 0;

    void Update()
    {
        Vector3 displacmentFromTarget = targetTransform.position - transform.position;
        Vector3 directionToTarget = displacmentFromTarget.normalized;
        Vector3 velocity = directionToTarget * speed;

        float distanceToTarget = displacmentFromTarget.magnitude;

        if(distanceToTarget > 1.5f)
        {
            transform.Translate(velocity * Time.deltaTime);
            transform.Translate(0.0f, Mathf.Sin(Time.deltaTime), 0.0f);
        }
        
    }
}
