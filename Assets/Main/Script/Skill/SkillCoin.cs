using System.Collections.Generic;
using System.Xml.Schema;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Coin")]
public class SkillCoin : SkillCoinOS
{
    public override int Execute(BattleActionContext battleAction, int coinPower)
    {
        //Debug.Log($"SkillCoin Execute ½ÇÇà: {name}");
        return DamageRoutain(battleAction, coinPower);
        
    }
}
