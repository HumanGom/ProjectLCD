using System.Collections.Generic;
using UnityEngine;

public abstract class SkillObjectOS : ScriptableObject
{
    [Header("스킬 타입")]
    [SerializeField] private SkillType skillType;
    [Header("기본 스킬 아이콘")]
    [SerializeField] private Sprite defaultIcon;
    [Header("스킬 표시 정보")]
    [SerializeField] private string skillName;
    [SerializeField] private Sprite icon;
    [SerializeField] private Color slotColor = Color.white;
    [Header("스킬 원소 속성")]
    [SerializeField] private ElementType elementType = ElementType.None;
    [Header("스킬 공격 속성")]
    [SerializeField] private AttackType attackType = AttackType.None;
    [Header("공격 레벨")]
    [SerializeField] private int defaultAttackLevel = 0;
    [Header("방어 레벨")]
    [SerializeField] private int defaultDefenseLevel = 0;
    [Header("기본 위력 (기본 스킬 위력")]
    [SerializeField] private int basiclSkillPower = 0;
    [Header("최종 위력 (마지막 코인 추가 위력)")]
    [SerializeField] private int finalCoinPower = 0;
    [Header("코인 위력 (코인이 앞면일때 추가되는 위력)")]
    [SerializeField] private int coinPower = 0;
    [Header("합 가능 여부")]
    [SerializeField] private bool canClash = true;
    [Header("코인 리스트")]
    [SerializeField] private List<SkillCoinOS> skillCoinList;

    private int attackLevel;
    private int defenseLevel;

    public int GetBasicSkillPower { get { return basiclSkillPower; } }
    public int GetfinalCoinPower { get { return finalCoinPower; } }
    public int GetcoinPower { get { return coinPower; } }
    public bool GetcanClash {  get { return canClash; } }
    public int AttackLevelValue { get { return attackLevel; } set { attackLevel = value; } }
    public int DefenseLevelValue { get { return defenseLevel; } set { defenseLevel = value; } }
    public ElementType GetElementType{ get { return elementType; } }
    public AttackType GetAttackType { get { return attackType; } }
    

    public SkillType SkillType => skillType;
    public string SkillName => skillName;
    public Sprite Icon => icon;
    public Color SlotColor => slotColor;

    public List<SkillCoinOS> GetSkillCoinList { get { return skillCoinList;  } }

    public abstract void Execute(SkillContext context);

    private void Awake()
    {
        attackLevel = defaultAttackLevel;
        if (icon == null && defaultIcon != null) icon = defaultIcon;
    }
}