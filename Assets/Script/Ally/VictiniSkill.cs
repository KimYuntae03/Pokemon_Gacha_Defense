using System.Collections;
using UnityEngine;

public class VictiniSkill : MonoBehaviour
{
    [Header("스킬 설정")]
    [SerializeField]
    private float cooldown = 80f;

    [SerializeField]
    private float buffDuration = 7f;

    [SerializeField]
    private float attackBuffAmount = 0.50f;


    [Header("스킬 준비 이펙트")]
    [SerializeField]
    private GameObject skillAuraPrefab;


    private GameObject currentAura;

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
        skillReady = false;

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
            $"{gameObject.name} 비크티니 스킬 준비 완료"
        );
    }


    public void TryUseSkill()
    {
        if (!skillReady)
        {
            return;
        }

        StartCoroutine(
            BuffRoutine()
        );
    }


    private IEnumerator BuffRoutine()
    {
        skillReady = false;

        HideAura();

        AllyUpgradeManager upgradeManager =
            FindFirstObjectByType<AllyUpgradeManager>();

        if (upgradeManager != null)
        {
            upgradeManager.ApplyTemporaryAttackBuff(
                attackBuffAmount
            );
        }

        Debug.Log(
            $"{gameObject.name} 비크티니 버프 시작 / " +
            $"공격력 +{attackBuffAmount * 100f}%"
        );


        // 여기에서 다음 단계에
        // 전체 아군 공격력 버프를 실제 적용한다.


        yield return new WaitForSeconds(
            buffDuration
        );


        // 여기에서 공격력 버프 해제

        if (upgradeManager != null)
        {
            upgradeManager.RemoveTemporaryAttackBuff();
        }

        Debug.Log(
            $"{gameObject.name} 비크티니 버프 종료"
        );


        StartCoroutine(
            CooldownRoutine()
        );
    }


    private void ShowAura()
    {
        if (skillAuraPrefab == null ||
            currentAura != null)
        {
            return;
        }

        currentAura =
            Instantiate(
                skillAuraPrefab,
                transform
            );
    }


    private void HideAura()
    {
        if (currentAura == null)
        {
            return;
        }

        Destroy(
            currentAura
        );

        currentAura = null;
    }
}