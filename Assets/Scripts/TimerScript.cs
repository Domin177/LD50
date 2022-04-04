using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    [SerializeField]
    private GameObject timer;

    private Text timerText;
    // Start is called before the first frame update
    private float _timeElapsed;
    void Start()
    {
        timerText = timer.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GlobalVariables.Running || GlobalVariables.GameOver) return;
        
        _timeElapsed += Time.deltaTime;
        if (!(_timeElapsed >= 1f)) return;
        
        _timeElapsed %= 1f;
        Stats.GameTime = Stats.GameTime.AddSeconds(1);
        timerText.text = (Stats.GameTime.Minute < 10 ? "0" + Stats.GameTime.Minute : Stats.GameTime.Minute.ToString()) + ":" + (Stats.GameTime.Second < 10 ? "0" + Stats.GameTime.Second : Stats.GameTime.Second.ToString());
    }
}
