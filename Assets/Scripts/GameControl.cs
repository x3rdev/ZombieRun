using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class GameControl : MonoBehaviour
{  
    private bool gameRunning = false;
    private int killCount = 0;
    [SerializeField] public TextMeshProUGUI killCountText;
    [SerializeField] public GameObject playButton;
    [SerializeField] public TextMeshProUGUI loseText;
    [SerializeField] public TextMeshProUGUI winText;
    [SerializeField] public TextMeshProUGUI playButtonText;

    [SerializeField] public GameObject soldier;

    [SerializeField] public GameObject zombiePrefab;

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
            if (Keyboard.current != null)
            {
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                    horizontalInput = -1;
                else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                    horizontalInput = 1;
            }

            soldier.transform.Translate(horizontalInput * 5 * Time.deltaTime, 0, 0);
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
        StartCoroutine(SpawnObjectLoop());
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

    public IEnumerator SpawnObjectLoop()
    {
      while (true) 
      {
          // 1. Wait for X seconds
          yield return new WaitForSeconds(2);

          // 2. Calculate a random position (so they don't all spawn in one line)
          Vector3 spawnPos = transform.position + new Vector3(
              0, 
              0, 
              20
          );

          // 3. Create the object
          Instantiate(zombiePrefab, spawnPos, Quaternion.AngleAxis(180, Vector3.up));
      }
    }
}
