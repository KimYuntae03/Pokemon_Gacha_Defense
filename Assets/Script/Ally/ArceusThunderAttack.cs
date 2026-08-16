using System.Collections.Generic;
using UnityEngine;

public class ArceusThunderAttack : MonoBehaviour
{
    private float damage;

    private readonly HashSet<EnemyHealth> damagedEnemies =
        new HashSet<EnemyHealth>();


    public void Initialize(float newDamage)
    {
        damage = newDamage;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemyHealth =
            other.GetComponent<EnemyHealth>();

        if (enemyHealth == null)
        {
            return;
        }

        /*
         * 같은 천둥 프리팹에 같은 적이 여러 번 접촉해도
         * 한 번만 데미지를 주기 위해 기록한다.
         */
        if (damagedEnemies.Contains(enemyHealth))
        {
            return;
        }

        damagedEnemies.Add(enemyHealth);

        enemyHealth.TakeDamage(
            damage
        );
    }
}