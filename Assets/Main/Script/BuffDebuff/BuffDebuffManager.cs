using System.Collections.Generic;
using UnityEngine;

public class BuffDebuffManager : MonoBehaviour
{
    private readonly List<BuffDebuffEffect> effects = new List<BuffDebuffEffect>();

    private CharacterStatus characterStatus;
    private EnemyStatus enemyStatus;
    private BossPart bossPart;

    public System.Action<BuffDebuffEffect> OnEffectAdded;
    public System.Action<BuffDebuffEffect> OnEffectRemoved;
    public System.Action<BuffDebuffEffect> OnEffectChanged;

    private List<BuffDebuffEffect> reservedEffects = new();

    public void AddEffect(BuffDebuffEffect newEffect)
    {
        foreach (BuffDebuffEffect effect in effects)
        {
            if (effect.CanMerge(newEffect))
            {
                effect.Merge(newEffect);
                OnEffectChanged?.Invoke(effect);
                return;
            }
        }
        effects.Add(newEffect);
        newEffect.OnAdd(this);

        OnEffectAdded?.Invoke(newEffect);
    }

    public T GetEffect<T>() where T : BuffDebuffEffect
    {
        foreach (BuffDebuffEffect effect in effects)
        {
            if (effect is T target) return target;
        }

        return null;
    }

    public void AddEffectNextTurn(BuffDebuffEffect effect)
    {
        reservedEffects.Add(effect);
    }

    public void OnBeforeClash()
    {
        foreach (BuffDebuffEffect effect in effects.ToArray())
        {
            effect.OnBeforeClash(this);
            OnEffectChanged?.Invoke(effect);
        }

        RemoveExpiredEffects();
    }

    public void OnBeforeAttack()
    {
        foreach (BuffDebuffEffect effect in effects.ToArray())
        {
            effect.OnBeforeAttack(this);
            OnEffectChanged?.Invoke(effect);
        }

        RemoveExpiredEffects();
    }

    public void OnAfterClash()
    {
        foreach (BuffDebuffEffect effect in effects.ToArray())
        {
            effect.OnAfterClash(this);
            OnEffectChanged?.Invoke(effect);
        }

        RemoveExpiredEffects();
    }

    public void OnAfterAttack()
    {
        foreach (BuffDebuffEffect effect in effects.ToArray())
        {
            effect.OnAfterAttack(this);
            OnEffectChanged?.Invoke(effect);
        }

        RemoveExpiredEffects();
    }

    public void OnStartTurn()
    {
        foreach (var effect in reservedEffects)
        {
            AddEffect(effect);
            OnEffectChanged?.Invoke(effect);
        }

        reservedEffects.Clear();

        foreach (BuffDebuffEffect effect in effects.ToArray())
        {
            effect.OnStartTurn(this);
            OnEffectChanged?.Invoke(effect);
        }

        RemoveExpiredEffects();
    }

    public void OnEndTurn()
    {
        foreach (BuffDebuffEffect effect in effects.ToArray())
        {
            effect.OnEndTurn(this);
            OnEffectChanged?.Invoke(effect);
        }

        RemoveExpiredEffects();
    }


    public void TakeFixedDamage(int damage)
    {
        if (characterStatus != null)
        {
            characterStatus.HpValue -= damage;
            return;
        }

        if (bossPart != null)
        {
            bossPart.TakeDamage(damage);
            return;
        }

        if (enemyStatus != null)
        {
            enemyStatus.HpValue -= damage;
            return;
        }
    }

    private void RemoveExpiredEffects()
    {
        for (int i = effects.Count - 1; i >= 0; i--)
        {
            BuffDebuffEffect effect = effects[i];

            if (!effect.IsExpired()) continue;

            effects.RemoveAt(i);

            OnEffectRemoved?.Invoke(effect);
        }
    }

    public void ClearAllEffects()
    {
        foreach (BuffDebuffEffect effect in effects.ToArray())
        {
            OnEffectRemoved?.Invoke(effect);
        }

        effects.Clear();
        reservedEffects.Clear();
    }

    private void Awake()
    {
        characterStatus = GetComponent<CharacterStatus>();
        enemyStatus = GetComponent<EnemyStatus>();
        bossPart = GetComponent<BossPart>();
    }

}