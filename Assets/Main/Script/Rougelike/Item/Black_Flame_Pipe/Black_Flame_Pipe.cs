using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/흑염 파이프")]
public class Black_Flame_Pipe : ItemObjectOS
{
    private int increaseBasicPowerBonus = 1;
    private int vulnerableStack = 1;

    public override void OnBeforeTurnStart()
    {
        CharactersManager charactersManager = FindFirstObjectByType<CharactersManager>();

        if (charactersManager == null) return;

        if (charactersManager.GetCharacterList.Count == 0) return;
            
        GameObject firstCharacter = charactersManager.GetCharacterList[0];

        if (firstCharacter == null) return;

        BuffDebuffManager buffManager = firstCharacter.GetComponent<BuffDebuffManager>();

        if (buffManager == null) return;

        IncreaseBasicPower increaseBasicPowerEffect = new IncreaseBasicPower();
        VulnerableEffect vulnerableEffect = new VulnerableEffect();
        increaseBasicPowerEffect.SetIncreaseBasicPowerStack(increaseBasicPowerBonus);
        vulnerableEffect.SetVulnerableStack(vulnerableStack);

        buffManager.AddEffect(increaseBasicPowerEffect);
        buffManager.AddEffect(vulnerableEffect);

        Debug.Log($"아이템 효과 발동: 1번 아군 기본위력{increaseBasicPowerBonus} 증가 및 받는피해증가{vulnerableStack}");
        increaseBasicPowerBonus = math.clamp(increaseBasicPowerBonus + 1, 0, 10);
        vulnerableStack = math.clamp(vulnerableStack + 1, 0, 10);
    }

    public override void OnBattleStart()
    {
        increaseBasicPowerBonus = 1;
        vulnerableStack = 1;
    }

    public override void OnBattleEnd()
    {
        increaseBasicPowerBonus = 1;
        vulnerableStack = 1;
    }
    private void Awake()
    {
        SetItemInfo($"매턴 시작 전에 1번 편성 아군에게 {increaseBasicPowerBonus}기본 위력 증가와 {vulnerableStack}취약을 부여함");
    }
}