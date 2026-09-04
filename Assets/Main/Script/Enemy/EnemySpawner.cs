using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class EnemySpawner : MonoBehaviour
{
    [Header("선택된 적")]
    [SerializeField] private List<GameObject> enemys = new List<GameObject>();
    [Header("기본스폰위치")]
    [SerializeField] private Vector3 defaultPos = new Vector3(1.7f, 2.5f, 0f);
    [Header("적 간격")]
    [SerializeField] private Vector3 offset = new Vector3(-0.5f, 0f, 0f);
    [Header("적 크기 배율")]
    [SerializeField] private float enemySize = 1f;
    [Header("적 리스트")]
    [SerializeField] private Transform enemyListRoot;

    [Header("라운드 매니저")]
    [SerializeField] private TurnManager roundManager;
    [Header("적 매니저")]
    [SerializeField] private EnemysManager enemysManager;

    public void EnemySpawn()
    {
        Vector3 pos = defaultPos;

        List<GameObject> spawnList = BattleSceneData.EnemyPrefabs.Count > 0 ? BattleSceneData.EnemyPrefabs : enemys;

        foreach (var enemy in spawnList)
        {
            if(enemy == null)
            {
                pos += offset;
                continue;
            }
                
            enemy.name = enemy.GetComponent<EnemyStatus>().GetName;
            GameObject spawnedEnemy = Instantiate(enemy, pos, Quaternion.identity, enemyListRoot);
            spawnedEnemy.transform.localScale = Vector3.one * enemySize;
            enemysManager.AddEnemyList(spawnedEnemy);

            pos += offset;
        }

        BattleSceneData.EnemysClear();
    }
}
