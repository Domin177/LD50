using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

public class CollectableScript : MonoBehaviour
{
    [SerializeField] private CollectType type;

    [SerializeField] private int amount;

    private PlayerScript _playerScript;

    private float _minScale;
    private float _maxScale;
    private float _scaleModifier = 0.0005f;
    private bool _increasing = true;

    // Start is called before the first frame update
    void Start()
    {
        _playerScript = GameObject.Find("Player").GetComponent<PlayerScript>();
        _minScale = transform.localScale.x;
        _maxScale = _minScale + 0.05f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GlobalVariables.Running || GlobalVariables.GameOver) return;
        
        Pulse();
    }

    private void Pulse()
    {
        Vector3 scale = transform.localScale;
        if (_increasing && scale.x < _maxScale)
        {
            transform.localScale = new Vector3(scale.x + _scaleModifier, scale.y + _scaleModifier, scale.z);
        } else if (_increasing && scale.x >= _maxScale)
        {
            _increasing = false;
            transform.localScale = new Vector3(scale.x - _scaleModifier, scale.y - _scaleModifier, scale.z);
        } else if (!_increasing && scale.x <= _minScale)
        {
            _increasing = true;
            transform.localScale = new Vector3(scale.x + _scaleModifier, scale.y + _scaleModifier, scale.z);
        }
        else
        {
            transform.localScale = new Vector3(scale.x - _scaleModifier, scale.y - _scaleModifier, scale.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            _playerScript.Collect(type, amount);
            Destroy(gameObject);
        }
    }

    public enum CollectType
    {
        PistolAmmo,
        RifleAmmo,
        ShotgunAmmo,
        Letter,
        HealthPoint,
        RepairKit
    }
}
