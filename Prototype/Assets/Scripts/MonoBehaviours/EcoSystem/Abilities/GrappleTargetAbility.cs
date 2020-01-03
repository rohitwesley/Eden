using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(ParticleSystem))]
public class GrappleTargetAbility : AgentAbility
{

    private LineRenderer laserLine;                                        // Reference to the LineRenderer component which will display our laserline
    [SerializeField] ParticleSystem _grappleParticleSystem;
    [SerializeField] ParticleSystem _splatterParticles;
    [SerializeField] Gradient _particleColorGradient;
    [SerializeField] ParticleDecalPool _splatDecalPool;
    
    List<ParticleCollisionEvent> _collisionEvents;
    
    [SerializeField] float searchRadius = 2;
    [SerializeField] float searchDistance = 10;
    [SerializeField] LayerMask interactMask;
    AgentInfo infoHookTarget;
    [SerializeField] protected float _height = .3f;
    [SerializeField] protected float _gravity = -9.8f;

    public bool IsGrapple { get; internal set; }

    void Start () 
    {   
        Name = "GrappleTargetAbility";
        // Get and store a reference to our LineRenderer component
        laserLine = GetComponent<LineRenderer>();
        // Turn off our line renderer by default
        laserLine.enabled = false;
        // Get and store a reference to our particle system component
        _grappleParticleSystem = GetComponent<ParticleSystem>();
        _collisionEvents = new List<ParticleCollisionEvent> ();

        // Get and store a reference to our AudioSource component
        gunAudio = GetComponent<AudioSource>();
    }

    void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents (_grappleParticleSystem, other, _collisionEvents);

