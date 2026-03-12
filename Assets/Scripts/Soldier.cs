using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Soldier : MonoBehaviour
{
    // Squad Management
    [Header("Effects")]
    public GameObject deathParticlePrefab;
    public List<GameObject> soldiers = new List<GameObject>();
    [Header("Growth")]
    [SerializeField] private int growthStepsToSplit = 10;
    [SerializeField] private float growthScalePerStep = 0.08f;
    [SerializeField] private float growthDiminishFactor = 0.4f;
    [SerializeField] private float growthStepDelay = 0.07f;
    [SerializeField] private float splitDelay = 0.12f;
    [SerializeField] private float splitPopScaleMultiplier = 1.18f;
    [Header("Squad Separation")]
    public float separationDistance = 1.0f; 
    public float separationForce = 2.0f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float moveSpeed = 6f;

    private float nextFireTime = 0f;
    private Animator animator;
    private Rigidbody rb;
    private Vector3 movement;
    private Vector3 baseScale;
    private int growthSteps;
    private int pendingPositiveGrowth;
    private Coroutine growthRoutine;
    private int growthRoundRobinIndex;
    private static Soldier mainSoldier;
    private bool isMainSoldier;

    public int GrowthSteps => growthSteps;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        baseScale = transform.localScale;
        UpdateGrowthVisual();
        if (mainSoldier == null)
        {
            mainSoldier = this;
            isMainSoldier = true;
        }
    }

    void Start()
    {
        soldiers.Clear();
        soldiers.Add(gameObject);
    }

    void Update()
    {
        if (!isMainSoldier) return;

        // 1. Read input
        float horizontalInput = 0;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                horizontalInput = -1;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                horizontalInput = 1;
        }

        // 2. Build movement vector (leader direction)
        movement = new Vector3(horizontalInput, 0, 0).normalized * moveSpeed;

        // 3. Animate
        animator.SetBool("Run", horizontalInput != 0);

        // Shooting handled as before
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FixedUpdate()
    {
        if (!isMainSoldier) return;
        if (soldiers.Count == 0) return;

        bool isRunning = movement.sqrMagnitude > 0.001f;
        Vector3 forward = movement.normalized;
        Vector3 right = Vector3.Cross(Vector3.up, forward);

        foreach (GameObject member in soldiers)
        {
            if (member == null) continue;
            Rigidbody memberRb = member.GetComponent<Rigidbody>();
            if (memberRb == null) continue;

            Animator memberAnimator = member.GetComponent<Animator>();
            if (memberAnimator != null)
            {
                memberAnimator.SetBool("Run", isRunning);
            }

            Vector3 separation = Vector3.zero;
            foreach (GameObject other in soldiers)
            {
                if (member == other || other == null) continue;

                Vector3 toOther = member.transform.position - other.transform.position;
                float dist = toOther.magnitude;
                if (dist < separationDistance)
                {
                    Vector3 lateralPush = Vector3.Project(toOther.normalized / (dist + 0.001f), right);
                    separation += lateralPush;
                }
            }

            float maxSeparation = 1f;
            if (separation.magnitude > maxSeparation)
                separation = separation.normalized * maxSeparation;

            Vector3 finalMove = forward * movement.magnitude + separation * separationForce;
            Vector3 targetPos = memberRb.position + finalMove * Time.fixedDeltaTime;
            memberRb.MovePosition(targetPos);
        }
    }

  void Shoot()
    {
        if (!isMainSoldier) return;
        if (bulletPrefab == null) return;

        foreach (GameObject member in soldiers)
        {
            if (member == null) continue;

            Soldier memberSoldier = member.GetComponent<Soldier>();
            if (memberSoldier == null) continue;

            Transform memberFirePoint = memberSoldier.firePoint != null ? memberSoldier.firePoint : member.transform;
            Vector3 spawnPos = memberFirePoint.position;
            spawnPos.y = 1.5f;
            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.Initialize(Vector3.forward);
            }
        }
    }

    public void ApplyMathGate(int value)
    {
        if (!isMainSoldier && mainSoldier != null)
        {
            mainSoldier.ApplyMathGate(value);
            return;
        }

        if (soldiers.Count == 0) return;

        if (value > 0)
        {
            pendingPositiveGrowth += value;
            if (growthRoutine == null)
            {
                growthRoutine = StartCoroutine(ProcessPositiveGrowth());
            }
        }
        else if (value < 0)
        {
            for (int i = 0; i < Mathf.Abs(value); i++)
            {
                ShrinkSquadOnce();
            }
        }
    }

    public void DecrementSoldiers()
    {
        if (!isMainSoldier && mainSoldier != null)
        {
            mainSoldier.DecrementSoldiers();
            return;
        }

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

    private void GrowSquadOnce()
    {
        GameObject memberToGrow = GetLeastGrownMember();
        if (memberToGrow == null) return;

        Soldier memberSoldier = memberToGrow.GetComponent<Soldier>();
        if (memberSoldier == null) return;

        if (memberSoldier.TryAdvanceGrowth())
        {
            StartCoroutine(SplitAfterDelay(memberToGrow));
        }
    }

    private IEnumerator ProcessPositiveGrowth()
    {
        while (pendingPositiveGrowth > 0)
        {
            pendingPositiveGrowth--;
            GrowSquadOnce();
            yield return new WaitForSeconds(growthStepDelay);
        }

        growthRoutine = null;
    }

    private void ShrinkSquadOnce()
    {
        GameObject memberToShrink = GetMostGrownMember();
        if (memberToShrink != null)
        {
            Soldier memberSoldier = memberToShrink.GetComponent<Soldier>();
            if (memberSoldier != null && memberSoldier.TryReduceGrowth())
            {
                return;
            }
        }

        DecrementSoldiers();
    }

    private GameObject GetMostGrownMember()
    {
        GameObject bestMember = null;
        int highestGrowth = int.MinValue;

        foreach (GameObject member in soldiers)
        {
            if (member == null) continue;

            Soldier memberSoldier = member.GetComponent<Soldier>();
            if (memberSoldier == null) continue;

            if (memberSoldier.GrowthSteps > highestGrowth)
            {
                highestGrowth = memberSoldier.GrowthSteps;
                bestMember = member;
            }
        }

        return bestMember;
    }

    private GameObject GetLeastGrownMember()
    {
        if (soldiers.Count == 0) return null;

        int count = soldiers.Count;
        int startIndex = ((growthRoundRobinIndex % count) + count) % count;
        int bestIndex = -1;
        int lowestGrowth = int.MaxValue;

        for (int i = 0; i < count; i++)
        {
            int index = (startIndex + i) % count;
            GameObject member = soldiers[index];
            if (member == null) continue;

            Soldier memberSoldier = member.GetComponent<Soldier>();
            if (memberSoldier == null) continue;

            if (memberSoldier.GrowthSteps < lowestGrowth)
            {
                lowestGrowth = memberSoldier.GrowthSteps;
                bestIndex = index;
            }
        }

        if (bestIndex == -1) return null;

        growthRoundRobinIndex = bestIndex + 1;
        return soldiers[bestIndex];
    }

    private bool TryAdvanceGrowth()
    {
        growthSteps++;

        if (growthSteps < growthStepsToSplit)
        {
            UpdateGrowthVisual();
            return false;
        }

        growthSteps = 0;
        UpdateGrowthVisual();
        return true;
    }

    private bool TryReduceGrowth()
    {
        if (growthSteps <= 0)
        {
            return false;
        }

        growthSteps--;
        UpdateGrowthVisual();
        return true;
    }

    private void SplitSoldier(GameObject sourceSoldier)
    {
        Vector3 spawnPos = sourceSoldier.transform.position + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0, UnityEngine.Random.Range(-0.5f, 0.5f));
        GameObject newSoldier = Instantiate(sourceSoldier, spawnPos, sourceSoldier.transform.rotation);
        Soldier newSoldierScript = newSoldier.GetComponent<Soldier>();
        if (newSoldierScript != null)
        {
            newSoldierScript.ResetGrowth();
        }

        soldiers.Add(newSoldier);
    }

    private IEnumerator SplitAfterDelay(GameObject sourceSoldier)
    {
        if (sourceSoldier == null) yield break;

        Vector3 normalScale = sourceSoldier.transform.localScale;
        sourceSoldier.transform.localScale = normalScale * splitPopScaleMultiplier;

        yield return new WaitForSeconds(splitDelay);

        if (sourceSoldier == null) yield break;

        sourceSoldier.transform.localScale = normalScale;
        SplitSoldier(sourceSoldier);
    }

    private void ResetGrowth()
    {
        growthSteps = 0;
        UpdateGrowthVisual();
    }

    private void UpdateGrowthVisual()
    {
        float growthMultiplier = (growthSteps * growthScalePerStep) / (1f + (growthSteps * growthDiminishFactor));
        transform.localScale = baseScale * (1f + growthMultiplier);
    }
}
