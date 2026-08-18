using UnityEngine;

public class AllyAttack : MonoBehaviour
{
    private AllyData allyData;

    private Transform currentTarget;

    private float attackTimer;//기본 투사체 공격

    private float globalAttackTimer;//전역 공격

    private LayerMask enemyLayer;

    private AllyDrag allyDrag;//AllyDrag에서 유닉이 드래그해서 이동중인가?
    
    private Transform[] globalAttackPoints;

    private AllyUpgradeManager allyUpgradeManager;

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

        allyUpgradeManager = FindFirstObjectByType<AllyUpgradeManager>();

        attackTimer = 0f;
        globalAttackTimer = 0f;
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

        if (!IsCurrentTargetValid())
        {
            currentTarget = FindNearestEnemy();
        }

        if (currentTarget != null)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0f)
            {
                Attack();

                attackTimer =
                    allyData.attackInterval;
            }
        }

        if (allyData.useGlobalAttack)
        {
            globalAttackTimer -= Time.deltaTime;

            if (globalAttackTimer <= 0f)
            {
                AttackGlobalArea();

                globalAttackTimer =
                    allyData.globalAttackInterval;
            }
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

    private float GetFinalDamage(float baseDamage)
    {
        if (allyUpgradeManager == null)
        {
            return baseDamage;
        }

        return allyUpgradeManager.GetUpgradedDamage(
            baseDamage
        );
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
            GetFinalDamage(allyData.attackDamage)
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
                GetFinalDamage(allyData.attackDamage)
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
                    allyData.globalAttackPrefab,
                    point.position,
                    Quaternion.identity
                );

            ArceusThunderAttack thunderAttack =
                attackObject.GetComponent<ArceusThunderAttack>();

            if (thunderAttack != null)
            {
                thunderAttack.Initialize(
                    GetFinalDamage(
                        allyData.globalAttackDamage
                    )
                );
            }
        }
    }
}