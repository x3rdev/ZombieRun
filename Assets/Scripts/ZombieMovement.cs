using NUnit.Framework;
using UnityEngine;

public class ZombieMovement : MonoBehaviour
{

    public float speed = 1;

    private Animator animator;

    public float attackThresholdX = 10;

    public float health = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (transform.position.x > attackThresholdX)
        {
            animator.SetBool("isAttacking", true);
            speed = 0;
        }

        if (health <= 0)
        {
            Destroy(gameObject);
        }


    }


    void takeDamage(float damage)
    {
        health -= damage;
    }

}
