using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("투사체 설정")]
    [SerializeField]
    private float moveSpeed = 8f;

    private Transform target;

    private float damage;


    public void Initialize(
        Transform newTarget,
        float newDamage
    )
    {
        target = newTarget;
        damage = newDamage;
    }


    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction =
            (target.position - transform.position)
            .normalized;

        transform.position +=
            direction *
            moveSpeed *
            Time.deltaTime;
    }


    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        if (target == null)
        {
            return;
        }

        if (other.transform != target)
        {
            return;
        }

        EnemyHealth enemyHealth =
            other.GetComponent<EnemyHealth>();

        if (enemyHealth == null)
        {
            return;
        }

        enemyHealth.TakeDamage(
            damage
        );

        Destroy(gameObject);
    }
}