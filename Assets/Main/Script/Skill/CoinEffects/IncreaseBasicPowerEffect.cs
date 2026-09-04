using UnityEngine;

[System.Serializable]
public class IncreaseBasicPowerEffect : CoinEffect
{
    [SerializeField] private int IncreaseBasicPowerStack;
    [SerializeField] private EffectTurnSet turnSet;
    [SerializeField] private EffectTargetSet targetSet;

    public override void Execute(BattleActionContext battleAction, SkillCoinOS coin, int coinPower)
    {
        BuffDebuffManager targetManager = null;

        switch (targetSet)
        {
            case EffectTargetSet.Caster:
                if (battleAction.Context.CastingOBJ != null)
                {
                    targetManager = battleAction.Context.CastingOBJ.GetComponent<BuffDebuffManager>();
                }
                break;
            case EffectTargetSet.Target:
                if (battleAction.Context.TargetEnemy != null)
                {
                    targetManager = battleAction.Context.TargetEnemy.GetComponent<BuffDebuffManager>();
                }
                else if (battleAction.Context.TargetCharacter != null)
                {
                    targetManager = battleAction.Context.TargetCharacter.GetComponent<BuffDebuffManager>();
                }
                break;
        }


        if (targetManager == null) return;

        IncreaseBasicPower increaseBasicPower = new IncreaseBasicPower();
        increaseBasicPower.SetIncreaseBasicPowerStack(IncreaseBasicPowerStack);


        switch (turnSet)
        {
            case EffectTurnSet.This:
                targetManager.AddEffect(increaseBasicPower);
                break;
            case EffectTurnSet.Next:
                targetManager.AddEffectNextTurn(increaseBasicPower);
                break;
            case EffectTurnSet.Both:
                targetManager.AddEffect(increaseBasicPower);
                targetManager.AddEffectNextTurn(increaseBasicPower);
                break;
        }
    }
}
