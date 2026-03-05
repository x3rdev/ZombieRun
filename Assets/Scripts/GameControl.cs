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
          yield return new WaitForSeconds(2);
          Vector3 zombiePos = new Vector3(
              UnityEngine.Random.Range(-10f, 10f),
              0, 
              20
          );
          Vector3 wallPos = new Vector3(
              5,
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
