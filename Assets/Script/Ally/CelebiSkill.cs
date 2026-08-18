using System.Collections;
using UnityEngine;

public class CelebiSkill : MonoBehaviour
{
    [Header("스킬 설정")]
    [SerializeField]
    private float cooldown = 80f; // 테스트용. 나중에 90f

    [SerializeField]
    private float slowDuration = 10f;

    [SerializeField]
    [Range(0f, 1f)]
    private float slowAmount = 0.80f;

    [Header("스킬 준비 이펙트")]
    [SerializeField]
    private GameObject skillAuraPrefab;


    private GameObject currentAura;

    private bool skillReady;
    private bool isTargeting;


    // 현재 타겟을 선택하고 있는 세레비
    private static CelebiSkill targetingCelebi;


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
        skillReady = false;
        isTargeting = false;

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

        Debug.Log(
            $"{gameObject.name} 스킬 준비 완료"
        );
    }


    public void SelectSkill()
    {
        if (!skillReady)
        {
            return;
        }


        // 이미 이 세레비가 선택된 상태라면 선택 취소
        if (targetingCelebi == this)
        {
            CancelTargeting();
            return;
        }


        // 다른 세레비가 선택되어 있었다면 취소
        if (targetingCelebi != null)
        {
            targetingCelebi.isTargeting = false;
        }


        targetingCelebi = this;
        isTargeting = true;


        Debug.Log(
            $"{gameObject.name} 스킬 타겟 선택 대기"
        );
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
        isTargeting = false;

        targetingCelebi = null;

        HideAura();


        enemyMovement.ApplySlow(
            slowAmount,
            slowDuration,
            this
        );


        Debug.Log(
            $"{gameObject.name} 스킬 사용 / " +
            $"감속 {slowAmount * 100f}% / " +
            $"{slowDuration}초"
        );
    }


    public void OnSlowFinished()
    {
        StartCoroutine(
            CooldownRoutine()
        );

        Debug.Log(
            $"{gameObject.name} 스킬 재충전 시작"
        );
    }


    private void CancelTargeting()
    {
        isTargeting = false;

        if (targetingCelebi == this)
        {
            targetingCelebi = null;
        }

        Debug.Log(
            $"{gameObject.name} 스킬 선택 취소"
        );
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