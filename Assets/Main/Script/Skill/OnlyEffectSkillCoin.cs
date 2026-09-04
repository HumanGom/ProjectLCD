using System.Collections.Generic;
using System.Xml.Schema;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/OnlyEffectCoin")]
public class OnlyEffectSkillCoin : SkillCoinOS
{
    public override int Execute(BattleActionContext battleAction, int coinPower)
    {
        foreach (var effect in GetEffectList)
        {
            effect?.Execute(battleAction, this, coinPower);
        }
        return 0;
    }
}