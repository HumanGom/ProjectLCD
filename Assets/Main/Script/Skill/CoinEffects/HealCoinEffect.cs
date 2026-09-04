using UnityEngine;

[System.Serializable]
public class HealCoinEffect : CoinEffect
{
    [SerializeField] private EffectTargetSet targetSet;

    public override void Execute(BattleActionContext battleAction, SkillCoinOS coin, int coinPower)
    {
        switch (targetSet)
        {
            case EffectTargetSet.Caster:
                if (battleAction.Context.CastingOBJ != null)
                {
                    if (battleAction.Context.CasterCharacter != null)
                    {
                        Debug.Log($"{battleAction.Context.CastingOBJ.name}이 {coinPower}의 체력을 회복함");
                        battleAction.Context.CasterCharacter.HpValue += coinPower;
                        return;
                    }
                    if (battleAction.IsCasterBoss)
                    {
                        Debug.Log($"{battleAction.Context.CastingOBJ.name}이 {coinPower}의 체력을 회복함");
                        BossPart bossPart = battleAction.Context.CastingOBJ.GetComponent<BossPart>();
                        bossPart.HpValue += coinPower;
                        return;
                    }
                    if (battleAction.Context.CasterEnemy != null)
                    {
                        Debug.Log($"{battleAction.Context.CastingOBJ.name}이 {coinPower}의 체력을 회복함");
                        battleAction.Context.CasterEnemy.HpValue += coinPower;
                        return;
                    }
                }
                break;

            case EffectTargetSet.Target:

                if (battleAction.Context.TargetCharacter != null)
                {
                    Debug.Log($"{battleAction.Context.TargetCharacter.name}이 {coinPower}의 체력을 회복함");
                    battleAction.Context.TargetCharacter.HpValue += coinPower;
                    return;
                }
                else
                {
                    if (battleAction.Context.TargetEnemy != null)
                    {
                        Debug.Log($"{battleAction.Context.TargetEnemy.name}이 {coinPower}의 체력을 회복함");
                        battleAction.Context.TargetEnemy.HpValue += coinPower;
                        return;
                    }
                    else
                    {

                        BossPart bossPart = battleAction.Context.CastingOBJ.GetComponent<BossPart>();
                        if (bossPart == null) return;
                        Debug.Log($"{bossPart.name}이 {coinPower}의 체력을 회복함");
                        bossPart.HpValue = coinPower;
                        return;
                    }
                }
                break;
        }
    }
}
