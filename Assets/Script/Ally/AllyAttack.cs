using UnityEngine;

public class AllyAttack : MonoBehaviour
{
    private AllyData allyData;

    private Transform currentTarget;

    private float attackTimer;

    private LayerMask enemyLayer;

    private AllyDrag allyDrag;//AllyDrag에서 유닉이 드래그해서 이동중인가?

    public void Initialize(
        AllyData data,
        LayerMask targetLayer
    )
    {
        allyData = data;
        enemyLayer = targetLayer;

        allyDrag = GetComponent<AllyDrag>();

        attackTimer = 0f;
    }


    private void Update()
    {
        if (allyData == null)
        {
            return;
        }

        if (allyDrag != null && allyDrag.IsDragging)
        {
            return;
        }
        
        // 현재 타겟이 없거나 유효하지 않으면
        // 새로운 적을 찾는다.
        if (!IsCurrentTargetValid())
        {
            currentTarget = FindNearestEnemy();
        }

        // 그래도 타겟이 없다면 공격하지 않는다.
        if (currentTarget == null)
        {
            return;
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            Attack();

            attackTimer =
                allyData.attackInterval;
        }
    }


    private bool IsCurrentTargetValid()
    {
        if (currentTarget == null)
        {
            return false;
        }

        float distance =
            Vector2.Distance(
                transform.position,
                currentTarget.position
            );

        return distance <= allyData.attackRange;
    }


    private Transform FindNearestEnemy()
    {
        Collider2D[] enemies =
            Physics2D.OverlapCircleAll(
                transform.position,
                allyData.attackRange,
                enemyLayer
            );

        Transform nearestEnemy = null;

        float nearestDistance =
            Mathf.Infinity;


        foreach (Collider2D enemy in enemies)
        {
            float distance =
                Vector2.Distance(
                    transform.position,
                    enemy.transform.position
                );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;

                nearestEnemy =
                    enemy.transform;
            }
        }


        return nearestEnemy;
    }


    private void Attack()
    {
        if (currentTarget == null)
        {
            return;
        }

        if (allyData.projectilePrefab == null)
        {
            Debug.LogWarning(
                $"{gameObject.name}: 투사체 Prefab이 없습니다.",
                this
            );

            return;
        }


        GameObject projectileObject =
            Instantiate(
                allyData.projectilePrefab,
                transform.position,
                Quaternion.identity
            );


        Projectile projectile =
            projectileObject.GetComponent<Projectile>();

        if (projectile == null)
        {
            Debug.LogError(
                $"{allyData.projectilePrefab.name}에 Projectile 스크립트가 없습니다.",
                allyData.projectilePrefab
            );

            Destroy(projectileObject);

            return;
        }


        projectile.Initialize(
            currentTarget,
            allyData.attackDamage
        );
    }
}