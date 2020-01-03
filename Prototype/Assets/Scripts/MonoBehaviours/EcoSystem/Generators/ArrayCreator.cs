using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathGenerator))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ArrayCreator : MonoBehaviour
{
    [Range(.05f, 1.5f)]
    [SerializeField] float _spacing = 0.1f;
    [SerializeField] bool _autoUpdate;
    [SerializeField] float _resolution = 1;
    [SerializeField] GameObject _objectType;
    GameObject[] _objectArray;

    public void ResetArray()
    {
        BezierCurvePath path = GetComponent<PathGenerator>().path;
        Vector3[] points = path.CalculateEvenlySpacedPoints(_spacing, _resolution);
        _objectArray = new GameObject[points.Length];
        for(var i = 0; i < points.Length; i++)
        {
            GameObject g;
            if(_objectType != null)
            {
                g = Instantiate(_objectType);
            }
            else
            {
                g = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            }
            g.transform.parent = this.transform;
            g.transform.position = points[i];
            g.transform.localScale = Vector3.one * _spacing * .5f;
            _objectArray[i] = g;
        }
    }
    
    public void UpdateArray()
    {
        BezierCurvePath path = GetComponent<PathGenerator>().path;
        Vector3[] points = path.CalculateEvenlySpacedPoints(_spacing, _resolution);
        for(var i = 0; i < points.Length; i++)
        {
            _objectArray[i].transform.parent = this.transform;
            _objectArray[i].transform.position = points[i];
            _objectArray[i].transform.localScale = Vector3.one * _spacing * .5f;
        }
    }

    private void Start()
    {
        ResetArray();   
    }

    private void Update()
    {
        if(_autoUpdate)
        {
            UpdateArray();
        }
    }

}
