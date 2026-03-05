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
    
    // Squad Management
    public List<GameObject> soldiers = new List<GameObject>();
    [Header("Squad Separation")]
    public float separationDistance = 1.0f; 
    public float separationForce = 2.0f;

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
            float horizontalInput = 0;
            if (Keyboard.current != null)
            {
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                    horizontalInput = -1;
                else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                    horizontalInput = 1;
            }

            // Move the entire squad
            // 1. Calculate center of mass for camera tracking if needed (optional)
            
            // 2. Apply movement and separation
            for (int i = 0; i < soldiers.Count; i++)
            {
                GameObject member = soldiers[i];
                if (member == null) continue;

                // Player Input Movement
                Vector3 moveDir = new Vector3(horizontalInput * 5, 0, 0);

                // Separation Logic (Boids-like)
                Vector3 separation = Vector3.zero;
                foreach (GameObject other in soldiers)
                {
                    if (member == other || other == null) continue;

                    float dist = Vector3.Distance(member.transform.position, other.transform.position);
                    if (dist < separationDistance)
                    {
                        Vector3 pushDir = member.transform.position - other.transform.position;
                        // Push away stronger if closer
                        separation += pushDir.normalized / (dist + 0.001f); 
                    }
                }

                // Apply both Input + Separation
                // We keep Y at 0 (or original Y) to prevent floating/sinking
                Vector3 finalMove = moveDir + (separation * separationForce);
                
                // Restrict Z movement slightly so they don't fall too far behind/ahead? 
                // For now, let them float naturally in X/Z to form a blob.
                
                member.transform.Translate(finalMove * Time.deltaTime, Space.World);
                
                // Clamp specific constraints if needed
                // e.g. Keep Z within -2 to +2 of leader?
                // For now, just keeping them near 0 Y level
                Vector3 pos = member.transform.position;
                pos.y = 0; 
                member.transform.position = pos;
            }
        }
    }

    public void StartGame()
    {
        // ui: setting up for gameplay

        // Initialize Squad
        soldiers.Clear();
        soldiers.Add(soldier);
        
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

    public void ApplyMathGate(int value)
    {
        if (soldiers.Count == 0) return;

        int currentCount = soldiers.Count;
        int newCount = currentCount + value; // Just add the value (negative subtracts)

        if (newCount < 1) newCount = 1; // Minimum 1 for now, or 0 for Game Over

        if (newCount > currentCount)
        {
            int toSpawn = newCount - currentCount;
            for (int i = 0; i < toSpawn; i++)
            {
                // Spawn near the lead soldier with a slight random offset to prevent perfect stacking
                Vector3 spawnPos = soldiers[0].transform.position + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0, UnityEngine.Random.Range(-0.5f, 0.5f));
                GameObject newSoldier = Instantiate(soldier, spawnPos, Quaternion.identity);
                soldiers.Add(newSoldier);
            }
        }
        else if (newCount < currentCount)
        {
            int toRemove = currentCount - newCount;
            for (int i = 0; i < toRemove; i++)
            {
                if (soldiers.Count > 1) // Keep at least one alive ideally, or handle Game Over
                {
                    GameObject obj = soldiers[soldiers.Count - 1];
                    soldiers.RemoveAt(soldiers.Count - 1);
                    Destroy(obj);
                }
            }
        }
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
