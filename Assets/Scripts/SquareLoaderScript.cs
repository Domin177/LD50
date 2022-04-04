using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareLoaderScript : MonoBehaviour
{
    private float _cooldown = 3f;

    private RectTransform _transform;

    private float _minTop = 93.98f;
    private float _maxTop = 0;

    private float _nextAvailability = 0f;
    private float _startCooldown;

    private void Start()
    {
        _transform = GetComponent<RectTransform>();
        _transform.offsetMax = new Vector2(_transform.offsetMax.x, -_minTop);
    }

    private void Update()
    {
        if (Time.time >= _nextAvailability)
        {
            _transform.offsetMax = new Vector2(_transform.offsetMax.x, -_minTop);
        }
        else
        {
            _transform.offsetMax = new Vector2(_transform.offsetMax.x, -CalculateTop());
        }
    }

    public void SetCooldown()
    {
        float time = Time.time;
        _transform.offsetMax = new Vector2(_transform.offsetMax.x, 0);
        _nextAvailability = time + _cooldown - 1 * 0.15f;
        _startCooldown = time;
    }

    private float CalculateTop()
    {
        float result = ((Time.time - _startCooldown) / (_nextAvailability - _startCooldown) * _minTop);
        
        return result;
    }

    public bool IsAvailable()
    {
        return Time.time > _nextAvailability;
    }
}
