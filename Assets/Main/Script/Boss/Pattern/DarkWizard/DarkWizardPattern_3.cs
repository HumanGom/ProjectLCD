using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class DarkWizardPattern_3 : BossPattern
{
    [Header("본체 슬롯 ID")]
    [SerializeField] private string bodySlot1 = "Body_1";
    [SerializeField] private string bodySlot2 = "Body_2";
    [SerializeField] private string bodySlot3 = "Body_3";

    [Header("지팡이")]
    [SerializeField] private string staffPart = "Staff";
    [SerializeField] private string staffSlot = "Staff";

    [Header("스킬")]
    [SerializeField] private SkillType skill2_1 = SkillType.Skill2_1;
    [SerializeField] private SkillType strongSkill3_1 = SkillType.Skill3_1;
    [SerializeField] private SkillType defenseSkill = SkillType.Defense;

    [Header("지팡이 강화 패턴 속도")]
    [SerializeField] private int staffOverrideSpeed = 10;

    public override void Execute(BossController boss)
    {
        BossPart staff = boss.GetPart(staffPart);

        if (staff != null && staff.IsBroken)
        {
            boss.ApplyCommand(new BossSlotCommand
            {
                slotID = bodySlot1,
                skillType = SkillType.None,
                canAct = false
            });

            boss.ApplyCommand(new BossSlotCommand
            {
                slotID = bodySlot2,
                skillType = SkillType.None,
                canAct = false
            });

            boss.ApplyCommand(new BossSlotCommand
            {
                slotID = bodySlot3,
                skillType = skill2_1,
                canAct = true
            });

            boss.ApplyCommand(new BossSlotCommand
            {
                slotID = staffSlot,
                skillType = SkillType.None,
                canAct = false,
                isBroken = true
            });

            return;
        }

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
            skillType = defenseSkill,
            canAct = true
        });

        boss.ApplyCommand(new BossSlotCommand
        {
            slotID = staffSlot,
            skillType = strongSkill3_1,
            canAct = true,
            useOverrideSpeed = true,
            overrideSpeed = staffOverrideSpeed,
            ignoreBrokenThisAction = true
        });
    }
}