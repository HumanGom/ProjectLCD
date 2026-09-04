using UnityEngine;

[System.Serializable]
public class VulnerableEffect : BuffDebuffEffect
{
    private int stack = 0;
    private bool isExpired = false;

    public override string effectCodeName => "Vulnerable";
    public override string effectNickName => "취약";
    public override int effectPower => -1;
    public override int effectCount => stack;

    public int GetvulnerableStack { get { return stack;} }

    public void SetVulnerableStack(int value)
    {
        stack = Mathf.Clamp(value, 0, 10);
    }

    public override void Merge(BuffDebuffEffect other)
    {
        VulnerableEffect otherEffect = other as VulnerableEffect;

        if (otherEffect != null)
        {
            stack = Mathf.Clamp(stack + otherEffect.stack, 0, 10);
        }
        Debug.Log($"취약 스택{stack}");
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