        for (int i = 0; i < _collisionEvents.Count; i++) 
        {
            _splatDecalPool.ParticleHit (_collisionEvents [i], _particleColorGradient);
            EmitAtLocation (_collisionEvents[i]);
        }

    }

    void EmitAtLocation(ParticleCollisionEvent particleCollisionEvent)
    {
        _splatterParticles.transform.position = particleCollisionEvent.intersection;
        _splatterParticles.transform.rotation = Quaternion.LookRotation (particleCollisionEvent.normal);
        ParticleSystem.MainModule psMain = _splatterParticles.main;
        psMain.startColor = _particleColorGradient.Evaluate (UnityEngine.Random.Range (0f, 1f));

        _splatterParticles.Emit (1);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere (transform.position + transform.forward * 4, searchRadius);

        if(IsGrapple && infoHookTarget != null)
        {
            LaunchData launchData = CalculateLaunchVelocity(_height + infoHookTarget.transform.up.magnitude, _gravity);
            Vector3 previousDrawPoint = _projectileHolder.transform.position;

            Gizmos.color =  Color.green;
            int resolution = 30;
            for (int i = 1; i <= resolution; i++) {
                float simulationTime = i / (float)resolution * launchData.timeToTarget;
                Vector3 displacement = launchData.initialVelocity * simulationTime + Vector3.up *_gravity * simulationTime * simulationTime / 2f;
                Vector3 drawPoint = _projectileHolder.transform.position + displacement;
                Gizmos.DrawLine(previousDrawPoint, drawPoint);
                previousDrawPoint = drawPoint;
            }
        }
        
    }

    float projectileTime = 0;
    public Vector3 ProjectileLaunch()
    {
        
        if(IsGrapple && infoHookTarget != null)
        {
            LaunchData launchData = CalculateLaunchVelocity(_height + infoHookTarget.transform.up.magnitude, _gravity);
            return launchData.initialVelocity;

            Vector3 previousDrawPoint = _projectileHolder.transform.position;
            int resolution = 10;
            // for (int i = 1; i <= resolution; i++) {
                float simulationTime = projectileTime / (float)resolution * launchData.timeToTarget;
                Vector3 displacement = launchData.initialVelocity * simulationTime + Vector3.up *_gravity * simulationTime * simulationTime / 2f;
                Vector3 drawPoint = _projectileHolder.transform.position + displacement;
                // Gizmos.DrawLine(previousDrawPoint, drawPoint);
                previousDrawPoint = drawPoint;
                projectileTime++;
                if(projectileTime <= resolution)
                    projectileTime=0;
                return drawPoint;
            // }
        }

        return Vector3.zero;
        
    }

    public void ViewHooks()
    {
        // Debug.Log("Grapple Activated");
        //get all hookes in view
        Debug.DrawRay (transform.position + transform.forward * 4, transform.forward * searchDistance, Color.red);
        // RaycastHit[] hit = Physics.SphereCastAll(transform.position, searchRadius, transform.forward, searchDistance, interactMask);
        RaycastHit hit;
        Physics.SphereCast(transform.position + transform.forward * 4, searchRadius, transform.forward, out hit, searchDistance, interactMask);
        int i = 0;
        // while (i < hit.Length)
        // {
            // AgentInfo info = hit[i].collider.gameObject.GetComponent<AgentInfo>();
            if(hit.collider != null)
            {
                AgentInfo info = hit.collider.gameObject.GetComponent<AgentInfo>();
                if(info != null && info.AgentSettings.AgentId == AgentType.Hooks)
                {
                    Debug.Log("Scanner Hit " + info.AgentSettings.AgentId);
                    StartCoroutine(HighlightHooks(info));
                }
            }    
            i++;
        // }
    }    
  
    public void FindElivatedHooks()
    {
        // Debug.Log("Grapple Activated");
        //get all hookes in view
        Debug.DrawRay (transform.position + transform.up * 4, transform.up * searchDistance, Color.red);
        // RaycastHit[] hit = Physics.SphereCastAll(transform.position, searchRadius, transform.forward, searchDistance, interactMask);
        RaycastHit hit;
        Physics.SphereCast(transform.position + transform.up * 4, searchRadius * 2, transform.up, out hit, searchDistance, interactMask);
        int i = 0;
        // while (i < hit.Length)
        // {
            // AgentInfo info = hit[i].collider.gameObject.GetComponent<AgentInfo>();
            if(hit.collider != null)
            {
                AgentInfo info = hit.collider.gameObject.GetComponent<AgentInfo>();
                if(info != null && info.AgentSettings.AgentId == AgentType.Hooks)
                {
                    Debug.Log("Scanner Hit " + info.AgentSettings.AgentId);
                    StartCoroutine(HighlightHooks(info));
                }
            }    
            i++;
        // }
    } 

    public override void PlayAbility () 
    {
        ParticleSystem.MainModule psMain = _grappleParticleSystem.main;
        psMain.startColor = _particleColorGradient.Evaluate (UnityEngine.Random.Range (0f, 1f));
        // _grappleParticleSystem.Emit(1);
        
        StartCoroutine(GrappleHook());
        

    }

    private IEnumerator HighlightHooks(AgentInfo info)
    {
        // if(Vector3.Distance(transform.position, info.gameObject.transform.position) > 1)
        // {

        // }
        // if(infoHookTarget == null)
            // infoHookTarget = info;

        if(infoHookTarget != null)infoHookTarget.uiPanel.SetActive(false);
        //Wait for .07 seconds
        yield return new WaitForSeconds(0.07f);
        // if(Vector3.Distance(transform.position, info.gameObject.transform.position) < Vector3.Distance(transform.position, infoHookTarget.gameObject.transform.position))
            infoHookTarget = info;

        infoHookTarget.uiPanel.SetActive(true);
        //Wait for .07 seconds
        yield return new WaitForSeconds(0.07f);
        
    }

    private IEnumerator GrappleHook()
    {
        if(infoHookTarget != null)
        {
            // Play the shooting sound effect
            gunAudio.Play ();

            // Turn on our line renderer
            laserLine.enabled = true;
            // Set the start position for our visual effect for our laser to the position of gunEnd
            laserLine.SetPosition (0, _projectileHolder.transform.position);
            // Set the end position for our laser line 
            laserLine.SetPosition (1, infoHookTarget.gameObject.transform.position);
            //set it up with fixed values
            laserLine.startWidth = 0.05f;
            laserLine.endWidth = 0.05f;
            laserLine.startColor = Color.yellow;
            laserLine.endColor = Color.yellow;

            //Wait for .07 seconds
            yield return new WaitForSeconds(shotDuration);
        
        }
    }

    public void DeactivateGrappleHook()
    {
        if(infoHookTarget != null)
        {
            // Deactivate our line renderer after waiting
            laserLine.enabled = false; 
            IsGrapple = false;
            infoHookTarget.uiPanel.SetActive(false);
            infoHookTarget = null;           
        }
    }

    public Transform GetHook()
    {
        if(infoHookTarget != null)return infoHookTarget.gameObject.transform;
        else return null;
    }

    struct LaunchData {
        public readonly Vector3 initialVelocity;
        public readonly float timeToTarget;
        public LaunchData(Vector3 initialVelocity, float timeToTarget)
        {
            this.initialVelocity = initialVelocity;
            this.timeToTarget = timeToTarget;
        }
    }
    
    LaunchData CalculateLaunchVelocity(float _height, float _gravity)
    {
        // call only if there is infoHookTarget
        Transform _target = infoHookTarget.transform;
        float displacmentY = _target.position.y - _projectileHolder.transform.position.y;
        Vector3 displacmentXZ = new Vector3(_target.position.x - _projectileHolder.transform.position.x, 0, _target.position.z - _projectileHolder.transform.position.z);
        float time = Mathf.Sqrt(-2 * _height/ _gravity) + Mathf.Sqrt(2 * (displacmentY - _height)/_gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * _gravity * _height);
        Vector3 velocityXZ = displacmentXZ / time;
        //use sign(gravity) to account for negative projectile 
        return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(_gravity), time);
    }



}

