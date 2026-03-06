using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Soldier : MonoBehaviour
{
    // Squad Management
    [Header("Effects")]
    public GameObject deathParticlePrefab;
    public List<GameObject> soldiers = new List<GameObject>();
    [Header("Squad Separation")]
    public float separationDistance = 1.0f; 
    public float separationForce = 2.0f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;

    private float nextFireTime = 0f;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();  
        soldiers.Clear();
        soldiers.Add(gameObject);
    }

    void Update()
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
            
            member.GetComponent<Rigidbody>().MovePosition(finalMove * Time.deltaTime);
            
            // Clamp specific constraints if needed
            // e.g. Keep Z within -2 to +2 of leader?
            // For now, just keeping them near 0 Y level
            Vector3 pos = member.transform.position;
            pos.y = 0; 
            member.transform.position = pos;
        }
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
        animator.SetBool("Run", horizontalInput != 0);
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.MovePosition(new Vector3(horizontalInput * 5 * Time.deltaTime, 0, 0));
        
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 90, 90));
        }
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
                GameObject newSoldier = Instantiate(gameObject, spawnPos, Quaternion.identity);
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
    public void DecrementSoldiers()
    {
        // We keep at least 1 (the leader) to avoid a null reference on the script holder
        if (soldiers.Count > 1)
        {
            int lastIndex = soldiers.Count - 1;
            GameObject soldierToDestroy = soldiers[lastIndex];

            // 1. Play Particle Effect at the soldier's position
            if (deathParticlePrefab != null)
            {
                Instantiate(deathParticlePrefab, soldierToDestroy.transform.position, Quaternion.identity);
            }

            // 2. Remove from List and Destroy
            soldiers.RemoveAt(lastIndex);
            Destroy(soldierToDestroy);
        }
        else
        {
            Debug.Log("Only the leader is left!");
            // Optional: Trigger Game Over logic here
        }
    }
}
