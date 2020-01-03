using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerMovement;
    // [SerializeField] private Player3rdPersonMovement _player3rDMovement;
    // [SerializeField] private PlayerPointNClickMovement _playerClickMovement;
    [SerializeField] private GameObject[] _characters;
    [SerializeField] private GameObject _spawnPoint;
    [SerializeField] private GameObject _cameraTargetPoint;
    [SerializeField] private bool _IsPlayer = true;
    private bool _IsSet = false;
    [SerializeField] private KeyCode _characterNextKey = KeyCode.RightBracket;
    [SerializeField] private KeyCode _characterPreviousKey = KeyCode.LeftBracket;

    int _currentIndex = 0;
    GameObject _currentCharacterType = null;
    GameObject _currentCharacter = null;


    
    void Start()
    {
        if(_spawnPoint != null)
        {
            SetCurrentCharacterType(_currentIndex);
            _IsSet = false;
        }    
    }

    private void Update()
    {
        //TODO find better way to fix on start character is stuck in run state bug
        if(_IsSet == false)
        {

            _IsSet = true;
            StartCoroutine(SetCurrentCharacterType(_currentIndex+1));
            if(_IsPlayer){
                if(_currentCharacter != null) _cameraTargetPoint.transform.rotation = _currentCharacter.transform.rotation;
                if(_currentCharacter != null)_cameraTargetPoint.transform.position = _currentCharacter.transform.position;
            }
            StartCoroutine(SetCurrentCharacterType(_currentIndex-1)); 
            if(_IsPlayer){
                if(_currentCharacter != null) _cameraTargetPoint.transform.rotation = _currentCharacter.transform.rotation;
                if(_currentCharacter != null)_cameraTargetPoint.transform.position = _currentCharacter.transform.position;
            }
        }
        if (Input.GetKeyDown(_characterNextKey) && _currentIndex<_characters.Length-1)
        {
            StartCoroutine(SetCurrentCharacterType(_currentIndex+1));
            if(_IsPlayer){
                if(_currentCharacter != null) _cameraTargetPoint.transform.rotation = _currentCharacter.transform.rotation;
                if(_currentCharacter != null)_cameraTargetPoint.transform.position = _currentCharacter.transform.position;
            }
        }
        if (Input.GetKeyDown(_characterPreviousKey) && _currentIndex>0)
        {

            StartCoroutine(SetCurrentCharacterType(_currentIndex-1));
            if(_IsPlayer){
                if(_currentCharacter != null) _cameraTargetPoint.transform.rotation = _currentCharacter.transform.rotation;
                if(_currentCharacter != null)_cameraTargetPoint.transform.position = _currentCharacter.transform.position;
            }
        }


    }

    IEnumerator SetCurrentCharacterType(int index)
    {
        if(_currentCharacterType != null)
        {
            Destroy(_currentCharacterType.gameObject);
        }

        GameObject character = _characters[index];
        this.transform.position = new Vector3(0.0f,0.0f,0.0f);
        _currentCharacterType = Instantiate<GameObject>(character, this.transform.position, Quaternion.identity);
        _currentCharacterType.transform.SetParent(this.transform);
        this.transform.position = _spawnPoint.transform.position;
        if(_playerMovement.isActiveAndEnabled)_playerMovement.playerAnim =  _currentCharacterType.gameObject.GetComponent<Animator>();
        // if(_player3rDMovement.isActiveAndEnabled)_player3rDMovement._anim =  _currentCharacterType.gameObject.GetComponent<Animator>();
        // if(_playerClickMovement.isActiveAndEnabled)_playerClickMovement.animator =  _currentCharacterType.gameObject.GetComponent<Animator>();
        
        // _cameraTargetPoint.transform.SetParent(_currentCharacterType.transform);
        // _cameraTargetPoint.transform.SetPositionAndRotation(new Vector3(0.0f,0.0f,0.0f), Quaternion.identity);
        _currentIndex = index;

        yield return new WaitForSeconds(1);

    }

    public void SetCurrentCharacterType(string name)
    {
        int idx = 0;
        foreach(GameObject characterInfo in _characters)
        {
            if(characterInfo.name == name)
            {
                SetCurrentCharacterType(idx);
                break;
            }
            idx++;
        }
    }

    public void CreateCurrentCharacter(string name)
    {
        _currentCharacter = Instantiate<GameObject>(_currentCharacterType, _spawnPoint.transform.position, Quaternion.identity);
        _currentCharacter.gameObject.SetActive(false);
        _currentCharacter.name = name;
        DontDestroyOnLoad(_currentCharacter);
        
        SceneManager.LoadScene(1);

    }

    // Update is called once per frame
    public GameObject GetCurrentCharacter()
    {
        return _currentCharacter;
    }

}
