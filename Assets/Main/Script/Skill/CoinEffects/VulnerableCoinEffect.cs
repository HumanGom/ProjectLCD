using UnityEngine;

[System.Serializable]
public class VulnerableCoinEffect : CoinEffect
{
    [SerializeField] private int vulnerableStackPower;
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


        VulnerableEffect vulnerableEffect = new VulnerableEffect();
        vulnerableEffect.SetVulnerableStack(vulnerableStackPower);

        switch (turnSet)
        {
            case EffectTurnSet.This:
                targetManager.AddEffect(vulnerableEffect);
                break;
            case EffectTurnSet.Next:
                targetManager.AddEffectNextTurn(vulnerableEffect);
                break;
            case EffectTurnSet.Both:
                targetManager.AddEffect(vulnerableEffect);
                targetManager.AddEffectNextTurn(vulnerableEffect);
                break;
        }
    }
}
