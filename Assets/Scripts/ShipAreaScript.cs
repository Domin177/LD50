using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAreaScript : MonoBehaviour
{
   
    private GameObject _fButton;
    private PlayerScript _playerScript;
    void Start()
    {
        _fButton = GameObject.Find("FButton");
        _fButton.SetActive(false);

        _playerScript = GameObject.Find("Player").GetComponent<PlayerScript>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (_playerScript.LettersHolding() <= 0) return;
            
            _fButton.SetActive(true);
            _playerScript.SendEnabled(true);
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            bool active = _playerScript.LettersHolding() > 0;
            _fButton.SetActive(active);
            _playerScript.SendEnabled(active);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _fButton.SetActive(false);
            _playerScript.SendEnabled(false);
        }
    }
}
