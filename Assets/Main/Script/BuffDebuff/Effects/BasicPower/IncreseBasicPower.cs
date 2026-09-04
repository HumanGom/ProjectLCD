using UnityEngine;

[System.Serializable]
public class IncreaseBasicPower : BuffDebuffEffect
{
    private int stack = 0;
    private bool isExpired = false;

    public override string effectCodeName => "IncreaseBasicPower";
    public override string effectNickName => "기본위력증가";
    public override int effectPower => -1;
    public override int effectCount => stack;

    public int GetIncreaseBasicPowerStack {  get { return stack; } }

    public void SetIncreaseBasicPowerStack(int value)
    {
        stack = Mathf.Clamp(value, 0, 99);
    }

    public override void Merge(BuffDebuffEffect other)
    {
        IncreaseBasicPower otherEffect = other as IncreaseBasicPower;

        if (otherEffect != null)
        {
            stack = Mathf.Clamp(stack + otherEffect.stack, 0, 99);
        }
        Debug.Log($"공격 위력 증가량{stack}");
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