using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceUI : MonoBehaviour
{
    [SerializeField] GameObject PanelObject;
    [SerializeField] Text titleText;
    [SerializeField] GameObject messageText;
    [SerializeField] string title;
    [SerializeField] string message;
    [SerializeField] Canvas HUDCanvas;

    // Start is called before the first frame update
    void Start()
    {
           messageText = Instantiate(PanelObject, HUDCanvas.transform);
        //    messageText.text = message;
    }

    // Update is called once per frame
    void Update()
    {
        // messageText
    }
}
