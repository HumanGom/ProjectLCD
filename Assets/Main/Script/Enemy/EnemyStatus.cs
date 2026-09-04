using Mono.Cecil;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    [Header("적이름")]
    [SerializeField] private string enemyName = "적";
    [Header("HP기본값")]
    [SerializeField] private int defaultHp = 250;
    [Header("레벨")]
    [SerializeField] private int level = 1;
    [Header("방어력")]
    [SerializeField] private int defaultDefenseLevel = 0;
    [Header("속성내성")]
    [SerializeField]
    private List<ElementResist> deFaultElementResists = new List<ElementResist>()
    {
        new ElementResist() { elementType = ElementType.Fire, resistValue = 1.0f },
        new ElementResist() { elementType = ElementType.Water, resistValue = 1.0f },
        new ElementResist() { elementType = ElementType.Earth, resistValue = 0.75f },
        new ElementResist() { elementType = ElementType.Electric, resistValue = 1.5f },
        new ElementResist() { elementType = ElementType.Ice, resistValue = 2.0f },
        new ElementResist() { elementType = ElementType.Wind, resistValue = 1.0f },
    };
    [Header("공격타입내성")]
    [SerializeField]
    private List<AttackResist> defaultAttackResist = new List<AttackResist>()
    {
        new AttackResist() { attackType = AttackType.Cut, resistValue = 1.0f },
        new AttackResist() { attackType = AttackType.Punch, resistValue = 0.75f },
        new AttackResist() { attackType = AttackType.Pierce, resistValue = 2.0f },
    };
    [Header("최소 최대 속도 기본값")]
    [SerializeField] private SpeedStat defaultSpeedRange = new SpeedStat { minSpeed = 2, maxSpeed = 6 };
    [Header("스피드UI")]
    [SerializeField] private SpeedUI speedUi;
    [Header("케릭터 초상화")]
    [SerializeField] private Sprite ownPortrait;
    [Header("화살표 프리팹")]
    [SerializeField] private GameObject arrowSplineMeshPrefab;
    [Header("화살표 생성 루트")]
    [SerializeField] private Transform arrowRoot;

    private readonly List<ArrowSplineMesh> spawnedArrows = new();
    private int mentality = 0;
    private BattleSide battleSide = BattleSide.Enemy;
    private EnemysManager m_EnemysManager;
    private int hp;
    private int shield = 0;
    private int speed;
    private int defenseLevel;
    private SpeedStat speedRange;
    private HPBarUI hpBarUI;
    private MentalUI mentalUI;
    private UsingSkillUI usingSkillUI;
    private List<ElementResist> elementResists = new List<ElementResist>();
    private List<AttackResist> attackResists = new List<AttackResist>();
    private bool isDead = false;
    private int additionalSpeed = 0;
    private EffectSound effectSound;

    public bool IsDead { get { return isDead; } }
    public int MentalityValue { get { return mentality; } set { mentality = Mathf.Clamp(value, -45, 45); if (mentalUI != null) { mentalUI.ReFreshMentalUI(); } } }
    public int DefenseLevelValue { get { return defenseLevel; } set { defenseLevel = value; } }
    public int GetSpeed { get { return speed + additionalSpeed; } }
    public Sprite GetPortrait { get { return ownPortrait; } }
    public string GetName { get { return enemyName; } }
    public UsingSkillUI GetUsingSkillUI { get { return usingSkillUI; } }
    public int HpValue
    {
        get { return hp; }
        set
        {
            hp = Mathf.Clamp(value, 0, defaultHp); if (hpBarUI != null)
            { hpBarUI.ReFreshHPUI(); }
            { if (hp <= 0) OnDeath(); }
        }
    }

    public int ShieldValue
    {
        get { return shield; }
        set { shield = Mathf.Max(0, value); if (hpBarUI != null) hpBarUI.ReFreshHPUI(); }
    }
    public int additionalSppeedValue 
    { 
        get { return additionalSpeed; } 
        set { additionalSpeed = Mathf.Max(0, additionalSpeed + value); speedUi.RequestSpeedForUi(GetSpeed); } 
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

        foreach (var resist in deFaultElementResists)
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

    public void SetEnemyRandomSpeed()
    {
        speed = Random.Range(speedRange.minSpeed, speedRange.maxSpeed);
        speedUi.RequestSpeedForUi(GetSpeed);
        //Debug.Log($"{this.name}의 속도값이{speed}로 변경됨");
    }

    public void InitializeEnemySlotArrows()
    {
        EnemySlot[] enemySlots = GetComponentsInChildren<EnemySlot>(true);

        foreach (EnemySlot slot in enemySlots)
        {
            if (slot == null) continue;

            ArrowSplineMesh arrow = Instantiate(arrowSplineMeshPrefab.GetComponent<ArrowSplineMesh>(), arrowRoot);

            slot.SetArrowSplineMesh(arrow);
            spawnedArrows.Add(arrow);
        }
    }
    public void OnDeath()
    {
        if (isDead) return;
        Debug.Log($"{enemyName} 사망");
        if (effectSound != null) effectSound.PlayDeadClip();
        isDead = true;
        m_EnemysManager.RemoveEnemyList(this.gameObject);
        gameObject.SetActive(false);
    }

    private void UIConnect()
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

    private void Awake()
    {
        m_EnemysManager = this.GetComponentInParent<EnemysManager>();
        hp = defaultHp;
        speed = defaultSpeedRange.minSpeed;
        speedRange = new SpeedStat { minSpeed = defaultSpeedRange.minSpeed, maxSpeed = defaultSpeedRange.maxSpeed };
        effectSound = GetComponentInChildren<EffectSound>();
        defenseLevel = defaultDefenseLevel;
        ResetElementResitValue();
        ResetAttackResitValue();
    }
    private void Start()
    {
        UIConnect();
        SetEnemyRandomSpeed();
        InitializeEnemySlotArrows();
    }

}
