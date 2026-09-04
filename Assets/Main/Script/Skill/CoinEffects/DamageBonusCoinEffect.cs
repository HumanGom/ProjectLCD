using UnityEngine;

[System.Serializable]
public class DamageBonusCoinEffect : CoinEffect
{
    [SerializeField] private float damageBonus;

    public override void Execute(BattleActionContext battleAction, SkillCoinOS coin, int coinPower)
    {
        coin.DamageBonusValue += damageBonus;
    }
}
