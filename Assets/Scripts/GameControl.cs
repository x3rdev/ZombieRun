using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{  
    private bool gameRunning = false;
    private int killCount = 0;
    [SerializeField] public TextMeshProUGUI killCountText;
    [SerializeField] public GameObject playButton;
    [SerializeField] public TextMeshProUGUI loseText;
    [SerializeField] public TextMeshProUGUI winText;
    [SerializeField] public TextMeshProUGUI playButtonText;

    [SerializeField] public GameObject soldierPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ui setup
        killCountText.gameObject.SetActive(false);
        playButtonText.text = "Play";
        playButton.SetActive(true);
        loseText.gameObject.SetActive(false);
        winText.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameRunning)
        {   
            float horizontalInput = 0;
            if (horizontalInput == 0 && Keyboard.current != null)
            {
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                    horizontalInput = -1;
                else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                    horizontalInput = 1;
            }

            soldierPrefab.transform.Translate(horizontalInput * Time.deltaTime, 0, 0);
        }
    }

    public void StartGame()
    {
        // ui: setting up for gameplay
        gameRunning = true;
        killCount = 0;
        killCountText.text = "Kill Count: " + killCount;
        killCountText.gameObject.SetActive(true);
        playButton.SetActive(false);
        loseText.gameObject.SetActive(false);
        winText.gameObject.SetActive(false);
    }

    public void WinGame()
    {
        // ui: setting up for win
        playButtonText.text = "Play Again";
        playButton.SetActive(true);
        loseText.gameObject.SetActive(false);
        winText.gameObject.SetActive(true);
    }

    public void IncrementKillCount()
    {
        // ui: increment kill count and update text
        killCount++;
        killCountText.text = "Kill Count: " + killCount;
        killCountText.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        // ui: setting up for lose
        playButtonText.text = "Play Again";
        playButton.SetActive(true);
        loseText.gameObject.SetActive(true);
        winText.gameObject.SetActive(false);
    }
}