public class ParticleDecalData 
{
    public Vector3 position;
    public float size;
    public Vector3 rotation;
    public Color color;

}

public class ParticleDecalPool : MonoBehaviour {

    public int maxDecals = 100;
    public float decalSizeMin = .5f;
    public float decalSizeMax = 1.5f;

    private ParticleSystem decalParticleSystem;
    private int particleDecalDataIndex;
    private ParticleDecalData[] particleData;
    private ParticleSystem.Particle[] particles;

    void Start () 
    {
        decalParticleSystem = GetComponent<ParticleSystem> ();
        particles = new ParticleSystem.Particle[maxDecals];
        particleData = new ParticleDecalData[maxDecals];
        for (int i = 0; i < maxDecals; i++) 
        {
            particleData [i] = new ParticleDecalData ();    
        }
    }

    public void ParticleHit(ParticleCollisionEvent particleCollisionEvent, Gradient colorGradient)
    {
        SetParticleData (particleCollisionEvent, colorGradient);
        DisplayParticles ();
    }

    void SetParticleData(ParticleCollisionEvent particleCollisionEvent, Gradient colorGradient)
    {
        if (particleDecalDataIndex >= maxDecals) 
        {
            particleDecalDataIndex = 0;
        }
            
        particleData [particleDecalDataIndex].position = particleCollisionEvent.intersection;
        Vector3 particleRotationEuler = Quaternion.LookRotation (particleCollisionEvent.normal).eulerAngles;
        particleRotationEuler.z = UnityEngine.Random.Range (0, 360);
        particleData [particleDecalDataIndex].rotation = particleRotationEuler;
        particleData [particleDecalDataIndex].size = UnityEngine.Random.Range (decalSizeMin, decalSizeMax);
        particleData [particleDecalDataIndex].color = colorGradient.Evaluate (UnityEngine.Random.Range (0f, 1f));

        particleDecalDataIndex++;
    }

    void DisplayParticles()
    {
        for (int i = 0; i < particleData.Length; i++) 
        {
            particles [i].position = particleData [i].position;
            particles [i].rotation3D = particleData [i].rotation;
            particles [i].startSize = particleData [i].size;
            particles [i].startColor = particleData [i].color;
        }

        decalParticleSystem.SetParticles (particles, particles.Length);
    }

}

public class SplatOnCollision : MonoBehaviour {

    public ParticleSystem particleLauncher;
    public Gradient particleColorGradient;
    public ParticleDecalPool dropletDecalPool;

    List<ParticleCollisionEvent> collisionEvents;


    void Start () 
    {
        collisionEvents = new List<ParticleCollisionEvent> ();
    }

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = ParticlePhysicsExtensions.GetCollisionEvents (particleLauncher, other, collisionEvents);

        int i = 0;
        while (i < numCollisionEvents) 
        {
            dropletDecalPool.ParticleHit(collisionEvents[i], particleColorGradient);
            i++;
        }

    }

}