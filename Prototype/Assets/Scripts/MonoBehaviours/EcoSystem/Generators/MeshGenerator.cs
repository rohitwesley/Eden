using UnityEngine;

public class MeshGenerator : MonoBehaviour
{

    [HideInInspector]
    public bool foldout;
    public MapSettings _mapSettings;
    [HideInInspector]
    [SerializeField] GameObject[] _meshObjects;
    // [HideInInspector]
    // [SerializeField] MeshFilter[] _meshFilters;
    // [HideInInspector]
    // [SerializeField] Texture2D[] _meshTexture;
    
    // ShapeGenerator[] _planeFaces;

    // draw/update mesh
    public void OnObjectSettingsUpdated()
    {
        if(_mapSettings.autoUpdate)
        {
            GenerateObject();
        }
    }

    // draw/update mesh
    public void OnShapeSettingsUpdated()
    {
        if(_mapSettings.autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    // draw/update material
    public void OnMaterialSettingsUpdated()
    {
        if(_mapSettings.autoUpdate)
        {
            Initialize();
            GenerateMaterials();
        }
    }

    void Start () {
        GenerateObject();
    }

    private void Initialize()
    {
        int TotalSides = 0;
        if(_mapSettings.mode == MapSettings.MeshModes.Plane)
        {                
            TotalSides = 1;
        }
        else
        {
            TotalSides = 6;
        }

        // // only create mesh filter once and then reuse it
        // if(_meshFilters == null || _meshFilters.Length == 0)
        // {
        //     _meshFilters = new MeshFilter[TotalSides];            
        // }

        // // only create textures once and then reuse it
        // if(_meshTexture == null || _meshTexture.Length == 0)
        // {
        //     _meshTexture = new Texture2D[TotalSides];            
        // }
        
        _mapSettings.planeFaces = new ShapeGenerator[TotalSides];

        Vector3[] directions;

        if(_mapSettings.mode == MapSettings.MeshModes.Plane)
        {
            directions = new Vector3[]{Vector3.up, Vector3.up, Vector3.up, Vector3.up, Vector3.up, Vector3.up};
        }
        else
        {
            directions = new Vector3[]{Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back};
        }

        if(_meshObjects != null)
        {
            // destroy object in editor and at run time
            foreach(GameObject obj in _meshObjects)
                DestroyImmediate(obj);

            Debug.Log("destroyed objects");
        }
        _meshObjects = new GameObject[TotalSides];
                
        for(var i =0; i < TotalSides; i++)
        {
            // if mesh not already created create it else just update it
            // if(_meshFilters[i] == null){
                //create new component called mesh
                GameObject meshObj = new GameObject("mesh");
                //parent it to this gameobject
                // meshObj.gameObject.transform.SetPositionAndRotation(new Vector3(0.0f,0.0f,0.0f), Quaternion.identity);
                meshObj.transform.parent = transform;
                // assign default material to the gameobject
                MeshFilter _meshFilters = meshObj.AddComponent<MeshFilter>();
                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                // _meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = _mapSettings.mapMaterial;
                _meshFilters.sharedMesh = new Mesh();
                _meshFilters.GetComponent<MeshRenderer>().sharedMaterial.color = _mapSettings.mapColor;
                Texture2D _meshTexture = new Texture2D(_mapSettings.resolution, _mapSettings.resolution);
                _meshFilters.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = _meshTexture;
                _meshObjects[i] = meshObj;
                MeshCollider meshCollider = _meshObjects[i].AddComponent<MeshCollider>();
                // _meshFilters.sharedMesh = meshCollider;
                // assign map settings material material to the gameobject
            // }
            
            _meshObjects[i].layer = LayerMask.NameToLayer("Floor");
            _mapSettings.planeFaces[i] = new ShapeGenerator(_mapSettings, _meshFilters.sharedMesh,  _meshFilters.GetComponent<MeshRenderer>(), _meshTexture, directions[i]);
            
        }
        Debug.Log("Initialized");
    }

    // draw/update mesh and material
    public void GenerateObject()
    {
        Initialize();
        GenerateMesh();
        GenerateMaterials();
    }

    void GenerateMesh()
    {
        foreach(ShapeGenerator face in _mapSettings.planeFaces)
        {
            face.ConstructMesh();
        }
        _mapSettings.IsGeneraterMesh = true;
        Debug.Log("Generated Mesh");

    }

    void GenerateMaterials()
    {
        foreach(ShapeGenerator face in _mapSettings.planeFaces)
        {
            face.ConstructMap(1.23f);
            face.ConstructTextureMap();
        }
        Debug.Log("Generate Material");
    }

}

