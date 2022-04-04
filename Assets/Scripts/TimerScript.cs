using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    // Start is called before the first frame update
    private float _timeElapsed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!GlobalVariables.Running || GlobalVariables.GameOver) return;
        
        _timeElapsed += Time.deltaTime;
        if (!(_timeElapsed >= 1f)) return;
        
        _timeElapsed %= 1f;
        Stats.GameTime = Stats.GameTime.AddSeconds(1);
        // timerText.text = (dateTime.Minute < 10 ? "0" + dateTime.Minute : dateTime.Minute.ToString()) + ":" + (dateTime.Second < 10 ? "0" + dateTime.Second : dateTime.Second.ToString());
    }
}
