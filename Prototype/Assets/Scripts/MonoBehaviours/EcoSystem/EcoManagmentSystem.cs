using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EcoManagmentSystem : MonoBehaviour
{
    public int seed;
    static System.Random prng;

    [SerializeField] MapSettings _mapSettings;

    [SerializeField] NavMeshSurface navMeshSurface;

    [Header ("Player")]
    [SerializeField] InteractionManager agentPlayer;
    [SerializeField] Transform playerCoordSpawnTransform;
    
    [Header ("Stationary Agents")]
    [SerializeField] MeshRenderer treePrefab;
    [Range (0, 1)]
    [SerializeField] float treePlacementProbability = 0.5f;
    
    [Header ("Boid Agents")]
    [SerializeField] MeshRenderer boidPrefab;
    [Range (0, 1)]
    [SerializeField] float boidPlacementProbability = 0.5f;

    [Header ("Movable Agents")]
    [SerializeField] Population[] initialPopulations;

    [Header ("Patrol Wapoint")]
    [SerializeField] List<WaypointPatrol> waypointList;

    // Cached data:
    public static Vector3[, ] tileCentres;
    static int size;
    static List<Coord> walkableCoords;
    // [SerializeField] Transform mapCoordTransform;
    [SerializeField] Transform mapCoordSpawnTransform;
    [SerializeField] float scaleObject;

    void Start () {
        prng = new System.Random ();
        Init();
        
    }

    // Call terrain generator and cache useful info
    void Init () {
        var sw = System.Diagnostics.Stopwatch.StartNew ();

        if(_mapSettings != null){
            // var terrainGenerator = FindObjectOfType<TerrainGenerator> ();
            // terrainData = terrainGenerator.Generate ();
            tileCentres = _mapSettings.terrainData.tileCentres;
            size = _mapSettings.terrainData.size;

            SpawnTrees ();
            SpawnInitialPopulations ();
            this.transform.localScale = Vector3.one / scaleObject;
            this.transform.localPosition = transform.position + new Vector3(0.0f, -1/scaleObject, 0.0f);
            Debug.Log("Eco Generated");
        }
        
        // Update navmesh
        navMeshSurface.BuildNavMesh();
    }
    
    void SpawnWaypoints (WaypointPatrol waypointPatrol, System.Random spawnPrng) {
        var spawnCoords = new List<Coord> (walkableCoords);
        for (int i = 0; i < waypointPatrol.waypoints.Count; i++) {
            if (spawnCoords.Count == 0) {
                Debug.Log ("Ran out of empty tiles to spawn initial population");
                break;
            }
            int spawnCoordIndex = spawnPrng.Next(0, spawnCoords.Count);
            Coord coord = spawnCoords[spawnCoordIndex];
            spawnCoords.RemoveAt (spawnCoordIndex);

            var entity = waypointPatrol.waypoints[i].gameObject;
            // entity.SetCoord(coord);
            entity.transform.position =  EcoManagmentSystem.tileCentres[coord.x, coord.y];
            // entity.transform.localScale = entity.transform.localScale * scaleObject;

        }
    }

    void SpawnTrees () {
        // Settings:
        float maxRot = 4;
        float maxScaleDeviation = .2f;
        float colVariationFactor = 0.15f;
        float minCol = .8f;

        var spawnPrng = new System.Random (seed);
        walkableCoords = new List<Coord> ();

        if(_mapSettings.terrainData.walkable == null)
        {
            Debug.Log("Error walkable on spawning trees");
        }
        else
        {
             Debug.Log("Spawning trees");
        }

        for (int y = 0; y < _mapSettings.resolution; y++) {
            for (int x = 0; x < _mapSettings.resolution; x++) {
                if (_mapSettings.terrainData.walkable[x, y]) {
                    if (prng.NextDouble () < treePlacementProbability) {
                        
                        // Randomize rot/scale
                        float rotX = Mathf.Lerp (-maxRot, maxRot, (float) spawnPrng.NextDouble ());
                        float rotZ = Mathf.Lerp (-maxRot, maxRot, (float) spawnPrng.NextDouble ());
                        float rotY = (float) spawnPrng.NextDouble () * 360f;
                        Quaternion rot = Quaternion.Euler (rotX, rotY, rotZ);
                        float scale = 1 + ((float) spawnPrng.NextDouble () * 2 - 1) * maxScaleDeviation;

                        // Randomize colour
                        float col = Mathf.Lerp (minCol, 1, (float) spawnPrng.NextDouble ());
                        float r = col + ((float) spawnPrng.NextDouble () * 2 - 1) * colVariationFactor;
                        float g = col + ((float) spawnPrng.NextDouble () * 2 - 1) * colVariationFactor;
                        float b = col + ((float) spawnPrng.NextDouble () * 2 - 1) * colVariationFactor;

                        // Spawn
                        MeshRenderer tree = Instantiate (treePrefab, tileCentres[x,y], rot);
                        tree.transform.parent = mapCoordSpawnTransform;
                        tree.transform.localScale *= scaleObject;

                        // Mark tile unwalkable
                        _mapSettings.terrainData.walkable[x, y] = false;
                    } else {
                        walkableCoords.Add (new Coord (x, y));
                    }
                }
            }
        }
    
    }

    void SpawnInitialPopulations () {
        var spawnPrng = new System.Random (seed);
        var spawnCoords = new List<Coord> (walkableCoords);

        foreach (var pop in initialPopulations) {
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
                entity.SetCoord (coord);
                //add agent to eco system list
                entity.transform.parent = mapCoordSpawnTransform;
                //adjust for world scale from map scale
                entity.transform.localScale = entity.transform.localScale * scaleObject;
                
                //if agents characters
                if(entity.GetComponent<CharacterInfo>().AgentSettings.AgentId == AgentType.Predator
                    || entity.GetComponent<CharacterInfo>().AgentSettings.AgentId == AgentType.Elephant)
                {
                    // destroy placement rigid body so that navmesh agent gets used.
                    entity.GetComponent<AgentMovement>().DeactivateRigidBody();
                    
                    //add player reference to agent script
                    entity.GetComponent<AgentMovement>().playerAgent = agentPlayer.gameObject.transform;
                    
                    //spawn waypoints on map for agent to patrol through.
                    var wayPointsForAI = Instantiate(waypointList[0]);
                    wayPointsForAI.transform.parent = mapCoordSpawnTransform;
                    SpawnWaypoints(wayPointsForAI,spawnPrng);
                    entity.GetComponent<AgentMovement>().SetupAI(true, wayPointsForAI);

                }

                if (entity is FoodInfo) {
                    _mapSettings.terrainData.plantMap.Add (entity, coord);
                } else {
                    _mapSettings.terrainData.preyMap.Add (entity, coord);
                    // mapCoordTransform = entity.transform;
                }
            }
        }
    }

    public void resetPlayer()
    {
        agentPlayer.gameObject.SetActive(false);
        agentPlayer.gameObject.transform.SetPositionAndRotation(
            playerCoordSpawnTransform.position, playerCoordSpawnTransform.rotation
        );
        agentPlayer.gameObject.SetActive(true);
    }

    [System.Serializable]
    public struct Population {
        public AgentInfo prefab;
        public int count;
    }

}
