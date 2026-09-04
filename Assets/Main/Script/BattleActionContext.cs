using System.Collections.Generic;

public class BattleActionContext
{
    public BattleSide Side;
    public bool IsCasterBoss;
    public bool IgnoreBrokenThisAction;

    public HeadSlot HeadSlot;
    public EnemySlot EnemySlot;

    public SkillObjectOS Skill;
    public SkillContext Context;

    public List<SkillCoinOS> RemainingCoins = new List<SkillCoinOS>();

    public int Speed;
    public bool IsActed;

    public bool IsCasterDead
    {
        get
        {
            if (IsCasterBoss)
            {
                if (IgnoreBrokenThisAction) return false;

                BossPart bossPart = Context.CastingOBJ.GetComponent<BossPart>();
                return bossPart != null && bossPart.IsBroken;
            }
            if (Context.CasterCharacter != null)
            {
                return Context.CasterCharacter.IsDead;
            }
            else if (Context.CasterEnemy != null)
            {
                return Context.CasterEnemy.IsDead;
            }
            return true;
        }
    }

    public bool IsTargetDead
    {
        get
        {
            if (Context.TargetCharacter != null)
            { 
                return Context.TargetCharacter.IsDead;
            }

            if (Context.TargetEnemy != null)
            {
                return Context.TargetEnemy.IsDead;
            }
            return true;
        }
    }
}