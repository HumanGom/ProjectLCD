using UnityEngine;

[System.Serializable]
public class DarkWizardPattern_2 : BossPattern
{
    [Header("∫ª√º ΩΩ∑‘ ID")]
    [SerializeField] private string bodySlot1 = "Body_1";
    [SerializeField] private string bodySlot2 = "Body_2";
    [SerializeField] private string bodySlot3 = "Body_3";

    [Header("¡ˆ∆Œ¿Ã")]
    [SerializeField] private string staffPart = "Staff";
    [SerializeField] private string staffSlot = "Staff";

    [Header("Ω∫≈≥")]
    [SerializeField] private SkillType skill2 = SkillType.Skill2;
    [SerializeField] private SkillType defenseSkill = SkillType.Defense;

    public override void Execute(BossController boss)
    {
        boss.HealPart(staffPart);

        boss.ApplyCommand(new BossSlotCommand
        {
            slotID = bodySlot1,
            skillType = defenseSkill,
            canAct = true
        });

        boss.ApplyCommand(new BossSlotCommand
        {
            slotID = bodySlot2,
            skillType = defenseSkill,
            canAct = true
        });

        boss.ApplyCommand(new BossSlotCommand
        {
            slotID = bodySlot3,
            skillType = skill2,
            canAct = true
        });

        boss.ApplyCommand(new BossSlotCommand
        {
            slotID = staffSlot,
            skillType = defenseSkill,
            canAct = true
        });
    }
}