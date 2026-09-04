using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotChanger : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private PlayerSlot slot1;
    [SerializeField] private PlayerSlot slot2;
    [SerializeField] private Image portrait;

    private CharacterSkillManager skillManager;
    private bool isDefenseMode = false;
    private int deckIndex;

    public void Initialize(CharacterSkillManager manager, Sprite portraitSprite, int deckIndex)
    {
        skillManager = manager;
        this.deckIndex = deckIndex;

        if (deckIndex == 0)
        {
            portrait.sprite = portraitSprite;
        }
        else
        {
            portrait.sprite = null;
        }

        isDefenseMode = false;

        slot1.SetSlot(skillManager, skillManager.GetSkill(deckIndex, 0), 0, deckIndex);
        slot2.SetSlot(skillManager, skillManager.GetSkill(deckIndex, 1), 1, deckIndex); 
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleDefenseSlot();
    }

    private void ToggleDefenseSlot()
    {
        if (skillManager == null)
            return;

        isDefenseMode = !isDefenseMode;

        if (isDefenseMode)
        {
            slot1.SetDefenseSlot(skillManager, skillManager.GetDefenseSkillType, 0, deckIndex);
        }
        else
        {
            slot1.SetSlot(skillManager, skillManager.GetSkill(deckIndex, 0), 0, deckIndex);
        }
        HeadSlot headSlot = skillManager.GetHeadSlot;

        if (headSlot != null && headSlot.GetTargetEnemySlot != null)
        {
            headSlot.RequestPlayerSlotAndSkillManager(slot1, skillManager, headSlot.GetTargetEnemySlot);
        }
    }
}
