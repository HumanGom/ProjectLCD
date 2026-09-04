using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemySlot : MonoBehaviour, IDropHandler
{
    [Header("선택된 스킬 종류")]
    [SerializeField] private SkillType skillType;
    [Header("스킬 미지정 기본 아이콘")]
    [SerializeField] private Sprite defaultSlotIcon;
    [Header("적 스킬 매니저")]
    [SerializeField] private EnemySkillManager enemySkillManager;
    [Header("슬롯ID")]
    [SerializeField] private string slotID;
    [Header("슬롯 행동가능 여부")]
    [SerializeField] private bool canAct = true;

    private bool useOverrideSpeed = false;
    private int overrideSpeed = 0;
    private ArrowSplineMesh arrowSplineMesh;
    private PlayerSlot dragedSlot;
    private Vector3 DefaultPos;
    private RectTransform rect;
    private HeadSlot defaultTargetHeadSlot;
    private HeadSlot targetHeadSlot;
    private bool ignoreBrokenThisAction = false;
    private readonly List<HeadSlot> targetHistory = new List<HeadSlot>();
    

    public bool IgnoreBrokenThisActionValue { get { return ignoreBrokenThisAction; } set { ignoreBrokenThisAction = value; } }
    public SkillObjectOS GetSkillObject { get { return enemySkillManager.GetSkillObject(skillType); } }
    public EnemyStatus GetEnemyStatus { get { return GetComponentInParent<EnemyStatus>(); } }
    public HeadSlot GetTargetHeadSlot { get { return targetHeadSlot; } }
    public ArrowSplineMesh GetArrowSplineMesh { get { return arrowSplineMesh; } }
    public HeadSlot GetEnemyTarget { get { return targetHeadSlot; } }
    public bool CanActValue { get { return canAct; } set { canAct = value; } }
    public string SlotID => slotID;

    public void SetArrowSplineMesh(ArrowSplineMesh arrow)
    {
        arrowSplineMesh = arrow;
    }

    public void OnDrop(PointerEventData eventData)
    {
        dragedSlot = eventData.pointerDrag.GetComponent<PlayerSlot>();

        dragedSlot.PassingSlotImport(this);
    }

    public void RegisterHeadSlotHistory(HeadSlot headSlot)
    {
        if (headSlot == null) return;

        targetHistory.Remove(headSlot);
        targetHistory.Add(headSlot);
    }

    public void SetTurnDefaultTarget(HeadSlot headSlot)
    {
        defaultTargetHeadSlot = headSlot;
        targetHeadSlot = headSlot;
    }

    public void RestoreDefaultTarget()
    {
        if (defaultTargetHeadSlot == null || !defaultTargetHeadSlot.TryGetCharacterStatus(out CharacterStatus status) || status.IsDead)
        {
            targetHeadSlot = null;
            DrawNormalLine();
            return;
        }

        targetHeadSlot = defaultTargetHeadSlot;
        DrawNormalLine();
    }

    public void DrawNormalLine()
    {
        if (arrowSplineMesh == null) return;

        if (!ShouldDrawEnemyLine())
        {
            arrowSplineMesh.ClearLine();
            return;
        }

        if (targetHeadSlot == null)
        {
            arrowSplineMesh.ClearLine();
            return;
        }

        arrowSplineMesh.DrawLine(transform.position, targetHeadSlot.transform.position, 1f);
    }

    public void DrawLineToCenter(Vector3 centerPos, float curveHeight)
    {
        arrowSplineMesh.DrawLine(transform.position, centerPos, curveHeight);
    }

    public HeadSlot FindFirstConflictableHeadSlot()
    {
        for (int i = targetHistory.Count - 1; i >= 0; i--)
        {
            HeadSlot headSlot = targetHistory[i];

            if (headSlot == null) continue;

            bool isTargetingThisEnemy = headSlot.GetTargetEnemySlot == this;

            if (!isTargetingThisEnemy) continue;

            bool enemyTargetsThisHead = targetHeadSlot == headSlot;
            bool isFaster = headSlot.GetCharacterSpeed() > GetEnemySpeed();

            if (enemyTargetsThisHead || isFaster) return headSlot;
        }

        return null;
    }

    public int GetEnemySpeed()
    {
        if (useOverrideSpeed) return overrideSpeed;

        return GetComponentInParent<EnemyStatus>().GetSpeed;
    }

    public void SetTargetHeadSlot(HeadSlot headSlot)
    {
        targetHeadSlot = headSlot;
    }

    public void RequesSkillManagerForEnemySlot(EnemySkillManager manager, SkillType selectedSkill)
    {
        enemySkillManager = manager;
        skillType = selectedSkill;

        SkillObjectOS skillObject = null;

        if (enemySkillManager != null && skillType != SkillType.None)
        {
            skillObject = enemySkillManager.GetSkillObject(skillType);
        }

        ApplySkillVisual(skillObject);
    }

    public void RequestSetEnemySlot()
    {
        HeadSlot[] allSlots = FindObjectsByType<HeadSlot>(FindObjectsSortMode.None);
        List<HeadSlot> validTargets = new();

        foreach (HeadSlot slot in allSlots)
        {
            if (slot == null) continue;

            if (!slot.TryGetCharacterStatus(out CharacterStatus status)) continue;

            if (status == null) continue;

            if (status.IsDead) continue;

            validTargets.Add(slot);
        }

        if (validTargets.Count == 0)
        {
            targetHeadSlot = null;
            defaultTargetHeadSlot = null;
            arrowSplineMesh.ClearLine();
            return;
        }

        HeadSlot randomHeadSlot = validTargets[Random.Range(0, validTargets.Count)];

        SetTurnDefaultTarget(randomHeadSlot);

        TargetConflictManager.Instance.ResolveEnemyTarget(this, randomHeadSlot);
    }

    public void RestoreTurnDefaultTarget()
    {
        if (defaultTargetHeadSlot == null)
        {
            targetHeadSlot = null;
            arrowSplineMesh.ClearLine();
            return;
        }

        targetHeadSlot = defaultTargetHeadSlot;
        DrawNormalLine();
    }

    public void EnemyDrawLine(HeadSlot headSlot)
    {
        targetHeadSlot = headSlot;
        arrowSplineMesh.DrawLine(transform.position, targetHeadSlot.transform.position, 1f);
    }

    public void DrawLineToCenter(Vector3 centerPos, HeadSlot headSlot)
    {
        arrowSplineMesh.DrawLine(transform.position, centerPos, 1f);
        targetHeadSlot = headSlot;
    }

    public void RemoveHeadSlotHistory(HeadSlot headSlot)
    {
        if (headSlot == null) return;

        targetHistory.Remove(headSlot);
    }

    public bool ShouldDrawEnemyLine()
    {
        if (!canAct) return false;

        if (skillType == SkillType.None) return false;

        if (GetSkillObject == null) return false;

        return true;
    }

    public void TurnOffImage()
    {
        this.GetComponent<Image>().enabled = false;
        arrowSplineMesh.TurnOff();
    }

    public void TurnOnImage()
    {
        this.GetComponent<Image>().enabled = true;
        arrowSplineMesh.TurnOn();   
    }

    public void SetOverrideSpeed(int speed)
    {
        useOverrideSpeed = true;
        overrideSpeed = speed;
    }

    public void ClearOverrideSpeed()
    {
        useOverrideSpeed = false;
        overrideSpeed = 0;
    }

    private void ApplySkillVisual(SkillObjectOS skillObject)
    {
        Image image = GetComponent<Image>();

        if (skillObject == null)
        {
            image.sprite = defaultSlotIcon;
            image.color = Color.white;
            return;
        }

        if (skillObject.Icon != null)
        {
            image.sprite = skillObject.Icon;
            image.color = Color.white;
        }
        else
        {
            image.sprite = null;
            image.color = skillObject.SlotColor;
        }
    }
}