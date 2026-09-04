using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("부위")]
    [SerializeField] private List<BossPart> parts = new();
    [Header("슬롯")]
    [SerializeField] private List<EnemySlot> slots = new();
    [Header("스킬 매니저")]
    [SerializeField] private EnemySkillManager skillManager;
    [Header("적 스테이터스")]
    [SerializeField] private EnemyStatus m_enemyStatus;
    [Header("패턴 순서")]
    [SerializeReference, SubclassSelector] private List<BossPattern> patterns = new();

    private int patternIndex = 0;

    public EnemySkillManager SkillManager => skillManager;

    public void SetNextPattern()
    {
        ClearSlots();

        if (patterns.Count == 0)
        {
            Debug.LogWarning($"{name} 보스 패턴이 비어있음");
            return;
        }

        BossPattern pattern = patterns[patternIndex];

        if (pattern != null) pattern.Execute(this);

        patternIndex = (patternIndex + 1) % patterns.Count;
    }

    public void ApplyCommand(BossSlotCommand command)
    {
       
        EnemySlot slot = FindSlot(command.slotID);

        if (slot == null)
        {
            Debug.LogWarning($"보스 슬롯을 찾지 못함: {command.slotID}");
            return;
        }
        slot.IgnoreBrokenThisActionValue = command.ignoreBrokenThisAction;

        if (command.isBroken)
        {
            CharactersManager charactersManager = FindFirstObjectByType<CharactersManager>();
            charactersManager.MentalBonusForAllCharactor(10);

            EnemyStatus status = GetComponent<EnemyStatus>();
            status.MentalityValue -= 10;

            command.isBroken = false;
        }

        slot.RequesSkillManagerForEnemySlot(skillManager, command.skillType);
        slot.CanActValue = command.canAct;

        if (command.useOverrideSpeed) slot.SetOverrideSpeed(command.overrideSpeed);
        else slot.ClearOverrideSpeed();
    }

    public EnemySlot FindSlot(string slotID)
    {
        foreach (EnemySlot slot in slots)
        {
            if (slot != null && slot.SlotID == slotID) return slot;
        }

        return null;
    }

    public BossPart GetPart(string partID)
    {
        foreach (BossPart part in parts)
        {
            if (part != null && part.PartID == partID) return part;
        }

        return null;
    }

    public void HealPart(string partID)
    {
        BossPart part = GetPart(partID);

        if (part != null) part.HealFull();
    }

    public void DamageCore(int damage)
    {
        foreach (BossPart part in parts)
        {
            if (part != null && part.IsCore)
            {
                part.TakeDamage(damage);
                return;
            }
        }
    }

    public void OnPartBroken(BossPart part)
    {
        if (part == null) return;

        

        if (part.IsCore)
        {
            m_enemyStatus.OnDeath();
            return;
        }

        Debug.Log($"{part.PartID} 파괴됨");
    }

    private void ClearSlots()
    {
        foreach (EnemySlot slot in slots)
        {
            if (slot == null) continue;

            slot.CanActValue = false;
            slot.ClearOverrideSpeed();
            slot.RequesSkillManagerForEnemySlot(skillManager, SkillType.None);
            slot.IgnoreBrokenThisActionValue = false;
        }
    }

    private void Awake()
    {
        foreach (BossPart part in parts)
        {
            if (part != null) part.Initialize(this);
        }
    }

}