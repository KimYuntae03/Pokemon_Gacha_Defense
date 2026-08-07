using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private float maxHealth;
    private float currentHealth;

    public void SetHealth(float health)
    {
        maxHealth = health;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}