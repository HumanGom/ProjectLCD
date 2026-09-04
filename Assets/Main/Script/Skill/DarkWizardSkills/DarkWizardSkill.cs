using NUnit.Framework;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(menuName = "Skill/DarkWizard")]
public class DarkWizardSkill : SkillObjectOS
{
    public override void Execute(SkillContext context)
    {
        List<SkillCoinOS> savedSkillCoin = new List<SkillCoinOS>(GetSkillCoinList);
        
    }
}
