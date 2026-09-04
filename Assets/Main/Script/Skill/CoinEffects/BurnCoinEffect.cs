using UnityEngine;

[System.Serializable]
public class BurnCoinEffect : CoinEffect
{
    [SerializeField] private int BurnStackPower;
    [SerializeField] private int BurnStackCount;
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

                if (battleAction.Context.EnemySlot != null)
                {
                    BossPartTarget bossTarget = battleAction.Context.EnemySlot.GetComponentInParent<BossPartTarget>();

                    if (bossTarget != null)
                    {
                        targetManager = bossTarget.BossPart.GetComponent<BuffDebuffManager>();
                        break;
                    }
                }

                if (battleAction.Context.TargetEnemy != null)
                {
                    targetManager = battleAction.Context.TargetEnemy.GetComponent<BuffDebuffManager>();
                    break;
                }
                if (battleAction.Context.TargetCharacter != null)
                {
                    targetManager = battleAction.Context.TargetCharacter.GetComponent<BuffDebuffManager>();
                    break;
                }
                break;
        }

        if (targetManager == null) return;

        BurnEffect burnEffect = new BurnEffect();
        burnEffect.SetBurnEffect(BurnStackPower, BurnStackCount);

        switch (turnSet)
        {
            case EffectTurnSet.This:
                targetManager.AddEffect(burnEffect);
                break;
            case EffectTurnSet.Next:
                targetManager.AddEffectNextTurn(burnEffect);
                break;
            case EffectTurnSet.Both:
                targetManager.AddEffect(burnEffect);
                targetManager.AddEffectNextTurn(burnEffect);
                break;
        }
    }
}
