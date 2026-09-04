using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleAnimationManager : MonoBehaviour
{
    [SerializeField] private BattleCameraManager battleCameraManager;

    private Transform focusA;
    private Transform focusB;
    private Vector3 focusAOriginal;
    private Vector3 focusBOriginal;
    private bool isFocused = false;

    public IEnumerator PlayCoinAction(BattleActionContext attacker, SkillCoinOS coin, int coinPower, bool isFront, int i, System.Action onHit)
    {
        UsingSkillUI usingSkillUI = GetUsingSkillUI(attacker);
        if (usingSkillUI == null) yield break;

        if (i == 0)
        {
            usingSkillUI.ShowSkill(attacker.Skill, attacker.RemainingCoins);
        }

        usingSkillUI.SetCoinResult(i, isFront);
        usingSkillUI.SetPower(coinPower);

        BattleAnimationPlayer attackerAnim = GetAnim(attacker);
        BattleAnimationPlayer targetAnim = GetTargetAnim(attacker);

        if (attackerAnim != null)
        {
            // 타격 프레임에서 실행할 함수 등록
            attackerAnim.SetAttackHitEvent(onHit);
            attackerAnim.PlayAttack();
        }
        if (targetAnim != null) targetAnim.PlayDamaged();

        yield return WaitBothAnimations(attackerAnim, "Attack", targetAnim, "Damaged");
    }

    public IEnumerator PlayDefenseAction(BattleActionContext action, SkillCoinOS coin, int coinPower, bool isFront, int i, System.Action onHit)
    {
        UsingSkillUI usingSkillUI = GetUsingSkillUI(action);

        if (usingSkillUI == null) yield break;

        if (i == 0)
        {
            usingSkillUI.ShowSkill(action.Skill, action.RemainingCoins);
        }

        usingSkillUI.SetCoinResult(i, isFront);
        usingSkillUI.SetPower(coinPower);

        BattleAnimationPlayer anim = GetAnim(action);

/*        if (anim != null)
        {
            anim.PlayDefense();
            yield return anim.WaitAnimation("Defense");
        }*/
    }

    public IEnumerator PlayClash(BattleActionContext a, BattleActionContext b, int aPower, int bPower, List<bool> aCoinResults, List<bool> bCoinResults)
    {
        UsingSkillUI aUI = GetUsingSkillUI(a);
        UsingSkillUI bUI = GetUsingSkillUI(b);

        if (aUI != null) aUI.ShowSkill(a.Skill, a.RemainingCoins);
        aUI.SetPower(aPower);
        if (bUI != null) bUI.ShowSkill(b.Skill, b.RemainingCoins);
        bUI.SetPower(bPower);

        for (int i = 0; i < aCoinResults.Count; i++)
        {
            aUI.SetCoinResult(i, aCoinResults[i]);
        }

        for (int i = 0; i < bCoinResults.Count; i++)
        {
            bUI.SetCoinResult(i, bCoinResults[i]);
        }

        BattleAnimationPlayer aAnim = GetAnim(a);
        BattleAnimationPlayer bAnim = GetAnim(b);

        if (aAnim != null) aAnim.PlayClash();
        if (bAnim != null) bAnim.PlayClash();

        yield return WaitBothAnimations(aAnim, "Clash", bAnim, "Clash");
    }

/*    public IEnumerator PlayCoinBreak(BattleActionContext loser)
    {
        UsingSkillUI loserUI = loser.Context.CastingOBJ.GetComponentInChildren<UsingSkillUI>();

        if (loserUI != null)
        {
            loserUI.BreakCoin(0);
        }

        BattleAnimationPlayer loserAnim = GetAnim(loser);
        if (loserAnim != null) loserAnim.PlayCoinBreak();

        yield return loserAnim.WaitAnimation("CoinBreak");
    }*/

    public IEnumerator PlayCoinBreakResult(BattleActionContext winner, BattleActionContext loser)
    {
        BattleAnimationPlayer winnerAnim = GetAnim(winner);
        BattleAnimationPlayer loserAnim = GetAnim(loser);

        if (winnerAnim != null) winnerAnim.PlayBreakEnemyCoin();

        if (loserAnim != null) loserAnim.PlayCoinBreak();

        yield return WaitBothAnimations(winnerAnim, "BreakingCoin", loserAnim, "CoinBreak");
    }

    public IEnumerator BeginFocus(BattleActionContext a, BattleActionContext b)
    {
        if (isFocused) yield break;

        focusA = GetActorTransform(a);
        focusB = GetActorTransform(b);

        if (focusA == null || focusB == null) yield break;

        focusAOriginal = focusA.position;
        focusBOriginal = focusB.position;

        isFocused = true;

        yield return battleCameraManager.FocusTwoCharacters(focusA, focusB, a.Side, b.Side);
    }

    public IEnumerator EndFocus()
    {
        if (!isFocused || focusA == null || focusB == null) yield break;

        yield return battleCameraManager.EndFocus(focusA, focusAOriginal, focusB, focusBOriginal);

        focusA = null;
        focusB = null;
        isFocused = false;
    }

    private UsingSkillUI GetUsingSkillUI(BattleActionContext action)
    {
        if (action.Side == BattleSide.Player && action.Context.CasterCharacter != null)
            return action.Context.CasterCharacter.GetUsingSkillUI;

        if (action.Side == BattleSide.Enemy && action.Context.CasterEnemy != null)
            return action.Context.CasterEnemy.GetUsingSkillUI;

        return null;
    }

    private Transform GetActorTransform(BattleActionContext action)
    {
        if (action.Side == BattleSide.Player && action.Context.CasterCharacter != null)
            return action.Context.CasterCharacter.transform;

        if (action.Side == BattleSide.Enemy && action.Context.CasterEnemy != null)
            return action.Context.CasterEnemy.transform;

        return null;
    }

    private BattleAnimationPlayer GetAnim(BattleActionContext action)
    {
        Transform actor = GetActorTransform(action);

        if (actor == null) return null;

        return actor.GetComponentInChildren<BattleAnimationPlayer>();
    }

    private Transform GetTargetTransform(BattleActionContext action)
    {
        if (action.Context.TargetCharacter != null) return action.Context.TargetCharacter.transform;

        if (action.Context.TargetEnemy != null) return action.Context.TargetEnemy.transform;

        return null;
    }
    private BattleAnimationPlayer GetTargetAnim(BattleActionContext action)
    {
        Transform target = GetTargetTransform(action);

        if (target == null) return null;

        return target.GetComponentInChildren<BattleAnimationPlayer>();
    }

    private IEnumerator WaitBothAnimations(BattleAnimationPlayer a, string aState, BattleAnimationPlayer b, string bState)
    {
        Coroutine aRoutine = null;
        Coroutine bRoutine = null;

        if (a != null) aRoutine = StartCoroutine(a.WaitAnimation(aState));

        if (b != null) bRoutine = StartCoroutine(b.WaitAnimation(bState));

        if (aRoutine != null) yield return aRoutine;

        if (bRoutine != null) yield return bRoutine;
    }

    public IEnumerator BeginAttackFocus(BattleActionContext attacker)
    {
        Transform attackerTransform = GetActorTransform(attacker);
        Transform targetTransform = GetTargetTransform(attacker);

        if (attackerTransform == null || targetTransform == null) yield break;

        BattleSide targetSide = BattleSide.None;

        if (attacker.Context.TargetCharacter != null) targetSide = BattleSide.Player;
        else if (attacker.Context.TargetEnemy != null) targetSide = BattleSide.Enemy;

        focusA = attackerTransform;
        focusB = targetTransform;
        focusAOriginal = focusA.position;
        focusBOriginal = focusB.position;
        isFocused = true;

        yield return battleCameraManager.FocusTwoCharacters(focusA, focusB, attacker.Side, targetSide);
    }
}