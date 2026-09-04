using UnityEngine;

[System.Serializable]
public class IncreaseDamageEffect : BuffDebuffEffect
{
    private int stack = 0;
    private bool isExpired = false;

    public override string effectCodeName => "IncreaseDamage";
    public override string effectNickName => "피해량증가";
    public override int effectPower => -1;
    public override int effectCount => stack;

    public int GetIncreaseDamageStack { get { return stack;} }

    public void SetIncreseDamageStack(int value)
    {
        stack = Mathf.Clamp(value, 0, 10);
    }

    public override void Merge(BuffDebuffEffect other)
    {
        IncreaseDamageEffect otherEffect = other as IncreaseDamageEffect;

        if (otherEffect != null)
        {
            stack = Mathf.Clamp(stack + otherEffect.stack, 0, 10);
        }
        Debug.Log($"피해량증가 스택{stack}");
    }


    public override void OnEndTurn(BuffDebuffManager owner)
    {
        isExpired = true;
    }
        
    public override bool IsExpired()
    {
        return isExpired;
    }
}