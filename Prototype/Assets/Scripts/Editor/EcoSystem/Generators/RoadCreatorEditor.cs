using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoadCreator))]
public class RoadCreatorEditor : Editor {

    RoadCreator creator;

    void OnSceneGUI()
    {
        // if (Event.current.type == EventType.Repaint)
        if (creator.autoUpdate && Event.current.type == EventType.Repaint)
        {
            creator.UpdateRoad();
        }
    }

    void OnEnable()
    {
        creator = (RoadCreator)target;
    }
}
