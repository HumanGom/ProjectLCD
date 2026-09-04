using UnityEngine;

[System.Serializable]
public abstract class CoinEffect
{
    public abstract void Execute(BattleActionContext battleAction, SkillCoinOS coin, int coinPower);
}