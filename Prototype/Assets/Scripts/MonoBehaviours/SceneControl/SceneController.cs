using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

// This script exists in the Persistent scene and manages the content
// based scene's loading.  It works on a principle that the
// Persistent scene will be loaded first, then it loads the scenes that
// contain the player and other visual elements when they are needed.
// At the same time it will unload the scenes that are not needed when
// the player leaves them.
public class SceneController : MonoBehaviour
{
    
    [SerializeField] private KeyCode _EscapeKey = KeyCode.Escape;
    [SerializeField] private KeyCode _DebugMapKey = KeyCode.F1;
    [SerializeField] private KeyCode _DebugStoryboardKey = KeyCode.F2;
    [SerializeField] private GameObject _UIBackgroundScreen;
    [SerializeField] private GameObject _MenuScreen;
    [SerializeField] private GameObject _DebugMapScreen;
    [SerializeField] private GameObject _DebugStoryboardScreen;
    [SerializeField] private EcoManagmentSystem _EcoSystem;

    public event Action BeforeSceneUnload;          // Event delegate that is called just before a scene is unloaded.
    public event Action AfterSceneLoad;             // Event delegate that is called just after a scene is loaded.
    [SerializeField] private KeyCode _characterNextKey = KeyCode.Alpha1;
    [SerializeField] private KeyCode _characterPreviousKey = KeyCode.Alpha2;
    [SerializeField] private GameObject[] _cameras;
    int _currentIndex = 0;
    GameObject _currentCamera = null;

    public CanvasGroup faderCanvasGroup;            // The CanvasGroup that controls the Image used for fading to black.
    public float fadeDuration = 1f;                 // How long it should take to fade to and from black.
    public string startingSceneName = "Scene_Gameplay";
                                                    // The name of the scene that should be loaded first.
    public string initialStartingPositionName = "Spawnpoint_TreeOfLife";
                                                    // The name of the StartingPosition in the first scene to be loaded.
    public SaveData playerSaveData;                 // Reference to the ScriptableObject which stores the name of the StartingPosition in the next scene.
    
    
    private bool isFading;                          // Flag used to determine if the Image is currently fading to or from black.


    private IEnumerator Start ()
    {
        // Set the initial alpha to start off with a black screen.
        faderCanvasGroup.alpha = 1f;

        // Write the initial starting position to the playerSaveData so it can be loaded by the player when the first scene is loaded.
        playerSaveData.Save (PlayerPointNClickMovement.startingPositionKey, initialStartingPositionName);
        
        // Start the first scene loading and wait for it to finish.
        yield return StartCoroutine (LoadSceneAndSetActive (startingSceneName));

        // Once the scene is finished loading, start fading in.
        StartCoroutine (Fade (0f));
        StartCoroutine(ResetScreen());

        // Close Debug Screen
        _DebugMapScreen.SetActive(false);
    }

    private void Update()
    {
        // Debug.Log("scenecontroller");
        if (Input.GetKeyDown(_characterNextKey) && _currentIndex<_cameras.Length-1)
        {
            SetCurrentCamera(_currentIndex+1);
        }
        if (Input.GetKeyDown(_characterPreviousKey) && _currentIndex>0)
        {

            SetCurrentCamera(_currentIndex-1);
        }

        if(Input.GetKeyDown(_EscapeKey))
        {
            StartCoroutine(ResetScreen());
        }

        if(Input.GetKeyDown(_DebugMapKey))
        {
            StartCoroutine(DebugMapScreen());
        }

        if(Input.GetKeyDown(_DebugStoryboardKey))
        {
            StartCoroutine(DebugStoryboardScreen());
        }
    }

    private void SetCurrentCamera(int index)
    {
        if(_currentCamera == null)
        {
            _currentIndex = 0;
            _currentCamera = _cameras[_currentIndex];
            _currentCamera.SetActive(true);
        }
        if(_currentIndex>= _cameras.Length)
        _currentIndex = 0;
        _currentCamera.SetActive(false);
        _currentCamera = _cameras[index];
        _currentCamera.SetActive(true);
        _currentIndex = index;
        Debug.Log(_currentIndex);
    }

