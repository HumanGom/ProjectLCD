using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/재에서 재로")]
public class Ashes_To_Ashes : ItemObjectOS
{   
    private int increaseBurnPowerValue = 3;

    public override void OnBeforeTurnStart()
    {
        EnemysManager enemysManager = FindFirstObjectByType<EnemysManager>();

        if (enemysManager == null) return;

        if (enemysManager.GetEnemyList.Count == 0) return;

        foreach (GameObject enemyObj in enemysManager.GetEnemyList)
        {
            if (enemyObj == null) continue;

            BuffDebuffManager[] buffManagers = enemyObj.GetComponentsInChildren<BuffDebuffManager>(true);

            foreach (BuffDebuffManager buffManager in buffManagers)
            {
                BurnEffect burnEffect = new BurnEffect();
                burnEffect.SetBurnEffect(increaseBurnPowerValue, 1);

                buffManager.AddEffect(burnEffect);
            }
        }
    }
}