using UnityEngine;
using UnityEngine.EventSystems;

public class EnemySkillTarget : MonoBehaviour,
    IPointerClickHandler
{
    private EnemyMovement enemyMovement;


    private void Awake()
    {
        enemyMovement =
            GetComponent<EnemyMovement>();
    }


    public void OnPointerClick(
        PointerEventData eventData
    )
    {
        if (enemyMovement == null)
        {
            return;
        }


        bool skillUsed =
            CelebiSkill.TryUseSkillOnEnemy(
                enemyMovement
            );


        if (skillUsed)
        {
            Debug.Log(
                $"{gameObject.name} 세레비 스킬 타겟 지정"
            );
        }
    }
}