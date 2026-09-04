using UnityEngine;

[System.Serializable]
public class AddShield : CoinEffect
{
    public override void Execute(BattleActionContext battleAction, SkillCoinOS coin, int coinPower)
    {
        if(battleAction.Context.CasterCharacter != null)
        {
            battleAction.Context.CasterCharacter.ShieldValue += coinPower;
            return;
        }
        if(battleAction.IsCasterBoss)
        {
            BossPart bossPart = battleAction.Context.CastingOBJ.GetComponent<BossPart>();
            bossPart.ShieldValue += coinPower;
            return;
        }
        if(battleAction.Context.CasterEnemy != null)
        {
            battleAction.Context.CasterEnemy.ShieldValue += coinPower;
            return;
        }
    }
}
