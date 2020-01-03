using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenManager : MonoBehaviour
{
    [SerializeField] EventSystem eventSystem;
    [SerializeField] GameObject selectedObject;

    private bool buttonSelected;

    void Update()
    {
        if(Input.GetAxisRaw("Vertical") != 0 && buttonSelected == false)
        {
            eventSystem.SetSelectedGameObject(selectedObject);
            buttonSelected = true;
        }        
    }

    private void OnDisable()
    {
        buttonSelected = false;    
    }
}
