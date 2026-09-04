using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public abstract class SkillCoinOS : ScriptableObject
{
    [Header("코인 효과")]
    [SerializeReference, SubclassSelector] private List<CoinEffect> effects = new List<CoinEffect>();
    
    private float totalDamageBonus = 0f;
    private int ShieldBonus = 0;
    public float DamageBonusValue { get { return totalDamageBonus; } set { totalDamageBonus = value; } }
    public int ShieldBonusValue { get { return ShieldBonus; } set { ShieldBonus = value; } }
    public List<CoinEffect> GetEffectList { get { return effects; } }

    public abstract int Execute(BattleActionContext battleAction, int coinPower);

    private BuffDebuffManager GetTargetBuffManager(BattleActionContext battleAction)
    {
        if (battleAction.Context.EnemySlot != null)
        {
            BossPartTarget bossTarget =
                battleAction.Context.EnemySlot.GetComponentInParent<BossPartTarget>();

            if (bossTarget != null && bossTarget.BossPart != null)
                return bossTarget.BossPart.GetComponent<BuffDebuffManager>();
        }

        if (battleAction.Context.TargetEnemy != null)
            return battleAction.Context.TargetEnemy.GetComponent<BuffDebuffManager>();

        if (battleAction.Context.TargetCharacter != null)
            return battleAction.Context.TargetCharacter.GetComponent<BuffDebuffManager>();

        return null;
    }

    private BuffDebuffManager GetCasterBuffManager(BattleActionContext battleAction)
    {
        if (battleAction.Context.CastingOBJ != null) 
            return battleAction.Context.CastingOBJ.GetComponent<BuffDebuffManager>();
        return null;
    }

    public int DamageRoutain(BattleActionContext battleAction, int coinPower)
    {
        foreach (var effect in GetEffectList)
        {
            effect?.Execute(battleAction, this, coinPower);
        }

        float damageModifier = 0f;
        float damageDynamicModifier = DamageBonusValue;
        int levelSum = 0;

        BuffDebuffManager targetBuffDebuffManager = null;
        BuffDebuffManager casterBuffDebuffManager = null;

        if (battleAction.Context.CasterCharacter != null)
        {
            levelSum = (battleAction.Context.TargetEnemy.DefenseLevelValue + battleAction.Skill.DefenseLevelValue) - battleAction.Skill.AttackLevelValue;

            damageModifier +=
                battleAction.Context.CasterCharacter.GetElementResistFromList(battleAction.Skill.GetElementType) +
                battleAction.Context.CasterCharacter.GetAttackResistFromList(battleAction.Skill.GetAttackType) -
                (levelSum / ((math.abs(levelSum)) - 25));

            targetBuffDebuffManager = GetTargetBuffManager(battleAction);
            casterBuffDebuffManager = GetCasterBuffManager(battleAction);
        }
        else if (battleAction.Context.CasterEnemy != null)
        {
            levelSum = (battleAction.Context.TargetCharacter.DefenseLevelValue + battleAction.Skill.DefenseLevelValue) - battleAction.Skill.AttackLevelValue;
            //battleAction.Context.TargetCharacter.GetBuffDebuffManager.additionalDefenseLevel - battleAction.Context.CasterEnemy.GetBuffDebuffManager.additionalAttackLevel;

            damageModifier =
                battleAction.Context.CasterEnemy.GetElementResistFromList(battleAction.Skill.GetElementType) +
                battleAction.Context.CasterEnemy.GetAttackResistFromList(battleAction.Skill.GetAttackType);

            targetBuffDebuffManager = GetTargetBuffManager(battleAction);
            casterBuffDebuffManager = GetCasterBuffManager(battleAction);
        }

        if (targetBuffDebuffManager.GetEffect<VulnerableEffect>() != null)
        {
            damageDynamicModifier += (float)targetBuffDebuffManager.GetEffect<VulnerableEffect>().GetvulnerableStack * 0.1f;
            Debug.Log($"타겟의 받는 데미지 증가{(float)targetBuffDebuffManager.GetEffect<VulnerableEffect>().GetvulnerableStack * 0.1f}");
        }

        if (casterBuffDebuffManager.GetEffect<IncreaseDamageEffect>() != null)
        {
            damageDynamicModifier += (float)casterBuffDebuffManager.GetEffect<IncreaseDamageEffect>().GetIncreaseDamageStack * 0.1f;
            Debug.Log($"캐스터의 주는 데미지 증가{(float)casterBuffDebuffManager.GetEffect<IncreaseDamageEffect>().GetIncreaseDamageStack * 0.1f}");
        }

        if (casterBuffDebuffManager.GetEffect<IncreaseAttackLevel>() != null)
        {
            levelSum -= casterBuffDebuffManager.GetEffect<IncreaseAttackLevel>().GetIncreseAttackLevelStack;
        }

        if (casterBuffDebuffManager.GetEffect<DecreaseAttackLevel>() != null)
        {
            levelSum += casterBuffDebuffManager.GetEffect<DecreaseAttackLevel>().GetDecreaseAttackLevelStack;
        }

        damageModifier -= (levelSum / ((math.abs(levelSum)) - 25));
        int totalDamage = Mathf.FloorToInt((coinPower * (1 + damageModifier) * (1 + damageDynamicModifier)));
        //Debug.Log($"최종 데미지{totalDamage}");

        if (battleAction.Context.CasterCharacter != null)
        {
            BossPartTarget bossTarget = battleAction.Context.EnemySlot.GetComponentInParent<BossPartTarget>();

            if (bossTarget != null)
            {
                bossTarget.BossPart.TakeDamage(totalDamage);
                return totalDamage;
            }

            EnemyStatus targetEnemy = battleAction.Context.TargetEnemy;

            if (targetEnemy.ShieldValue >= totalDamage)
            {
                targetEnemy.ShieldValue -= totalDamage;
            }
            else
            {
                totalDamage -= targetEnemy.ShieldValue;
                targetEnemy.HpValue -= totalDamage;
                targetEnemy.ShieldValue = 0;
            }

            return totalDamage;
        }

        if (battleAction.Context.CasterEnemy != null)
        {
            if (battleAction.Context.TargetCharacter.ShieldValue >= totalDamage)
            {
                battleAction.Context.TargetCharacter.ShieldValue -= totalDamage;
            }
            else
            {
                totalDamage -= battleAction.Context.TargetCharacter.ShieldValue;
                battleAction.Context.TargetCharacter.HpValue -= totalDamage;
                battleAction.Context.TargetCharacter.ShieldValue = 0;
            }
            //Debug.Log($"{battleAction.Context.CasterEnemy}가 {battleAction.Context.TargetCharacter}에게 {totalDamage}의 피해를 줘서 {battleAction.Context.TargetCharacter.HpValue}의 체력이 남음");
            return totalDamage;
        }
        return 0;
    }
}