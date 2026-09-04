using System;
using System.Collections;
using UnityEngine;

public class BattleAnimationPlayer : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private static readonly int ClashHash = Animator.StringToHash("Clash");
    private static readonly int CoinBreakHash = Animator.StringToHash("CoinBreak");
    private static readonly int BreakingCoinHash = Animator.StringToHash("BreakingCoin");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int DamagedHash = Animator.StringToHash("Damaged");

    private System.Action onAttackHit;
    private EffectSound effectSound;

    [HideInInspector] public bool IsAnimationFinished = false;


    public IEnumerator WaitAnimation(string stateName)
    {
        if (animator == null)
            yield break;

        float enterTimeout = 1f;
        float timer = 0f;

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            timer += Time.deltaTime;

            if (timer >= enterTimeout)
            {
                Debug.LogWarning($"{gameObject.name}이 {stateName} State에 진입하지 못함");
                yield break;
            }

            yield return null;
        }

        while (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        IsAnimationFinished = true;
    }   



    public void PlayClash()
    {
        if (animator == null) return;
        IsAnimationFinished = false;
        animator.SetTrigger(ClashHash);
        if(effectSound != null) effectSound.PlayClashClip();
    }

    public void PlayCoinBreak()
    {
        if (animator == null) return;
        IsAnimationFinished = false;
        animator.SetTrigger(CoinBreakHash);
    }

    public void PlayBreakEnemyCoin()
    {
        if (animator == null) return;
        IsAnimationFinished = false;
        animator.SetTrigger(BreakingCoinHash);
    }

    public void PlayAttack()
    {
        if (animator == null) return;
        IsAnimationFinished = false;
        animator.SetTrigger(AttackHash);
        if (effectSound != null) effectSound.PlayAttackClip();
    }

    public void PlayDamaged()
    {
        if (animator == null) return;
        IsAnimationFinished = false;
        animator.SetTrigger(DamagedHash);
        if (effectSound != null) effectSound.PlayDamagedClip();
    }

    public void SetAttackHitEvent(Action hitAction)
    {
        onAttackHit = hitAction;
    }

    public void OnAttackHit()
    {
        onAttackHit?.Invoke();

        // 한 공격에서 두 번 호출되지 않게 제거
        onAttackHit = null;
    }

    private void Awake()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        effectSound = GetComponentInChildren<EffectSound>();
    }

}