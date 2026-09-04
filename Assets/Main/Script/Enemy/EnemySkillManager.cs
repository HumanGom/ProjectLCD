using System.Collections.Generic;
using UnityEngine;

public class EnemySkillManager : MonoBehaviour
{
    [Header("적 슬롯")]
    [SerializeField] private EnemySlot enemySlot;

    [Header("적 스킬 리스트")]
    [SerializeField] private List<SkillObjectOS> skillObjects = new List<SkillObjectOS>();

    private List<SkillType> savedSkillList = new List<SkillType>() {
            SkillType.Skill1,
            SkillType.Skill1,
            SkillType.Skill1,

            SkillType.Skill2,
            SkillType.Skill2,

            SkillType.Skill3
    };

    private List<SkillType> skillList = new List<SkillType>();
    private int currentIndex = 0;

    private void RefillSkills()
    {
        List<SkillType> tempList = new List<SkillType>(savedSkillList);


        Shuffle(tempList);

        skillList.AddRange(tempList);
    }

    private void Shuffle(List<SkillType> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    public SkillType UseSkill()
    {
        SkillType skill = skillList[0];
        skillList.RemoveAt(0);

        if (skillList.Count <= 3) RefillSkills();

        return skill;
    }

    public void SetEnemySlot()
    {
        SkillType selectedSkill = UseSkill();
        enemySlot.RequesSkillManagerForEnemySlot(this, selectedSkill);
    }

    public SkillObjectOS GetSkillObject(SkillType skillType)
    {
        foreach (SkillObjectOS skill in skillObjects)
        {
            if (skill.SkillType == skillType) return skill;
        }

        return null;
    }

    public void ExecuteSkill(SkillType skillType, SkillContext context)
    {
        SkillObjectOS skill = GetSkillObject(skillType);

        if (skill == null)
        {
            Debug.LogWarning($"{gameObject.name}에게 {skillType} 스킬 오브젝트가 없음");
            return;
        }

        skill.Execute(context);
    }

    private void Awake()
    {
        RefillSkills();
    }

    void Start()
    {
        SetEnemySlot();
    }
}
