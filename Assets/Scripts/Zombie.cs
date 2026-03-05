using NUnit.Framework;
using UnityEngine;

public class Zombie : MonoBehaviour
{

    public float speed = 1;

    private Animator animator;

    public float attackThresholdZ = 15F;

    public float health = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            if (!animator.GetBool("isDead")) 
            {
                speed = 0;
                animator.SetBool("isDead", true);
                Destroy(gameObject, 1.5f);
            }
            return;
        }

        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (transform.position.z < attackThresholdZ)
        {
            animator.SetBool("isAttacking", true);
            speed = 0;
        }
    }


    public void TakeDamage(float damage)
    {
        health -= damage;
    }

}
