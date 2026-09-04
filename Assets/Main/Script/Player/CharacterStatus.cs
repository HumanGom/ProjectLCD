using Mono.Cecil;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    [Header("케릭터이름")]
    [SerializeField] private string characterName = "전사";
    [Header("HP기본값")]
    [SerializeField] private int defaultHp = 250;
    [Header("레벨")]
    [SerializeField] private int level = 1;
    [Header("방어력")]
    [SerializeField] private int defaultDefenseLevel = 0;
    [Header("속성내성")]
    [SerializeField] private List<ElementResist> defaultElementResists = new List<ElementResist>()
    {
        new ElementResist() { elementType = ElementType.Fire, resistValue = 1.0f },
        new ElementResist() { elementType = ElementType.Water, resistValue = 1.0f },
        new ElementResist() { elementType = ElementType.Earth, resistValue = 0.75f },
        new ElementResist() { elementType = ElementType.Electric, resistValue = 1.5f },
        new ElementResist() { elementType = ElementType.Ice, resistValue = 2.0f },
        new ElementResist() { elementType = ElementType.Wind, resistValue = 1.0f },
    };
    [Header("공격타입내성")]
    [SerializeField] private List<AttackResist> defaultAttackResist = new List<AttackResist>()
    {
        new AttackResist() { attackType = AttackType.Cut, resistValue = 1.0f },
        new AttackResist() { attackType = AttackType.Punch, resistValue = 0.75f },
        new AttackResist() { attackType = AttackType.Pierce, resistValue = 2.0f },
    };
    [Header("최소 최대 속도 기본값")]
    [SerializeField] private SpeedStat defaultSpeed = new SpeedStat { minSpeed = 4, maxSpeed = 8 };
    [Header("정신력")]
    [SerializeField] private int mentality = 0;
    [Header("캐릭터 초상화")]
    [SerializeField] private Sprite ownPortrait;

    [Header("머리슬롯 프리팹")]
    [SerializeField] private GameObject headSlotPrefab;
    [Header("머리슬롯 생성루트")]
    [SerializeField] private Transform headSlotRoot;

    [Header("화살표 프리팹")]
    [SerializeField] private GameObject arrowSplineMeshPrefab;
    [Header("화살표 생성루트")]
    [SerializeField] private Transform arrowRoot;

    private readonly List<HeadSlot> spawnedHeadSlots = new List<HeadSlot>();
    private readonly List<ArrowSplineMesh> spawnedArrows = new List<ArrowSplineMesh>();
    private SpeedUI speedUi;
    private BattleSide battleSide = BattleSide.Player;
    private CharactersManager m_CharatersManager;
    private int hp;
    private int shield = 0;
    private int speed;
    private int defenseLevel;
    private SpeedStat charaterSpeed;
    private HPBarUI hpBarUI;
    private MentalUI mentalUI;
    private UsingSkillUI usingSkillUI;
    private List<ElementResist> elementResists = new List<ElementResist>();
    private List<AttackResist> attackResists = new List<AttackResist>();
    private bool isDead = false;
    private EffectSound effectSound;
    public bool IsDead { get { return isDead; } }
    public int MentalityValue { get { return mentality; } set { mentality = Mathf.Clamp(value, -45, 45); if (mentalUI != null) { mentalUI.ReFreshMentalUI(); } } }
    public int DefenseLevelValue { get { return defenseLevel; } set { defenseLevel = value; } }
    public int GetSpeed { get { return speed; }}
    public Sprite GetPortrait { get { return ownPortrait; } }
    public string GetName { get { return characterName; } }
    public UsingSkillUI GetUsingSkillUI { get { return usingSkillUI; } }
    public int GetDefaultHP { get { return defaultHp; }}
    public int HpValue { 
        get { return hp; } 
        set { hp = Mathf.Clamp(value, 0, defaultHp); 
            if (hpBarUI != null) hpBarUI.ReFreshHPUI(); 
            { if (hp <= 0) OnDeath(); }
        } }

    public int ShieldValue
    {
        get { return shield; }
        set { shield = Mathf.Max(0, value); if (hpBarUI != null)  hpBarUI.ReFreshHPUI(); }
    }

    public float GetElementResistFromList(ElementType elementType) 
    {
        float resistValue = 0;
        foreach (ElementResist elementResist in elementResists)
        {
            if (elementResist.elementType == elementType)
            {
                resistValue = elementResist.resistValue;
                break;
            }
        }
        return resistValue;
    }
    
    public void SetElementResistFromList(ElementType elementType, float resistValue)
    {
        foreach (ElementResist elementResist in elementResists)
        {
            if (elementResist.elementType == elementType)
            {
                elementResist.resistValue = resistValue;
                return;
            }
        }
    }

    public void ResetElementResitValue()
    {
        elementResists.Clear();

        foreach (var resist in defaultElementResists)
        {
            elementResists.Add(new ElementResist() { elementType = resist.elementType, resistValue = resist.resistValue });
        }
    }

    public float GetAttackResistFromList(AttackType attackType)
    {
        float resistValue = 0;
        foreach (AttackResist attackResist in attackResists)
        {
            if (attackResist.attackType == attackType)
            {
                resistValue = attackResist.resistValue;
                break;
            }
        }
        return resistValue;
    }

    public void SetAttackResistFromList(AttackType attackType, float resistValue)
    {
        foreach (AttackResist attackResist in attackResists)
        {
            if (attackResist.attackType == attackType)
            {
                attackResist.resistValue = resistValue;
                return;
            }
        }
    }

    public void ResetAttackResitValue()
    {
        attackResists.Clear();

        foreach (var resist in defaultAttackResist)
        {
            attackResists.Add(new AttackResist() { attackType = resist.attackType, resistValue = resist.resistValue });
        }
    }

    public void SetRandomSpeed()
    {
        speed = UnityEngine.Random.Range(defaultSpeed.minSpeed, defaultSpeed.maxSpeed);
        //Debug.Log($"{this.name}의 속도값이{speed}로 변경됨");
    }

    public void RequestPassingSpeedForUI()
    {
        speedUi.RequestSpeedForUi(speed);
    }

    public void UIConnect()
    {
        usingSkillUI = this.GetComponentInChildren<UsingSkillUI>();

        hpBarUI = GetComponentInChildren<HPBarUI>();
        if (hpBarUI != null)
        {
            hpBarUI.Initialize(this);
            hpBarUI.ReFreshHPUI();
        }

        mentalUI = GetComponentInChildren<MentalUI>();
        if (mentalUI != null)
        {
            mentalUI.Initialize(this);
            mentalUI.ReFreshMentalUI();
        }
    }

    public HeadSlot CreateHeadSlot()
    {
        HeadSlot headSlot = Instantiate(headSlotPrefab.GetComponent<HeadSlot>(), headSlotRoot);
        ArrowSplineMesh arrow = Instantiate(arrowSplineMeshPrefab.GetComponent<ArrowSplineMesh>(), arrowRoot);

        CharacterSkillManager skillManager = GetComponent<CharacterSkillManager>();
        headSlot.InitializeOwner(this, skillManager);
        headSlot.SetArrowSplineMesh(arrow);

        spawnedHeadSlots.Add(headSlot);
        spawnedArrows.Add(arrow);

        return headSlot;
    }

    public void ClearHeadSlots()
    {
        foreach (HeadSlot headSlot in spawnedHeadSlots)
        {
            if (headSlot != null)
            {
                Destroy(headSlot.gameObject);
            }
        }

        foreach (ArrowSplineMesh arrow in spawnedArrows)
        {
            if (arrow != null)
            {
                Destroy(arrow.gameObject);
            }
        }

        spawnedHeadSlots.Clear();
        spawnedArrows.Clear();
    }

    public void OnDeath()
    {
        if (isDead) return;

        Debug.Log($"{characterName} 사망");
        isDead = true;

        if (effectSound != null) effectSound.PlayDeadClip();

        if (m_CharatersManager != null)
        {
            m_CharatersManager.OnCharacterDead(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Awake()
    {
        isDead = false;
        m_CharatersManager = this.GetComponentInParent<CharactersManager>();
        speedUi = this.GetComponentInChildren<SpeedUI>();
        effectSound = GetComponentInChildren<EffectSound>();
        hp = defaultHp;
        speed = defaultSpeed.minSpeed;
        defenseLevel = defaultDefenseLevel;
        ResetElementResitValue();
        ResetAttackResitValue();
    }

    private void Start()
    {
        UIConnect();
    }
}
