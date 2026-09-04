using System.Collections.Generic;
using UnityEngine;

public class CharacterSkillManager : MonoBehaviour
{

    [Header("헤드 슬롯")]
    [SerializeField] private HeadSlot headSlot;

    [Header("캐릭터 스킬 리스트")]
    [SerializeField] private List<SkillObjectOS> skillObjects = new List<SkillObjectOS>();

    [SerializeField] private SkillType defenseSkillType = SkillType.Defense;

    private List<SkillType> savedSkillList = new List<SkillType>() {
            SkillType.Skill1,
            SkillType.Skill1,
            SkillType.Skill1,

            SkillType.Skill2,
            SkillType.Skill2,

            SkillType.Skill3 
    };

    private List<List<SkillType>> skillDecks = new List<List<SkillType>>();
    private int currentIndex = 0;

    public HeadSlot GetHeadSlot {  get { return headSlot; } }
    public SkillType GetDefenseSkillType { get { return defenseSkillType; } }


    public SkillType UseSkill(int deckIndex, int slotIndex)
    {
        EnsureDeckCount(deckIndex + 1);

        SkillType skill = skillDecks[deckIndex][slotIndex];
        skillDecks[deckIndex].RemoveAt(slotIndex);

        if (skillDecks[deckIndex].Count <= 3)
        {
            FillDeck(skillDecks[deckIndex]);
        }

        return skill;
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


    public SkillType GetSkill(int deckIndex, int skillIndex)
    {
        EnsureDeckCount(deckIndex + 1);

        return skillDecks[deckIndex][skillIndex];
    }

    public void EnsureDeckCount(int count)
    {
        while (skillDecks.Count < count)
        {
            List<SkillType> newDeck = new List<SkillType>();
            FillDeck(newDeck);
            skillDecks.Add(newDeck);
        }
    }

    public void RequestFromHeadSlot(PlayerSlot _playerSlot, HeadSlot targetHeadSlot, EnemySlot enemySlot)
    {
        targetHeadSlot.RequestPlayerSlotAndSkillManager(_playerSlot, this, enemySlot);
    }

    private void FillDeck(List<SkillType> deck)
    {
        List<SkillType> tempList = new List<SkillType>(savedSkillList);
        Shuffle(tempList);
        deck.AddRange(tempList);
    }

    private void Shuffle(List<SkillType> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
    
    private void Awake()
    {
        EnsureDeckCount(1);
        //Debug.Log($"{this.gameObject.name}의 현재 배열된 스킬들 {string.Join(", ", skillList)}");
    }
}
