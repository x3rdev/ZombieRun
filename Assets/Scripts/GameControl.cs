using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class GameControl : MonoBehaviour
{
    public static GameControl Instance;

    private bool gameRunning = false;
    private int killCount = 0;

    [SerializeField] public TextMeshProUGUI killCountText;
    [SerializeField] public GameObject playButton;
    [SerializeField] public GameObject mainMenuBackground;
    [SerializeField] public TextMeshProUGUI loseText;
    [SerializeField] public TextMeshProUGUI winText;
    [SerializeField] public TextMeshProUGUI playButtonText;
    [SerializeField] public GameObject soldier;
    [SerializeField] public GameObject zombiePrefab;
    [SerializeField] public GameObject wallPrefab;

    void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ui setup
        killCountText.gameObject.SetActive(false);
        playButtonText.text = "Play";
        
        if (mainMenuBackground != null) mainMenuBackground.SetActive(true);
        playButton.SetActive(true);
        
        loseText.gameObject.SetActive(false);
        winText.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameRunning)
        {   
            
        }
    }

    public void StartGame()
    {
        // ui: setting up for gameplay

        // Initialize Squad
        
        gameRunning = true;
        killCount = 0;
        killCountText.text = "Kill Count: " + killCount;
        killCountText.gameObject.SetActive(true);
        if (mainMenuBackground != null) mainMenuBackground.SetActive(false);
        playButton.SetActive(false);
        loseText.gameObject.SetActive(false);
        winText.gameObject.SetActive(false);
        StartCoroutine(SpawnObjectLoop());
    }

    public void WinGame()
    {
        // ui: setting up for win
        playButtonText.text = "Play Again";
        if (mainMenuBackground != null) mainMenuBackground.SetActive(true);
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
        if (mainMenuBackground != null) mainMenuBackground.SetActive(true);
        playButton.SetActive(true);
        loseText.gameObject.SetActive(true);
        winText.gameObject.SetActive(false);
    }

    public IEnumerator SpawnObjectLoop()
    {
      while (true) 
      {
          // Decrease spawn rate to every 4 seconds
          yield return new WaitForSeconds(8);
          
          Vector3 zombiePos = new Vector3(
              UnityEngine.Random.Range(-10f, 10f),
              0, 
              20
          );
          
          // Randomly choose left (-5) or right (5) side for the wall
          float wallX = UnityEngine.Random.Range(0, 2) == 0 ? -5f : 5f;
          
          Vector3 wallPos = new Vector3(
              wallX,
              1.5F, 
              30
          );
          Instantiate(zombiePrefab, zombiePos, Quaternion.AngleAxis(180, Vector3.up));
          GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.AngleAxis(0, Vector3.up));
          MultiplierWall wallScript = wall.GetComponent<MultiplierWall>();

          if (wallScript != null)
          {
              // Simple random range for additive/subtractive values
              // Range from -5 to 5, excluding 0 to be interesting
              int val = UnityEngine.Random.Range(-5, 6);
              if (val == 0) val = 1; 
              
              wallScript.value = val;
              wallScript.speed = 5;
              wallScript.UpdateVisuals(); 
          }
      }
    }
}
