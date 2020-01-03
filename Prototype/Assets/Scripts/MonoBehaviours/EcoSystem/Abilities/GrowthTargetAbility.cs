using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(ParticleSystem))]
public class GrowthTargetAbility : AgentAbility
{
    [SerializeField] MapSettings _mapSettings;
    [SerializeField] Transform mapCoordSpawnTransform;

    [SerializeField] float searchRadius = 2;
    [SerializeField] float searchDistance = 10;
    [SerializeField] LayerMask interactMask;
    [SerializeField] protected float _height = .3f;
    [SerializeField] protected float _gravity = -9.8f;
    [SerializeField] AgentInfo infoHookTarget;
    public int seed;
    static List<Coord> walkableCoords;

    [Header ("Trees")]
    [SerializeField] Population[] initialTreePopulations;

    private void Update()
    {
        // for (int i = 0; i < _mapSettings.terrainData.Count; i++)
        // {
        //     if (_mapSettings.terrainData[i].walkable!=null)
        //     {
        //         if(walkableCoords == null)walkableCoords = new List<Coord> ();

        //         for (int y = 0; y < _mapSettings.resolution; y++) {
        //             for (int x = 0; x < _mapSettings.resolution; x++) {
        //                 if (_mapSettings.terrainData[i].walkable[x, y]) {
        //                     // Mark walkable tile
        //                     if(_mapSettings.terrainData[i].walkable[x, y] == true)
        //                         walkableCoords.Add (new Coord (x, y));
        //                 }
        //             }
        //         }
        //     }
        // }
    }

    public override void PlayAbility () 
    {
        StartCoroutine(PopulateTrees());
    }

    private IEnumerator PopulateTrees()
    {
        var spawnPrng = new System.Random (seed);
        var spawnCoords = new List<Coord> (walkableCoords);

        foreach (var pop in initialTreePopulations) {
            for (int i = 0; i < pop.count; i++) {

                if (spawnCoords.Count == 0) {
                    Debug.Log ("Ran out of empty tiles to spawn initial population");
                    break;
                }
                int spawnCoordIndex = spawnPrng.Next (0, spawnCoords.Count);
                Coord coord = spawnCoords[spawnCoordIndex];
                spawnCoords.RemoveAt (spawnCoordIndex);

                //create instance of agent
                var entity = Instantiate (pop.prefab);
                if(entity.GetComponent<CharacterInfo>().AgentSettings.AgentId == AgentType.Predator
                    || entity.GetComponent<CharacterInfo>().AgentSettings.AgentId == AgentType.Elephant)
                {
                    // activate placement rigid body so that navmesh agent gets used.
                    entity.GetComponent<AgentMovement>().ActivateRigidBody();
                }
                //place agent on the map
                // entity.SetCoord(coord, _mapSettings.terrainData[0]); TODO
                //add agent to eco system list
                entity.transform.parent = mapCoordSpawnTransform;
                //adjust for world scale from map scale
                //TODO entity.transform.localScale = entity.transform.localScale * scaleObject;

            }
        }
        //Wait for .07 seconds
        yield return new WaitForSeconds(0.07f);
    }

    void OnDrawGizmos()
    {
        // Gizmos.color = Color.magenta;
        // Gizmos.DrawWireSphere (transform.position + transform.forward * 4, searchRadius);

        // if(walkableCoords != null)
        // foreach(Coord walkable in walkableCoords)
        // {
        //     infoHookTarget.SetCoord(walkable, _mapSettings.terrainData[0]);
        //     LaunchData launchData = CalculateLaunchVelocity(_height + infoHookTarget.transform.up.magnitude, _gravity, infoHookTarget.transform);
        //     Vector3 previousDrawPoint = _projectileHolder.transform.position;

        //     Gizmos.color =  Color.green;
        //     int resolution = 30;
        //     for (int i = 1; i <= resolution; i++) {
        //         float simulationTime = i / (float)resolution * launchData.timeToTarget;
        //         Vector3 displacement = launchData.initialVelocity * simulationTime + Vector3.up *_gravity * simulationTime * simulationTime / 2f;
        //         Vector3 drawPoint = _projectileHolder.transform.position + displacement;
        //         Gizmos.DrawLine(previousDrawPoint, drawPoint);
        //         previousDrawPoint = drawPoint;
        //     }
        // }
        
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

    [System.Serializable]
    public struct Population {
        public AgentInfo prefab;
        public int count;
    }

}
