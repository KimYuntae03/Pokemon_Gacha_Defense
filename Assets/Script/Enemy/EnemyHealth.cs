using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private float maxHealth;

    [SerializeField]
    private float currentHealth;
    private GoldOrbManager goldOrbManager;

    private EnemyData enemyData;
    
    private void Start()
    {
        goldOrbManager =
            FindFirstObjectByType<GoldOrbManager>();
    }

    public void Initialize(EnemyData data)
    {
        enemyData = data;

        maxHealth = data.maxHealth;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log(
            $"{gameObject.name} 피격 / " +
            $"받은 데미지: {damage} / " +
            $"남은 체력: {currentHealth}"
        );
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {      
        if (GameResultManager.Instance != null)
        {
            GameResultManager.Instance.EnemyDied();
        }

        if (enemyData != null && enemyData.enemyName == "ETERNATUS")
        {
            if (GameResultManager.Instance != null)
            {
                GameResultManager.Instance.GameClear();
            }
        }

        if (goldOrbManager != null)
        {
            goldOrbManager.AddGoldOrb(1);
        }

        Destroy(gameObject);
    }
}