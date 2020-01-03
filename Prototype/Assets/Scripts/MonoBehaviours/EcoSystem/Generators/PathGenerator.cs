using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    [HideInInspector]
    public BezierCurvePath path;

	public Color anchorCol = Color.red;
    public Color controlCol = Color.white;
    public Color segmentCol = Color.green;
    public Color selectedSegmentCol = Color.yellow;
    public float anchorDiameter = .1f;
    public float controlDiameter = .075f;
    public bool displayControlPoints = true;
    public bool autoUpdate;

    public void CreatePath()
    {
        path = new BezierCurvePath(transform.position);
    }
    
    void Reset()
    {
        CreatePath();
    }

    private void Update()
    {
        if(autoUpdate && transform.hasChanged)
        {
            for (int i = 0; i < path.NumPoints; i++)
            {
                if (i % 3 == 0)
                {
                    Vector3 newPos = path[i] + transform.position;
                    // if (path[i] != newPos)
                    // {
                        path.MovePoint(i, newPos);
                    // }
                }
            }

            transform.hasChanged = false;
        }
    }

}
