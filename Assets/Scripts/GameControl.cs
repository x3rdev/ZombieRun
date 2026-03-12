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

          int positiveValue = UnityEngine.Random.Range(1, 6);
          int negativeValue = -UnityEngine.Random.Range(1, 6);
          bool positiveOnLeft = UnityEngine.Random.Range(0, 2) == 0;

          SpawnWallAtX(-5f, positiveOnLeft ? positiveValue : negativeValue);
          SpawnWallAtX(5f, positiveOnLeft ? negativeValue : positiveValue);
      }
    }

    private void SpawnWallAtX(float wallX, int value)
    {
        Vector3 wallPos = new Vector3(wallX, 1.5f, 30f);
        GameObject wall = Instantiate(wallPrefab, wallPos, Quaternion.identity);
        MultiplierWall wallScript = wall.GetComponent<MultiplierWall>();

        if (wallScript == null) return;

        wallScript.value = value;
        wallScript.speed = 5f;
        wallScript.UpdateVisuals();
    }

    public IEnumerator SpawnZombies()
    {
        // Start slower and ramp gently
        float spawnInterval = 3.2f;
        while (gameRunning)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (!gameRunning) yield break;

            int zombieCount = 1;

            // Reduce pressure: at most 2 zombies in late waves
            if (spawnInterval < 1.8f) zombieCount = UnityEngine.Random.Range(1, 3);

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

            // Decrease spawn interval slowly, clamped to a safer minimum
            spawnInterval = Mathf.Max(1.4f, spawnInterval * 0.97f);
        }
    }
}
