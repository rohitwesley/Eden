using System;
using System.Collections.Generic;
using UnityEngine;


public class ShapeGenerator 
{
    //TODO better way of accessing these modes
    // private MeshGenerator.MeshModes _mode;
    MapSettings _mapSettings;
    Mesh _mesh;
    Renderer _renderer;
    Vector3[] _map3D;
    float[,] _map2D;
    MinMax _elevation3DMinMax = new MinMax();
    MinMax _elevation2DMinMax = new MinMax();
    Vector3 _localUp;
    Vector3 _axisA;
    Vector3 _axisB;
    
    // [Header ("Info")]
    public INoiseFilter[] _noiseFilter;
    public Texture2D texture;
    public int numTiles;
    public int numLandTiles;
    public int numWaterTiles;
    public float waterPercent;
    public bool centralize = true;
    public float waterDepth = .2f;
    public float edgeDepth = .2f;

    public ShapeGenerator(MapSettings mapSettings, Mesh mesh, Renderer renderer, Texture2D texture, Vector3 localUp)
    {
        _mapSettings = mapSettings;

        _mesh = mesh;
        _renderer = renderer;
        this.texture = texture;
        // up
        _localUp = localUp;
        //swap cordinates of local up to get axis a on the plane
        _axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        // axisB is on the plane perpendicular to axisA and lookup using cross product 
        _axisB = Vector3.Cross(_localUp, _axisA);

        _noiseFilter = new INoiseFilter[_mapSettings.noiseLayers.Length];
        for(var i = 0; i < _noiseFilter.Length; i++)
        {
            if(_noiseFilter[i] == null)
                _noiseFilter[i] = NoiseFilterGenerator.CreateNoiseFilter(_mapSettings.noiseLayers[i].noiseSettings);

        }
    }

    public void ConstructMesh()
    {
        _map3D = new Vector3[_mapSettings.resolution * _mapSettings.resolution];
        // total no. of triangle to form the plane (each plane is made up resolution - 1 rows and collumns which are made up of 2 triangles having 3 vertices each)
        int[] triangles = new int[(_mapSettings.resolution - 1) * (_mapSettings.resolution - 1) * 2 * 3];
        int triIndex = 0;
        
        //run through each point on the plane row by row
        for(var y = 0; y < _mapSettings.resolution; y++)
        {
            for(var x = 0; x < _mapSettings.resolution; x++)
            {
                

                // Vertex Index across the plane from top left to bottom right.
                int i = x + y * _mapSettings.resolution;
                //percent of vertex per row completed
                Vector2 percent = new Vector2(x,y) / (_mapSettings.resolution - 1);
                if(_mapSettings.mode == MapSettings.MeshModes.Plane)
                {
                    // point on unity plane along axisA and axisB;
                    Vector3 pointOnUnitPlane = Vector3.up + (percent.x - 0.5f) * 2 * _axisA + (percent.y - .5f) * 2 * _axisB;
                    _map3D[i] = CalculatePointOnScaledMap(pointOnUnitPlane);
                }
                else if(_mapSettings.mode == MapSettings.MeshModes.Cube)
                {
                    //move planes points 1 unit up on the localUP axis
                    Vector3 pointOnUnitCube = _localUp + (percent.x - 0.5f) * 2 * _axisA + (percent.y - .5f) * 2 * _axisB;
                    _map3D[i] = CalculatePointOnScaledMap(pointOnUnitCube);
                }
                else if(_mapSettings.mode == MapSettings.MeshModes.Sphere)
                {
                    //move planes points 1 unit up on the localUP axis
                    Vector3 pointOnUnitCube = _localUp + (percent.x - 0.5f) * 2 * _axisA + (percent.y - .5f) * 2 * _axisB;
                    Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                    _map3D[i] = CalculatePointOnScaledSphere(pointOnUnitSphere);
                }

                if(i == 0 )
                {
                    _elevation3DMinMax.ResetValue(_map3D[i].magnitude);
                }
                _elevation3DMinMax.AddValue(_map3D[i].magnitude);

                if(x != _mapSettings.resolution - 1 && y != _mapSettings.resolution - 1)
                {
                    // firts trinagle of the plane
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + _mapSettings.resolution + 1;
                    triangles[triIndex + 2] = i + _mapSettings.resolution;

                    // second triangle of the plane
                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + _mapSettings.resolution + 1;
                    triIndex += 6;
                }
            }
        }         

        Debug.Log("map3D ready");
        _mesh.Clear(); 
        _mesh.vertices = _map3D;
        _mesh.triangles = triangles;
        Vector2[] uvs = new Vector2[_map3D.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            if(_localUp == Vector3.up) uvs[i] = new Vector2((_map3D[i].x-1.0f)/2, 1-(_map3D[i].z - 1.0f)/2);
            if(_localUp == Vector3.down) uvs[i] = new Vector2(1-(_map3D[i].x-1.0f)/2, 1-(_map3D[i].z - 1.0f)/2);
            if(_localUp == Vector3.left) uvs[i] =new Vector2((_map3D[i].x-1.0f)/2, (1.0f - _map3D[i].z)/2);
            if(_localUp == Vector3.right) uvs[i] = new Vector2((_map3D[i].x-1.0f)/2, (1.0f - _map3D[i].z)/2);
            if(_localUp == Vector3.forward) uvs[i] = new Vector2((_map3D[i].x-1.0f)/2, (1.0f - _map3D[i].z)/2);
            if(_localUp == Vector3.back) uvs[i] = new Vector2((_map3D[i].x-1.0f), (1.0f - _map3D[i].z));
            // uvs[i] = new Vector2((map3D[i].x-1.0f)/2, (1.0f - map3D[i].z)/2);
        }
        _mesh.uv = uvs;
        _mesh.RecalculateNormals();
        
    }