    // This is the main external point of contact and influence from the rest of the project.
    // This will be called by a SceneReaction when the player wants to switch scenes.
    public void FadeAndLoadScene (SceneReaction sceneReaction)
    {
        // If a fade isn't happening then start fading and switching scenes.
        if (!isFading)
        {
            StartCoroutine (FadeAndSwitchScenes (sceneReaction.sceneName));
        }
    }

    // This is the coroutine where the 'building blocks' of the script are put together.
    private IEnumerator FadeAndSwitchScenes (string sceneName)
    {
        // Start fading to black and wait for it to finish before continuing.
        yield return StartCoroutine (Fade (1f));

        // If this event has any subscribers, call it.
        if (BeforeSceneUnload != null)
            BeforeSceneUnload ();

        // Unload the current active scene.
        yield return SceneManager.UnloadSceneAsync (SceneManager.GetActiveScene ().buildIndex);

        // Start loading the given scene and wait for it to finish.
        yield return StartCoroutine (LoadSceneAndSetActive (sceneName));

        // If this event has any subscribers, call it.
        if (AfterSceneLoad != null)
            AfterSceneLoad ();
        
        // Start fading back in and wait for it to finish before exiting the function.
        yield return StartCoroutine (Fade (0f));
    }


    public void FadeAndLoadScene (string sceneName)
    {
        // If a fade isn't happening then start fading and switching scenes.
        if (!isFading)
        {
            StartCoroutine (FadeAndSwitchScenes (sceneName));
        }
    }

    public IEnumerator LoadSceneAndSetActive (string sceneName)
    {
        // Allow the given scene to load over several frames and add it to the already loaded scenes (just the Persistent scene at this point).
        yield return SceneManager.LoadSceneAsync (sceneName, LoadSceneMode.Additive);

        // Find the scene that was most recently loaded (the one at the last index of the loaded scenes).
        Scene newlyLoadedScene = SceneManager.GetSceneAt (SceneManager.sceneCount - 1);

        // Set the newly loaded scene as the active scene (this marks it as the one to be unloaded next).
        SceneManager.SetActiveScene (newlyLoadedScene);
    }


    private IEnumerator Fade (float finalAlpha)
    {
        // Set the fading flag to true so the FadeAndSwitchScenes coroutine won't be called again.
        isFading = true;

        // Make sure the CanvasGroup blocks raycasts into the scene so no more input can be accepted.
        faderCanvasGroup.blocksRaycasts = true;

        // Calculate how fast the CanvasGroup should fade based on it's current alpha, it's final alpha and how long it has to change between the two.
        float fadeSpeed = Mathf.Abs (faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

        // While the CanvasGroup hasn't reached the final alpha yet...
        while (!Mathf.Approximately (faderCanvasGroup.alpha, finalAlpha))
        {
            // ... move the alpha towards it's target alpha.
            faderCanvasGroup.alpha = Mathf.MoveTowards (faderCanvasGroup.alpha, finalAlpha,
                fadeSpeed * Time.deltaTime);

            // Wait for a frame then continue.
            yield return null;
        }

        // Set the flag to false since the fade has finished.
        isFading = false;

        // Stop the CanvasGroup from blocking raycasts so input is no longer ignored.
        faderCanvasGroup.blocksRaycasts = false;
    }

    public void Quit()
    {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public IEnumerator ResetScreen()
    {
        // Reload the level that is currently loaded.
        _MenuScreen.SetActive(true);
        _UIBackgroundScreen.SetActive(true);
        _EcoSystem.resetPlayer();
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        yield return new WaitForSeconds(1);
    }

    public IEnumerator DebugMapScreen()
    {
        // Toggle Debug Screen
        _DebugMapScreen.SetActive(!_DebugMapScreen.activeSelf);
        // _UIBackgroundScreen.SetActive(true);
        yield return new WaitForSeconds(1);
    }

    public IEnumerator DebugStoryboardScreen()
    {
        // Toggle Debug Screen
        _DebugStoryboardScreen.SetActive(!_DebugStoryboardScreen.activeSelf);
        // _UIBackgroundScreen.SetActive(true);
        yield return new WaitForSeconds(1);
    }

}
