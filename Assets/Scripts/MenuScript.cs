using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{

    [SerializeField]
    private GameObject menu;
    
    private GameObject _btnRestart;
    private Button _btnNewGame;
    private Button _btnContinue;
    private Text _gameState;
    
    void Start()
    {
        _btnRestart = menu.transform.Find("BtnRestart").gameObject;
        _btnNewGame = menu.transform.Find("BtnNewGame").GetComponent<Button>();
        _btnContinue = menu.transform.Find("BtnContinue").GetComponent<Button>();
        _gameState = menu.transform.Find("GameState").GetComponent<Text>();

        menu.SetActive(false);

        _btnRestart.GetComponent<Button>().onClick.AddListener(Restart);
        _btnNewGame.GetComponent<Button>().onClick.AddListener(Restart);
        _btnContinue.GetComponent<Button>().onClick.AddListener(Continue);
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalVariables.GameOver)
        {
            menu.SetActive(true);
            _gameState.text = "Game Over";
            _btnRestart.SetActive(false);
            _btnContinue.gameObject.SetActive(false);
            
            _btnNewGame.gameObject.SetActive(true);
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
                
            if (GlobalVariables.Running)
            {
                menu.SetActive(true);
                _btnRestart.SetActive(true);
                _btnContinue.gameObject.SetActive(true);
                
                _btnNewGame.gameObject.SetActive(false);

                GlobalVariables.Running = false;
            }
            else
            {
                Continue();
            }
        }
    }

    public void Restart()
    {
        // SceneManager.LoadScene(1);
    }

    public void Continue()
    {
        menu.SetActive(false);
        GlobalVariables.Running = true;
    }
}
