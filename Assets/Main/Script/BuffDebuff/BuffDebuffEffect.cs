using UnityEngine;

public abstract class BuffDebuffEffect
{
    public abstract string effectCodeName { get; }
    public abstract string effectNickName { get; }
    public virtual Sprite effectIcon => null;
    public virtual int effectPower => 0;
    public virtual int effectCount => 0;


    public virtual bool CanMerge(BuffDebuffEffect other) { return GetType() == other.GetType(); }
    public virtual void Merge(BuffDebuffEffect other) { }
    public virtual void OnAdd(BuffDebuffManager owner) { }
    public virtual void OnBeforeClash(BuffDebuffManager owner) { }
    public virtual void OnBeforeAttack(BuffDebuffManager owner) { }
    public virtual void OnAfterClash(BuffDebuffManager owner) { }
    public virtual void OnAfterAttack(BuffDebuffManager owner) { }
    public virtual void OnStartTurn(BuffDebuffManager owner) { }
    public virtual void OnEndTurn(BuffDebuffManager owner) { }
    public virtual bool IsExpired() { return false; }
}