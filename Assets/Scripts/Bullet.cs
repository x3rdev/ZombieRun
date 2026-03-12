using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float speed = 20f;
    public float damage = 5f;
    public float lifeTime = 2f;
    public float spinSpeed = 720f;
    [SerializeField] private Vector3 modelLongAxisLocal = Vector3.up;
    private Vector3 flightDirection;
    private float spinAngle;
    private readonly HashSet<int> triggeredWallIds = new HashSet<int>();

    void Awake()
    {
        flightDirection = Vector3.forward;
        ApplyVisualRotation();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += flightDirection * speed * Time.deltaTime;
        spinAngle += spinSpeed * Time.deltaTime;
        ApplyVisualRotation();
    }

    public void Initialize(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.0001f)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude <= 0.0001f)
            {
                direction = Vector3.forward;
            }

            flightDirection = direction.normalized;
            ApplyVisualRotation();
        }
    }

    private void ApplyVisualRotation()
    {
        Vector3 axis = modelLongAxisLocal.sqrMagnitude > 0.0001f ? modelLongAxisLocal.normalized : Vector3.up;
        Quaternion align = Quaternion.FromToRotation(axis, flightDirection);
        Quaternion spin = Quaternion.AngleAxis(spinAngle, flightDirection);
        transform.rotation = spin * align;
    }

    private void OnTriggerEnter(Collider other)
    { 
        if(other.CompareTag("Zombie")) {
          Zombie zombie = other.GetComponent<Zombie>();
                    if (zombie != null && zombie.health > 0f)
          {
              zombie.TakeDamage(damage);
              Destroy(gameObject);
          }
        }
        else 
        {
            MultiplierWall wall = other.GetComponentInParent<MultiplierWall>();
            if (wall != null)
            {
                int wallId = wall.GetInstanceID();
                if (triggeredWallIds.Contains(wallId)) return;

                triggeredWallIds.Add(wallId);
                wall.HitByBullet();
            }
        }
        
    }
}
