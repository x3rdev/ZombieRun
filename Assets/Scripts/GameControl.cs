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
        mainMenuBackground.SetActive(false);
        playButton.SetActive(false);
        loseText.gameObject.SetActive(false);
        winText.gameObject.SetActive(false);
        StartCoroutine(SpawnWalls());
        StartCoroutine(SpawnZombies());
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

    public IEnumerator SpawnWalls()
    {
      while (gameRunning) 
      {
          yield return new WaitForSeconds(6);
          if (!gameRunning) yield break;

          // Randomly choose left (-5) or right (5) side for the wall
          float wallX = UnityEngine.Random.Range(0, 2) == 0 ? -5f : 5f;
          
          Vector3 wallPos = new Vector3(
              wallX,
              1.5F, 
              30
          );

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

    public IEnumerator SpawnZombies()
    {
        // Start moderately
        float spawnInterval = 2.5f;
        while (gameRunning)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (!gameRunning) yield break;

            int zombieCount = 1;

            // Increase difficulty: spawn more zombies 
            if (spawnInterval < 2.0f) zombieCount = UnityEngine.Random.Range(1, 3);
            if (spawnInterval < 1.0f) zombieCount = UnityEngine.Random.Range(2, 4);

            for(int i = 0; i < zombieCount; i++)
            {
                // Slight random offset for multiple zombies
                Vector3 zombiePos = new Vector3(
                    UnityEngine.Random.Range(-10f, 10f),
                    0, 
                    20 + (i * 2.0f) 
                );
                Instantiate(zombiePrefab, zombiePos, Quaternion.AngleAxis(180, Vector3.up));
            }

            // Decrease spawn interval over time (5% faster each wave), clamped to 0.5s minimum
            spawnInterval = Mathf.Max(0.5f, spawnInterval * 0.95f);
        }
    }
}
