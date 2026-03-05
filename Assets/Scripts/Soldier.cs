using UnityEngine;
using UnityEngine.InputSystem;

public class Soldier : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.5f;

    private float nextFireTime = 0f;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();  
    }

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
        float horizontalInput = 0;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                horizontalInput = -1;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                horizontalInput = 1;
        }
        animator.SetBool("Run", horizontalInput != 0);
        transform.Translate(horizontalInput * 5 * Time.deltaTime, 0, 0);      
        
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}
