using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

//TODO redraw everything effectivly using rawImage to stream data
public class MapDisplay : MonoBehaviour
{
    enum Modes{
        Picker,
        Draw,
        Erase
    }


    [Header("Player Controles")]
    [SerializeField] KeyCode _DrawMapKey = KeyCode.Return;

    [SerializeField] Image _colorPreview;
    [SerializeField] Modes _mode;
    [SerializeField] Color _drawColor = Color.red;
    [SerializeField] Renderer _textureRender;
    [SerializeField] Texture2D _mapDraw;
    [SerializeField] RawImage _mapRaw;
    Vector2 _mousePos = new Vector2();
    RectTransform _rect;
    int _width = 0;
    int _height = 0;

    [HideInInspector]
    public bool foldout;
    public MapSettings _mapSettings;
    public INoiseFilter[] noiseFilter;

    private void Initialize()
    {
        _rect = _mapRaw.GetComponent<RectTransform>();
        _width = (int) _mapSettings.resolution;
        _height = (int) _mapSettings.resolution;

        // _mapDraw = _mapRaw.texture as Texture2D;
        // var srcMap = _mapRaw.texture as Texture2D;
        
        //Convert image to 32 bit;
        _mapDraw = new Texture2D(_mapSettings.resolution, _mapSettings.resolution);
        _mapDraw.filterMode = FilterMode.Point;
        // _mapDraw.SetPixels32(srcMap.GetPixels32());
        // _mapDraw.Apply();

        var pixelData = _mapDraw.GetPixels();

        Debug.Log("Map Size : " + pixelData.Length);

        var colorIndex = new List<Color>();
        var total = pixelData.Length;
        for(var i = 0; i < total; i++)
        {
            var color = pixelData[i];
            if(colorIndex.IndexOf(color) == -1)
            {
                colorIndex.Add(color);
            }
        }
        Debug.Log("Total Color Values : " + colorIndex.Count);

        // foreach(var color in colorIndex){
        //     Debug.Log("Color Value : " + color + "Hexa : " + "#" + ColorUtility.ToHtmlStringRGBA(color));
        // }

        noiseFilter = new INoiseFilter[_mapSettings.noiseLayers.Length];
        for(var i = 0; i < noiseFilter.Length; i++)
        {
            if(noiseFilter[i] == null)
                noiseFilter[i] = NoiseFilterGenerator.CreateNoiseFilter(_mapSettings.noiseLayers[i].noiseSettings);

        }

    }

    private void Update()
    {
        // RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, Input.mousePosition, Camera.main, out _mousePos);
        // Debug.Log("Map Display Update");

        // // Reset Mouse to text co-ordinate (0.0) - (1,1) from (-w,-w) - (+h. +h)
        // _mousePos.x = _width - (_width / 2 - _mousePos.y);
        // if(_mousePos.x > _width || _mousePos.x < 0)
        // {
        //     _mousePos.x = -1;
        // }
        // _mousePos.y = Mathf.Abs((_height / 2 - _mousePos.y) - _height);
        // if(_mousePos.y > _height || _mousePos.y < 0)
        // {
        //     _mousePos.y = -1;
        // }

        // Debug.Log("Mouse X : " + _mousePos.x + "Mouse Y : " + _mousePos.y);
        
        if(Input.GetMouseButton(0))
        {
            if(_mode == Modes.Picker)
            {
                if(_mousePos.x > -1 && _mousePos.y > -1)
                {
                    var color = _mapDraw.GetPixel((int)_mousePos.x, (int) _mousePos.y);
                    _colorPreview.color = new Color(color.r, color.g, color.b, color.a);
                }
                else
                {
                    _colorPreview.color = Color.magenta;
                }

            }
            else if(_mode == Modes.Draw)
            {
                _mapDraw.SetPixel((int)_mousePos.x, (int) _mousePos.y, new Color( _drawColor.r, _drawColor.g, _drawColor.b, _drawColor.a));
                _mapDraw.Apply();
            }
            else if(_mode == Modes.Erase)
            {
                _mapDraw.SetPixel((int)_mousePos.x, (int) _mousePos.y, Color.magenta);
                _mapDraw.Apply();
            }


        }
        
        if(Input.GetKeyDown(_DrawMapKey))
        {
            DrawMap();
            updateDisplay();
            Debug.Log("Map Drawn and Updated");
        }

    }
    
    public void Clear()
    {
        var pixelData = _mapDraw.GetPixels();
        var total = pixelData.Length;

        for(var i =0; i < total; i++)
        {
            pixelData[i] = Color.magenta;
        }
        _mapDraw.SetPixels(pixelData);
        _mapDraw.Apply();
    }

    public void Save()
    {
        var bytes = _mapDraw.EncodeToPNG();

        var dirPath = Application.dataPath + "/../SaveMaps/";
        if(!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        File.WriteAllBytes(dirPath + "EdenMap_" + timeStamp + ".png", bytes);


    }
    
    public void DrawMap()
    {
        // _mapSettings.mapNoise = new Texture2D(_mapSettings.resolution,_mapSettings.resolution);
        Color[] colorMap = new Color[_mapSettings.resolution * _mapSettings.resolution];

        for(var y = 0; y < _mapSettings.resolution; y++)
        {
            for(var x = 0; x < _mapSettings.resolution; x++)
            {
                
                // Terrain data:
                // _mapSettings.terrainData.tileCentres[x, y];
                // _mapSettings.terrainData.walkable[x, y];
                // Vertex Index across the plane from top left to bottom right.
                int i = x + y * _mapSettings.resolution;
                colorMap[i] = new Vector4(_mapSettings.terrainData.tileCentres[x, y].x,
                                            _mapSettings.terrainData.tileCentres[x, y].y,
                                            _mapSettings.terrainData.tileCentres[x, y].z,
                                            1.0f);
                colorMap[i] = Color.Lerp(Color.black, Color.white, _mapSettings.terrainData.walkable[x, y] ? 1.0f : 0.0f);
            }
        }

        _mapDraw.SetPixels(colorMap);
        _mapDraw.Apply();
        Debug.Log("Map Drawn");
        
    }

    void updateDisplay()
    {
        // _mapRaw.texture = _mapDraw;
        _textureRender.sharedMaterial.mainTexture = _mapDraw;
        // _textureRender.transform.localScale = new Vector3(_mapSettings.resolution, 1, _mapSettings.resolution);
        // _textureRender.transform.localScale = new Vector3(1, 1, 1);
    }
}

