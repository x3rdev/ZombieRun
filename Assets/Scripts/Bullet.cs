using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float speed = 20f;
    public float damage = 5f;
    public float lifeTime = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
      transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    { 
        if(other.CompareTag("Zombie")) {
          ZombieMovement zombie = other.GetComponent<ZombieMovement>();
          if (zombie != null)
          {
              zombie.TakeDamage(damage);
              
          }
          Destroy(gameObject);
        }
    }
}
