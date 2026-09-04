using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemysManager : MonoBehaviour
{
    [Header("적 스포너")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private List<GameObject> enemyList = new List<GameObject>();

    public List<GameObject> GetEnemyList { get { return enemyList; } }

    public void SetAllEnemysSpeeds()
    {
        foreach(var enemy in enemyList) 
        {
            enemy.GetComponent<EnemyStatus>().SetEnemyRandomSpeed();
        }
    }

    public void AddEnemyList(GameObject enemy)
    {
        enemyList.Add(enemy);
    }

    public void RemoveEnemyList(GameObject enemy)
    {
        enemyList.Remove(enemy);
    }

    public void SetAllEnemyTartget()
    {
        SetAllEnemysSpeeds();

        foreach (var enemy in enemyList)
        {
            EnemySlot[] slots = enemy.GetComponentsInChildren<EnemySlot>(true);

            foreach (EnemySlot slot in slots)
            {
                if (slot == null) continue;

                slot.RequestSetEnemySlot();
            }
        }
    }
    public bool IsAllEnemyDead()
    {
        return enemyList.Count == 0;
    }
}
