using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BezierCurvePath {

    [SerializeField, HideInInspector]
    List<Vector3> _points;
    [SerializeField, HideInInspector]
    bool _isClosed;
    [SerializeField, HideInInspector]
    bool _autoSetControlPoints;

    public BezierCurvePath(Vector3 centre)
    {
        _points = new List<Vector3>
        {
            centre+Vector3.left,
            centre+(Vector3.left+Vector3.up)*.5f,
            centre + (Vector3.right+Vector3.down)*.5f,
            centre + Vector3.right
        };
    }

    public Vector3 this[int i]
    {
        get
        {
            return _points[i];
        }
    }

    public bool IsClosed
    {
        get
        {
            return _isClosed;
        }
        set
        {
            if (_isClosed != value)
            {
                _isClosed = value;

				if (_isClosed)
				{
					_points.Add(_points[_points.Count - 1] * 2 - _points[_points.Count - 2]);
					_points.Add(_points[0] * 2 - _points[1]);
					if (_autoSetControlPoints)
					{
						AutoSetAnchorControlPoints(0);
						AutoSetAnchorControlPoints(_points.Count - 3);
					}
				}
				else
				{
					_points.RemoveRange(_points.Count - 2, 2);
					if (_autoSetControlPoints)
					{
						AutoSetStartAndEndControls();
					}
				}
            }
        }
    }

    public bool AutoSetControlPoints
    {
        get
        {
            return _autoSetControlPoints;
        }
        set
        {
            if (_autoSetControlPoints != value)
            {
                _autoSetControlPoints = value;
                if (_autoSetControlPoints)
                {
                    AutoSetAllControlPoints();
                }
            }
        }
    }

    public int NumPoints
    {
        get
        {
            return _points.Count;
        }
    }

    public int NumSegments
    {
        get
        {
            return _points.Count/3;
        }
    }

    public void AddSegment(Vector3 anchorPos)
    {
        _points.Add(_points[_points.Count - 1] * 2 - _points[_points.Count - 2]);
        _points.Add((_points[_points.Count - 1] + anchorPos) * .5f);
        _points.Add(anchorPos);

        if (_autoSetControlPoints)
        {
            AutoSetAllAffectedControlPoints(_points.Count - 1);
        }
    }

    public void SplitSegment(Vector3 anchorPos, int segmentIndex)
    {
        _points.InsertRange(segmentIndex * 3 + 2, new Vector3[] { Vector3.zero, anchorPos, Vector3.zero });
        if (_autoSetControlPoints)
        {
            AutoSetAllAffectedControlPoints(segmentIndex * 3 + 3);
        }
        else
        {
            AutoSetAnchorControlPoints(segmentIndex * 3 + 3);
        }
    }

    public void DeleteSegment(int anchorIndex)
    {
        if (NumSegments > 2 || !_isClosed && NumSegments > 1)
        {
            if (anchorIndex == 0)
            {
                if (_isClosed)
                {
                    _points[_points.Count - 1] = _points[2];
                }
                _points.RemoveRange(0, 3);
            }
            else if (anchorIndex == _points.Count - 1 && !_isClosed)
            {
                _points.RemoveRange(anchorIndex - 2, 3);
            }
            else
            {
                _points.RemoveRange(anchorIndex - 1, 3);
            }
        }
    }

    public Vector3[] GetPointsInSegment(int i)
    {
        return new Vector3[] { _points[i * 3], _points[i * 3 + 1], _points[i * 3 + 2], _points[LoopIndex(i * 3 + 3)] };
    }

    public void MovePoint(int i, Vector3 pos)
    {
        Vector3 deltaMove = pos - _points[i];

        if (i % 3 == 0 || !_autoSetControlPoints) {
            _points[i] = pos;

            if (_autoSetControlPoints)
            {
                AutoSetAllAffectedControlPoints(i);
            }
            else
            {

                if (i % 3 == 0)
                {
                    if (i + 1 < _points.Count || _isClosed)
                    {
                        _points[LoopIndex(i + 1)] += deltaMove;
                    }
                    if (i - 1 >= 0 || _isClosed)
                    {
                        _points[LoopIndex(i - 1)] += deltaMove;
                    }
                }
                else
                {
                    bool nextPointIsAnchor = (i + 1) % 3 == 0;
                    int correspondingControlIndex = (nextPointIsAnchor) ? i + 2 : i - 2;
                    int anchorIndex = (nextPointIsAnchor) ? i + 1 : i - 1;

                    if (correspondingControlIndex >= 0 && correspondingControlIndex < _points.Count || _isClosed)
                    {
                        float dst = (_points[LoopIndex(anchorIndex)] - _points[LoopIndex(correspondingControlIndex)]).magnitude;
                        Vector3 dir = (_points[LoopIndex(anchorIndex)] - pos).normalized;
                        _points[LoopIndex(correspondingControlIndex)] = _points[LoopIndex(anchorIndex)] + dir * dst;
                    }
                }
            }
        }
    }

    void AutoSetAllAffectedControlPoints(int updatedAnchorIndex)
    {
        for (int i = updatedAnchorIndex-3; i <= updatedAnchorIndex +3; i+=3)
        {
            if (i >= 0 && i < _points.Count || _isClosed)
            {
                AutoSetAnchorControlPoints(LoopIndex(i));
            }
        }

        AutoSetStartAndEndControls();
    }

    void AutoSetAllControlPoints()
    {
        for (int i = 0; i < _points.Count; i+=3)
        {
            AutoSetAnchorControlPoints(i);
        }

        AutoSetStartAndEndControls();
    }

    void AutoSetAnchorControlPoints(int anchorIndex)
    {
        Vector3 anchorPos = _points[anchorIndex];
        Vector3 dir = Vector3.zero;
        float[] neighbourDistances = new float[2];

        if (anchorIndex - 3 >= 0 || _isClosed)
        {
            Vector3 offset = _points[LoopIndex(anchorIndex - 3)] - anchorPos;
            dir += offset.normalized;
            neighbourDistances[0] = offset.magnitude;
        }
		if (anchorIndex + 3 >= 0 || _isClosed)
		{
			Vector3 offset = _points[LoopIndex(anchorIndex + 3)] - anchorPos;
			dir -= offset.normalized;
			neighbourDistances[1] = -offset.magnitude;
		}

        dir.Normalize();

        for (int i = 0; i < 2; i++)
        {
            int controlIndex = anchorIndex + i * 2 - 1;
            if (controlIndex >= 0 && controlIndex < _points.Count || _isClosed)
            {
                _points[LoopIndex(controlIndex)] = anchorPos + dir * neighbourDistances[i] * .5f;
            }
        }
    }

    void AutoSetStartAndEndControls()
    {
        if (!_isClosed)
        {
            _points[1] = (_points[0] + _points[2]) * .5f;
            _points[_points.Count - 2] = (_points[_points.Count - 1] + _points[_points.Count - 3]) * .5f;
        }
    }

    int LoopIndex(int i)
    {
        return (i + _points.Count) % _points.Count;
    }

    public Vector3[] CalculateEvenlySpacedPoints(float spacing, float resolution = 1)
    {
        List<Vector3> evenlySpacedPoints = new List<Vector3>();
        evenlySpacedPoints.Add(_points[0]);
        Vector3 previousPoint = _points[0];
        float dstSinceLastEvenPoint = 0;

        for (int segmentIndex = 0; segmentIndex < NumSegments; segmentIndex++)
        {
            Vector3[] p = GetPointsInSegment(segmentIndex);
            float controlNetLength = Vector3.Distance(p[0], p[1]) + Vector3.Distance(p[1], p[2]) + Vector3.Distance(p[2], p[3]);
            float estimatedCurveLength = Vector3.Distance(p[0], p[3]) + controlNetLength / 2f;
            int divisions = Mathf.CeilToInt(estimatedCurveLength * resolution * 10);
            float t = 0;
            while (t <= 1)
            {
                t += 1f/divisions;
                Vector3 pointOnCurve = Bezier.EvaluateCubic(p[0], p[1], p[2], p[3], t);
                dstSinceLastEvenPoint += Vector3.Distance(previousPoint, pointOnCurve);

                while (dstSinceLastEvenPoint >= spacing)
                {
                    float overshootDst = dstSinceLastEvenPoint - spacing;
                    Vector3 newEvenlySpacedPoint = pointOnCurve + (previousPoint - pointOnCurve).normalized * overshootDst;
                    evenlySpacedPoints.Add(newEvenlySpacedPoint);
                    dstSinceLastEvenPoint = overshootDst;
                    previousPoint = newEvenlySpacedPoint;
                }

                previousPoint = pointOnCurve;
            }
        }

        return evenlySpacedPoints.ToArray();
    }

}
