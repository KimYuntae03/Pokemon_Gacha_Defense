using System.Collections;
using UnityEngine;

public class CelebiSkill : MonoBehaviour
{
    [Header("스킬 설정")]
    [SerializeField]
    private float cooldown = 80f; 

    [SerializeField]
    private float slowDuration = 10f;

    [SerializeField]
    [Range(0f, 1f)]
    private float slowAmount = 0.80f;

    [Header("스킬 준비 이펙트")]
    [SerializeField]
    private GameObject skillAuraPrefab;


    private GameObject currentAura;

    // 현재 타겟을 선택하고 있는 세레비
    private static CelebiSkill targetingCelebi;
    private bool skillReady;

    public bool SkillReady
    {
        get { return skillReady; }
    }


    public void Initialize(
        GameObject auraPrefab
    )
    {
        skillAuraPrefab =
            auraPrefab;
    }


    private void Start()
    {

        StartCoroutine(
            CooldownRoutine()
        );
    }


    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(
            cooldown
        );

        SetSkillReady();
    }


    private void SetSkillReady()
    {   
        skillReady = true;
        
        ShowAura();
    }


    public void SelectSkill()
    {
        targetingCelebi = this;
    }


    public static bool TryUseSkillOnEnemy(
        EnemyMovement enemyMovement
    )
    {
        if (targetingCelebi == null)
        {
            return false;
        }

        if (!targetingCelebi.skillReady)
        {
            return false;
        }

        targetingCelebi.UseSkill(
            enemyMovement
        );

        return true;
    }


    private void UseSkill(
        EnemyMovement enemyMovement
    )
    {
        if (enemyMovement == null)
        {
            return;
        }

        skillReady = false;

        targetingCelebi = null;

        HideAura();


        enemyMovement.ApplySlow(
            slowAmount,
            slowDuration,
            this
        );
    }


    public void OnSlowFinished()
    {
        StartCoroutine(
            CooldownRoutine()
        );
    }


    private void CancelTargeting()
    {
        if (targetingCelebi == this)
        {
            targetingCelebi = null;
        }
    }


    private void ShowAura()
    {
        if (skillAuraPrefab == null)
        {
            return;
        }

        if (currentAura != null)
        {
            return;
        }


        currentAura = Instantiate(
            skillAuraPrefab,
            transform
        );

        currentAura.transform.localPosition =
            Vector3.zero;
    }


    private void HideAura()
    {
        if (currentAura == null)
        {
            return;
        }

        Destroy(currentAura);

        currentAura = null;
    }


    private void OnDestroy()
    {
        if (targetingCelebi == this)
        {
            targetingCelebi = null;
        }
    }
}