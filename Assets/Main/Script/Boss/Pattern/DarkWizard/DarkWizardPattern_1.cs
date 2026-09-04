using UnityEngine;

[System.Serializable]
public class DarkWizardPattern_1 : BossPattern
{
    [Header("∫ª√º ΩΩ∑‘ ID")]
    [SerializeField] private string bodySlot1 = "Body_1";
    [SerializeField] private string bodySlot2 = "Body_2";
    [SerializeField] private string bodySlot3 = "Body_3";

    [Header("¡ˆ∆Œ¿Ã ΩΩ∑‘ ID")]
    [SerializeField] private string staffSlot = "Staff";

    [Header("Ω∫≈≥")]
    [SerializeField] private SkillType skill1 = SkillType.Skill1;
    [SerializeField] private SkillType skill3 = SkillType.Skill3;

    public override void Execute(BossController boss)
    {
        boss.ApplyCommand(new BossSlotCommand
        {
            slotID = bodySlot1,
            skillType = skill1,
            canAct = true
        });

        boss.ApplyCommand(new BossSlotCommand
        {
            slotID = bodySlot2,
            skillType = skill1,
            canAct = true
        });

        boss.ApplyCommand(new BossSlotCommand
        {
            slotID = bodySlot3,
            skillType = skill3,
            canAct = true
        });

        boss.ApplyCommand(new BossSlotCommand
        {
            slotID = staffSlot,
            skillType = SkillType.None,
            canAct = false
        });
    }
}