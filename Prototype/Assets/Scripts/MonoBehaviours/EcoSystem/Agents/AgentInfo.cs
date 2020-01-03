using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AgentInfo : MonoBehaviour
{

    public AgentSettings AgentSettings;
    public GameObject uiPanel;                                 // Reference to the UI Panel
    public AgentAction currentAction;
    // Setup Agent on map
    public Coord coord { get; protected set; }
    public int mapIndex { get; set; }
    public Coord mapCoord { get; set; }
    public virtual void SetCoord (Coord coord) {
        this.coord = coord;
        transform.position = EcoManagmentSystem.tileCentres[coord.x, coord.y];
    }
    
}
