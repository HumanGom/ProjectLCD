using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class HeadSlot : MonoBehaviour
{
    [Header("스킬종류")]
    [SerializeField] private SkillType skillType;
    [Header("화살표라인매쉬")]
    [SerializeField] GameObject arrowSplineMeshOBJ;
    [Header("스킬 미지정 기본 이미지")]
    [SerializeField] private Sprite defaultSprite;

    private PlayerSlot playerSlot;
    private EnemySlot targetEnemySlot;
    private CharacterSkillManager characterSkillManager;
    private ArrowSplineMesh arrowSplineMesh;
    private CharacterStatus ownerCharacterStatus;

    public SkillObjectOS GetSkillObject {  get { return characterSkillManager.GetSkillObject(skillType); } }
    public CharacterSkillManager GetSkillManager { get { return characterSkillManager; } }
    public int GetUsedSkillIndex { get { return playerSlot.GetSkillIndex; } }
    public PlayerSlot GetPlayerSlot { get { return playerSlot; } }
    public int GetUsedDeckIndex { get { return playerSlot.GetDeckIndex; } }
    public CharacterStatus GetCharacterStatus
    {
        get
        {
            if (ownerCharacterStatus != null) return ownerCharacterStatus;

            if (characterSkillManager == null) return null;

            return characterSkillManager.GetComponent<CharacterStatus>();
        }
    }

    public void InitializeOwner(CharacterStatus status, CharacterSkillManager skillManager)
    {
        ownerCharacterStatus = status;
        characterSkillManager = skillManager;
    }

    public bool HasSkill()
    {
        return skillType != SkillType.None;
    }

    public bool HasTarget()
    {
        return targetEnemySlot != null;
    }

    public void ResetSlot()
    {

        Image image = GetComponent<Image>();
        image.sprite = defaultSprite;
        image.color = Color.white;
        skillType = SkillType.None;
        playerSlot = null;
        if (arrowSplineMesh != null) arrowSplineMesh.ClearLine();
    }



    public EnemySlot GetTargetEnemySlot { get { return targetEnemySlot; } }

    public ArrowSplineMesh GetArrowSplineMesh {  get { return arrowSplineMesh; } }

    public int GetCharacterSpeed()
    {
        if (characterSkillManager == null)
            return 0;

        return characterSkillManager.GetComponent<CharacterStatus>().GetSpeed;
    }

    public void SetTargetEnemySlot(EnemySlot enemySlot)
    {
        targetEnemySlot = enemySlot;
    }

    public void DrawNormalLine()
    {
        if (targetEnemySlot == null)
        {
            arrowSplineMesh.ClearLine();
            return;
        }

        arrowSplineMesh.DrawLine(transform.position, targetEnemySlot.transform.position, 1f);
    }

    public void DrawLineToCenter(Vector3 centerPos, float curveHeight)
    {
        arrowSplineMesh.DrawLine(transform.position, centerPos, curveHeight);
    }
    public void RequestPlayerSlotAndSkillManager(PlayerSlot _playerSlot, CharacterSkillManager _characterSkillManager, EnemySlot enemySlot)
    {
        playerSlot = _playerSlot;
        characterSkillManager = _characterSkillManager;
        skillType = playerSlot.GetSkillType;

        SkillObjectOS skillObject = characterSkillManager.GetSkillObject(skillType);
        ApplySkillVisual(skillObject);

        TargetConflictManager.Instance.ResolvePlayerTarget(this, enemySlot);
    }

    public void SetArrowSplineMesh(ArrowSplineMesh arrow)
    {
        arrowSplineMesh = arrow;
    }

    public bool TryGetCharacterStatus(out CharacterStatus status)
    {
        status = GetCharacterStatus;
        return status != null;
    }

    public void TurnOffImage()
    {
        Image image = GetComponent<Image>();

        if (image != null) image.enabled = false;

        if (arrowSplineMesh != null) arrowSplineMesh.TurnOff();
    }
    public void TurnOnImage()
    {
        Image image = GetComponent<Image>();

        if (image != null) image.enabled = true;

        if (arrowSplineMesh != null) arrowSplineMesh.TurnOn();
    }

    private void ApplySkillVisual(SkillObjectOS skillObject)
    {
        Image image = GetComponent<Image>();

        if (skillObject == null)
        {
            image.sprite = defaultSprite;
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
            image.sprite = defaultSprite;
            image.color = skillObject.SlotColor;
        }
    }
}
