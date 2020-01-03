using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class MapSettings : ScriptableObject
{
    public enum MeshModes{
        Plane,
        Cube,
        Sphere,
    }
    public MeshModes mode = MeshModes.Plane;
    [Range(2,256)]
    public int resolution = 10;
    public bool autoUpdate = true;
    public bool IsGeneraterMesh = false;

    public float planetRadius = 1;
    public NoiseLayer[] noiseLayers;
    public ShapeGenerator[] planeFaces;
    public Biome[] biome;
    public TerrainData terrainData;

    public Color mapColor;
    public Material mapMaterial;
    
    [System.Serializable]
    public class NoiseLayer
    {
        public bool enabled = true;
        public bool useFirstLayerAsMask;
        public NoiseSettings noiseSettings;
    }
    
    [System.Serializable]
    public class Biome
    {
        public enum BiomeType{
            Water,
            Land,
            Sky,
        }
        public BiomeType biomeType;
        [Range (-1, 1)]
        public float height;
        public Color color;
        public Color startCol;
        public Color endCol;
        public int numSteps;
    }

    [System.Serializable]
    public class TerrainData {
        public int size;
        public Vector3[, ] tileCentres;
        public bool[, ] walkable;
        public bool[, ] shore;
        public Map preyMap;
        public Map plantMap;

        public TerrainData (int size) {
            this.size = size;
            tileCentres = new Vector3[size, size];
            walkable = new bool[size, size];
            shore = new bool[size, size];
            preyMap = new Map(size, size);
            plantMap = new Map(size, size);
            Debug.Log("Map Initialised");
        }
    }
}
