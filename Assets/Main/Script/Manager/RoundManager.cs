using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundManager : MonoBehaviour
{
    [Header("캐릭터매니저")]
    [SerializeField] private CharactersManager charactersManager;
    [Header("캐릭터스포너")]
    [SerializeField] private CharacterSpawner characterSpawner;
    [Header("적매니저")]
    [SerializeField] private EnemysManager enemysManager;
    [Header("적스포너")]
    [SerializeField] private EnemySpawner enemySpawner;

    private IEnumerator Start()
    {
        if (BGMManager.Instance != null) BGMManager.Instance.RequestChangeBGM(true);
        characterSpawner.CharacterSpawn();
        charactersManager.RequestSetSpeedAndSlot();

        enemySpawner.EnemySpawn();

        yield return null;

        BossController[] bosses = FindObjectsByType<BossController>(FindObjectsSortMode.None);
        if(bosses.Length > 0)
        {
            //Debug.Log("보스감지");
            foreach (BossController boss in bosses)
            {
                boss.SetNextPattern();
            }
        }


        enemysManager.SetAllEnemyTartget();

        if (GoodsManager.Instance != null)
        {
            GoodsManager.Instance.OnBattleStart();
            GoodsManager.Instance.OnBeforeTurnStart();
        }

        yield return null;

        TargetConflictManager.Instance.ResetPlayerSelectionAndRestoreEnemyTargets();
    }

    public void ReturnToMap(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
