using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTargetReached : MonoBehaviour
{
    [SerializeField] Projectile _projectileSystem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Projectile")
        {
            Debug.Log("I reached");
            _projectileSystem.SetProjectile();
        }
        
    }
}
