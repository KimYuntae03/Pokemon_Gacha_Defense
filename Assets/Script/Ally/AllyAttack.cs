using UnityEngine;

public class AllyAttack : MonoBehaviour
{
    private AllyData allyData;

    private Transform currentTarget;

    private float attackTimer;

    private LayerMask enemyLayer;

    private AllyDrag allyDrag;//AllyDrag에서 유닉이 드래그해서 이동중인가?
    
    private Transform[] globalAttackPoints;

    public void Initialize(
        AllyData data,
        LayerMask targetLayer,
        Transform[] attackPoints
    )
    {
        allyData = data;
        enemyLayer = targetLayer;
        globalAttackPoints = attackPoints;
        
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

    switch (allyData.attackType)
    {
        case AllyAttackType.Projectile:
            FireProjectile();
            break;

        case AllyAttackType.TargetArea:
            AttackTargetArea();
            break;

        case AllyAttackType.GlobalArea:
            AttackGlobalArea();
            break;
    }
}

    private void FireProjectile()
    {
        if (allyData.attackPrefab == null)
        {
            return;
        }

        GameObject projectileObject =
            Instantiate(
                allyData.attackPrefab,
                transform.position,
                Quaternion.identity
            );

        Projectile projectile =
            projectileObject.GetComponent<Projectile>();

        if (projectile == null)
        {
            Destroy(projectileObject);

            return;
        }

        projectile.Initialize(
            currentTarget,
            allyData.attackDamage
        );
    }

    private void AttackTargetArea()
    {
        if (currentTarget == null)
        {
            return;
        }

        if (allyData.attackPrefab == null)
        {
            return;
        }

        Vector3 attackPosition = currentTarget.position;

        EnemyHealth enemyHealth = currentTarget.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(
                allyData.attackDamage
            );
        }

        Instantiate(
            allyData.attackPrefab,
            attackPosition,
            Quaternion.identity
        );
    }

    private void AttackGlobalArea()
    {
        if (allyData.attackPrefab == null)
        {
            return;
        }

        if (globalAttackPoints == null ||
            globalAttackPoints.Length == 0)
        {
            return;
        }


        foreach (Transform point in globalAttackPoints)
        {
            if (point == null)
            {
                continue;
            }

            GameObject attackObject =
                Instantiate(
                    allyData.attackPrefab,
                    point.position,
                    Quaternion.identity
                );

            ArceusThunderAttack thunderAttack =
                attackObject.GetComponent<ArceusThunderAttack>();

            if (thunderAttack != null)
            {
                thunderAttack.Initialize(
                    allyData.attackDamage
                );
            }
        }
    }
}