    public void ConstructMap(float scale)
    {
        _map2D = new float[_mapSettings.resolution, _mapSettings.resolution];
        if(scale <= 0)
        {
            scale = 0.0001f;
        }
        for(var y = 0; y < _mapSettings.resolution; y++)
        {
            for(var x = 0; x < _mapSettings.resolution; x++)
            {
                float sampleX = x / scale;
                float sampleY = y / scale;
                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                _map2D[x, y] = perlinValue;
                Vector3 pointOnUnitMap = new Vector3(x, 1.0f, y);
                // map2D[x, y] = CalculatePointOnScaledMap(pointOnUnitMap);
                int i = x + y * _mapSettings.resolution;
                //TODO find a better way to account for all sides of the sphere/cube
                _map2D[x, y] = _map3D[i].magnitude;// - _mapSettings.planetRadius;//Mathf.InverseLerp(elevation3DMinMax.Min, elevation3DMinMax.Max, map3D[i].y);
                if(_localUp == Vector3.up) _map2D[x, y] = Mathf.InverseLerp(_elevation3DMinMax.Min, _elevation3DMinMax.Max, Mathf.Abs(_map3D[i].y));
                if(_localUp == Vector3.down) _map2D[x, y] = Mathf.InverseLerp(_elevation3DMinMax.Min, _elevation3DMinMax.Max, Mathf.Abs(_map3D[i].y));
                // if(_localUp == Vector3.left) map2D[x, y] = Mathf.InverseLerp(elevation3DMinMax.Min, elevation3DMinMax.Max, Mathf.Abs(map3D[i].magnitude));
                // if(_localUp == Vector3.right) map2D[x, y] = Mathf.InverseLerp(elevation3DMinMax.Min, elevation3DMinMax.Max, Mathf.Abs(map3D[i].magnitude));
                // if(_localUp == Vector3.forward)map2D[x, y] = Mathf.InverseLerp(elevation3DMinMax.Min, elevation3DMinMax.Max, Mathf.Abs(map3D[i].magnitude));
                // if(_localUp == Vector3.back) map2D[x, y] = Mathf.InverseLerp(elevation3DMinMax.Min, elevation3DMinMax.Max, Mathf.Abs(map3D[i].magnitude));
                // Debug.Log("Min: " + elevation3DMinMax.Min + "Max: " + elevation3DMinMax.Max + "3d Map: " + map3D[i].magnitude + "2D Map: " + map2D[x, y]);
                if(i == 0 )
                {
                    _elevation2DMinMax.ResetValue(_map2D[x, y]);
                }
                _elevation2DMinMax.AddValue(_map2D[x, y]);

            }
        }
        Debug.Log("map2D ready");
    }

