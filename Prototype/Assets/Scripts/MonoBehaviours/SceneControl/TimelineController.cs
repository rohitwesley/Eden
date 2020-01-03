using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineController : MonoBehaviour {

    public List<PlayableDirector> playableDirectors;
    public List<TimelineAsset> timelines;
    
    TimelineAsset selectedAsset;
    int indx = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Minus)){
            indx--;
        }
        else if (Input.GetKeyDown(KeyCode.Equals)){
            indx++;
        }
        if(Input.GetKeyDown(KeyCode.Return) && Input.GetKey(KeyCode.LeftShift))
            PlayAllInSync();
            
        if(Input.GetKeyDown(KeyCode.Return))
            PlayPlayableDirector(indx);

    }

    public void PlayAllInSync()
    {
        foreach (PlayableDirector playableDirector in playableDirectors) 
        {
            playableDirector.Play ();
        }
    }

    public void PlayPlayableDirector(int index)
    {
        playableDirectors [indx].Play();
    }

    public void ActivateTimelines(int index)
    {
        if (timelines.Count <= index) 
        {
            selectedAsset = timelines [timelines.Count - 1];
            indx = 0;
        } 
        else 
        {
            selectedAsset = timelines [index];
        }
    }

}