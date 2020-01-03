using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timmer : MonoBehaviour
{
    [SerializeField] ItemFloat _timeItem;
    [SerializeField] float _totalGameTime = 600;
    // [SerializeField] Text timmer;
    [SerializeField] GameObject _mainMenu;
    [SerializeField] GameObject _UIBackgroundScreen;
    [SerializeField] GameObject _HUD;

    // Start is called before the first frame update
    void Start()
    {
        ResetTime();
    }

    // Update is called once per frame
    void Update()
    {
        // timeItem.Amount++;
        _timeItem.Amount += Time.deltaTime;
        _timeItem.textPlaceHolder.text = " " + _timeItem.Amount;
        if(_timeItem.Amount>_totalGameTime)
        {
            _timeItem.Amount = 0;
            _HUD.SetActive(false);
            _mainMenu.SetActive(true);
            _UIBackgroundScreen.SetActive(true);
        }
            
    }

    public void ResetTime()
    {
        _timeItem.Amount = 0;
    }

}