    public void ConstructTextureMap()
    {
        // _mapSettings.mapNoise = new Texture2D(_mapSettings.resolution,_mapSettings.resolution);
        Color[] colorMap = new Color[_mapSettings.resolution * _mapSettings.resolution];

        float min = (centralize) ? -_mapSettings.resolution / 2f : 0;
        // Terrain data:
        _mapSettings.terrainData = new MapSettings.TerrainData(_mapSettings.resolution);
        numLandTiles = 0;
        numWaterTiles = 0;
        
        var uvs = new List<Vector2> ();

        for(var y = 0; y < _mapSettings.resolution; y++)
        {
            for(var x = 0; x < _mapSettings.resolution; x++)
            {
                Vector2 uv = GetBiomeInfo (_map2D[x, y], _mapSettings.biome);
                uvs.AddRange (new Vector2[] { uv, uv, uv, uv });

                bool isWaterTile = false;//uv.x == 0f;
                bool isLandTile = !isWaterTile;


                float height = (isWaterTile) ? -waterDepth : 0;
                Vector3 nw = new Vector3 (min + x, height, min + y + 1);

                // Vertex Index across the plane from top left to bottom right.
                int i = x + y * _mapSettings.resolution;
                colorMap[i] = Color.Lerp(Color.black, Color.white, _map2D[x, y]);
                for(var j =0; j < _mapSettings.biome.Length; j++)
                {
                    if(_map2D[x, y] <= _mapSettings.biome[j].height)
                    {
                        if(_mapSettings.biome[j].biomeType == MapSettings.Biome.BiomeType.Water)
                        {
                            isWaterTile = true;
                            isLandTile = !isWaterTile;
                        }
                        colorMap[i] = _mapSettings.biome[j].color;                        
                    }
                }

                if (isWaterTile) {
                    numWaterTiles++;
                } else {
                    numLandTiles++;
                }
                // Terrain data:
                _mapSettings.terrainData.tileCentres[x, y] = _map3D[i];//nw + new Vector3 (0.5f, 0, -0.5f);
                _mapSettings.terrainData.walkable[x, y] = isLandTile;
                // colorMap[i] = Color.Lerp(Color.black, Color.white, isLandTile ? 1.0f : 0.0f);
            }
        }

        numTiles = numLandTiles + numWaterTiles;
        waterPercent = numWaterTiles / (float) numTiles;

        if(_mapSettings.terrainData.walkable == null)
        {
            Debug.Log("Error walkable on Texture creation");
        }
        else
        {
             Debug.Log("walkable on Texture created");
        }

        texture = new Texture2D(_mapSettings.resolution, _mapSettings.resolution);
        texture.filterMode = FilterMode.Point;
        // _texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        _renderer.sharedMaterial.color = _mapSettings.mapColor;
        _renderer.sharedMaterial.mainTexture = texture;
        // _renderer.transform.localScale = new Vector3(_mapSettings.resolution, 1, _mapSettings.resolution);
        // _renderer.transform.localScale = new Vector3(1, 1, 1);
        Debug.Log("mapTexture ready");
        
        // if (_renderer.sharedMaterial != null) {
        //     Color[] startCols = { water.startCol, sand.startCol, grass.startCol };
        //     Color[] endCols = { water.endCol, sand.endCol, grass.endCol };
        //     _renderer.sharedMaterial.SetColorArray ("_StartCols", startCols);
        //     _renderer.sharedMaterial.SetColorArray ("_EndCols", endCols);
        // }
        
    }

