using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonCollider : MonoBehaviour {

    public UnityEvent onSpacebar;
    public UnityEvent onReturn;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (onSpacebar != null)
            {
                onSpacebar.Invoke();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (onReturn != null)
            {
                onReturn.Invoke();
            }
        }
    }

    public void OnTriggerStay()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (onSpacebar != null)
            {
                onSpacebar.Invoke();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (onReturn != null)
            {
                onReturn.Invoke();
            }
        }
    }
}