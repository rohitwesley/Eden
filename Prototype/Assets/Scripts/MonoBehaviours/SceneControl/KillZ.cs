using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillZ : SceneController
{
    


    private void OnTriggerEnter(Collider other) {
        
        ResetScreen();
        
    }

}