    public Vector3 CalculatePointOnScaledMap(Vector3 pointOnUnitMap)
    {
        float firstLayerValue = 0;        
        float elevation = 0;

        // evaluate for first noise filter
        if(_noiseFilter.Length > 0)
        {
            firstLayerValue = _noiseFilter[0].Evaluate(pointOnUnitMap);
            if(_mapSettings.noiseLayers[0].enabled)
            elevation = firstLayerValue;
        } 
        
        Vector3 point = new Vector3();
        if(_localUp == Vector3.up) point = new Vector3(pointOnUnitMap.x,0,pointOnUnitMap.z);
        if(_localUp == Vector3.down) point = new Vector3(pointOnUnitMap.x,0,pointOnUnitMap.z);
        if(_localUp == Vector3.left) point = new Vector3(0,pointOnUnitMap.y,pointOnUnitMap.z);
        if(_localUp == Vector3.right) point = new Vector3(0,pointOnUnitMap.y,pointOnUnitMap.z);
        if(_localUp == Vector3.forward) point = new Vector3(pointOnUnitMap.x,pointOnUnitMap.y,0);
        if(_localUp == Vector3.back) point = new Vector3(pointOnUnitMap.x,pointOnUnitMap.y, 0);
        //evaluate for the rest of the noise filters
        for(var i = 1; i < _noiseFilter.Length; i++)
        {
            if(_mapSettings.noiseLayers[i].enabled)
            {
                // check to use first layer as mask or not
                float mask = (_mapSettings.noiseLayers[i].useFirstLayerAsMask) ? firstLayerValue : 1;
                elevation += _noiseFilter[i].Evaluate(point) * mask;
            }
        }

        elevation = _mapSettings.planetRadius * (1+ elevation);
        
        if(_localUp == Vector3.up) point = new Vector3(pointOnUnitMap.x,elevation,pointOnUnitMap.z);
        if(_localUp == Vector3.down) point = new Vector3(pointOnUnitMap.x,-elevation,pointOnUnitMap.z);
        if(_localUp == Vector3.left) point = new Vector3(-elevation,pointOnUnitMap.y,pointOnUnitMap.z);
        if(_localUp == Vector3.right) point = new Vector3(elevation,pointOnUnitMap.y,pointOnUnitMap.z);
        if(_localUp == Vector3.forward) point = new Vector3(pointOnUnitMap.x,pointOnUnitMap.y,elevation);
        if(_localUp == Vector3.back) point = new Vector3(pointOnUnitMap.x,pointOnUnitMap.y, -elevation);
        return point;
    }

    public Vector3 CalculatePointOnScaledSphere(Vector3 pointOnUnitSphere)
    {
        float firstLayerValue = 0;        
        float elevation = 0;

        // evaluate for first noise filter
        if(_noiseFilter.Length > 0)
        {
            firstLayerValue = _noiseFilter[0].Evaluate(pointOnUnitSphere);
            if(_mapSettings.noiseLayers[0].enabled)
            elevation = firstLayerValue;
        } 

        //evaluate for the rest of the noise filters
        for(var i = 1; i < _noiseFilter.Length; i++)
        {
            if(_mapSettings.noiseLayers[i].enabled)
            {
                // check to use first layer as mask or not
                float mask = (_mapSettings.noiseLayers[i].useFirstLayerAsMask) ? firstLayerValue : 1;
                elevation += _noiseFilter[i].Evaluate(pointOnUnitSphere) * mask;
            }
        }

        elevation = _mapSettings.planetRadius * (1+ elevation);
        
        return pointOnUnitSphere * elevation;
    }

    Vector2 GetBiomeInfo (float height, MapSettings.Biome[] biomes) {
        // Find current biome
        int biomeIndex = 0;
        float biomeStartHeight = 0;
        for (int i = 0; i < biomes.Length; i++) {
            if (height <= biomes[i].height) {
                biomeIndex = i;
                break;
            }
            biomeStartHeight = biomes[i].height;
        }

        MapSettings.Biome biome = biomes[biomeIndex];
        float sampleT = Mathf.InverseLerp (biomeStartHeight, biome.height, height);
        sampleT = (int) (sampleT * biome.numSteps) / (float) Mathf.Max (biome.numSteps, 1);

        // UV stores x: biomeIndex and y: val between 0 and 1 for how close to prev/next biome
        Vector2 uv = new Vector2 (biomeIndex, sampleT);
        return uv;
    }

}


public class MinMax {
    
    public float Min { get; private set; }
    public float Max { get; private set; }

    public MinMax()
    {
        // TODO better way to initialise the min max with starting values
        Min = 1;//float.MinValue;
        Max = 0;//float.MaxValue;
    }

    public void AddValue(float v)
    {
        if(v > Max)
        {
            Max = v;
        }
        if (v < Min)
        {
            Min = v;
        }
    }

    public void ResetValue(float magnitude)
    {
        Min = magnitude;
        Max = magnitude;
    }
}
