using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerSlot : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [Header("스킬종류")]
    [SerializeField] private SkillType skillType;
    [Header("스킬 배열 순서(위 1/ 아래 0")]
    [SerializeField] private int skillIndex = -1;

    private HeadSlot linkedHeadSlot;
    private CharacterSkillManager ownerSkillManager;
    private CanvasGroup canvasGroup;
    private Vector3 DefaultPos;
    private RectTransform rect;
    private bool isDefenseSkill = false;
    private int deckIndex = 0;

    public SkillType GetSkillType { get { return skillType; } }
    public int GetSkillIndex { get { return skillIndex; } }
    public bool IsDefenseSkill { get {return isDefenseSkill; } }
    public int GetDeckIndex { get { return deckIndex; } }
    public CharacterSkillManager GetSkillManager()
    {
        return ownerSkillManager;
    }

    public void PassingSlotImport(EnemySlot enemySlot)
    {
        ownerSkillManager.RequestFromHeadSlot(this, linkedHeadSlot, enemySlot);
    }

    public void SetSlot(CharacterSkillManager manager, SkillType skill, int skillIndex, int deckIndex)
    {
        ownerSkillManager = manager;
        skillType = skill;
        this.skillIndex = skillIndex;
        this.deckIndex = deckIndex;
        isDefenseSkill = false;
        SkillObjectOS skillObject = manager.GetSkillObject(skillType);
        ApplySkillVisual(skillObject);
    }

    public void SetDefenseSlot(CharacterSkillManager manager, SkillType defenseSkillType, int index, int deckIndex)
    {
        ownerSkillManager = manager;
        skillType = defenseSkillType;
        skillIndex = index;
        isDefenseSkill = true;

        SkillObjectOS skillObject = manager.GetSkillObject(skillType);
        ApplySkillVisual(skillObject);
    }

    public void SetLinkedHeadSlot(HeadSlot headSlot)
    {
        linkedHeadSlot = headSlot;
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("드래그시작");
        canvasGroup.blocksRaycasts = false;

        rect = GetComponent<RectTransform>();

        Vector3 mouseWorld = GetMouseWorldPos(eventData);
        DefaultPos = transform.position - mouseWorld;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        //Debug.Log("드래그중");
        Vector3 mouseWorld = GetMouseWorldPos(eventData);
        transform.position = mouseWorld + DefaultPos;
        rect.position = eventData.position;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("드래그완료");

        rect.position = DefaultPos;
        canvasGroup.blocksRaycasts = true;
    }

    Vector3 GetMouseWorldPos(PointerEventData eventData)
    {
        Vector3 screenPos = eventData.position;

        screenPos.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);

        return Camera.main.ScreenToWorldPoint(screenPos);
    }

    private void ApplySkillVisual(SkillObjectOS skillObject)
    {
        Image image = GetComponent<Image>();

        if (skillObject == null)
        {
            image.sprite = null;
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

    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

}
