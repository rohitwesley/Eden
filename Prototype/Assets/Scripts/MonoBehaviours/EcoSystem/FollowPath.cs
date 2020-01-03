using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    [SerializeField] GameObject _followObject;
    [SerializeField] GameObject _followHolder;
    [SerializeField] private KeyCode _FollowKey = KeyCode.E;
    bool IsGlide = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(_FollowKey))
        {
            Debug.Log("Activate IsGliding");
            IsGlide = true;
            if(_followObject.GetComponent<CharacterController>())_followObject.GetComponent<CharacterController>().enabled = false;
            _followObject.transform.SetPositionAndRotation(new Vector3(0.0f,0.0f,0.0f),Quaternion.identity);
            _followObject.transform.SetParent(_followHolder.transform);
        }
        else if(Input.GetKey(_FollowKey))
        {
            _followObject.transform.SetPositionAndRotation(new Vector3(0.0f,0.0f,0.0f),Quaternion.identity);
            Debug.Log("IsGliding");
            IsGlide = true;
        }
        else if(Input.GetKeyUp(_FollowKey))
        {
            IsGlide = false;
            Debug.Log("Deactivat Gliding");
        //    if(_followObject.GetComponent<CharacterController>()) _followObject.GetComponent<CharacterController>().enabled = true;
            _followObject.transform.SetParent(null);
             _followObject.transform.SetPositionAndRotation(new Vector3(0.0f,0.0f,0.0f),Quaternion.identity);

        }

        
        if(IsGlide)
        {
            
            // _followObject.transform.position = _followHolder.transform.position;
            // _followObject.transform.rotation = _followHolder.transform.rotation;
        }
        // else 
        // {
        //     Debug.Log("Not Gliding");
        //     _followObject.GetComponent<CharacterController>().enabled = true;
        //     _followObject.transform.SetParent(null);
        //      _followObject.transform.SetPositionAndRotation(new Vector3(0.0f,0.0f,0.0f),Quaternion.identity);
            
        // }

    }
}
