using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private float maxHealth;

    [SerializeField]
    private float currentHealth;
    private GoldOrbManager goldOrbManager;

    private void Start()
    {
        goldOrbManager =
            FindFirstObjectByType<GoldOrbManager>();
    }

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
        if (goldOrbManager != null)
        {
            goldOrbManager.AddGoldOrb(1);
        }

        Destroy(gameObject);
    }
}