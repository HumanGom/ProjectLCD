using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestKiller : MonoBehaviour
{
    [Header("아군 전멸 버튼")]
    public InputAction playerKillKeyBind;
    [Header("적 전멸 버튼")]
    public InputAction enemyKillKeyBind;
    [Header("적 전멸 및 아군 한명 죽이는 버튼")]
    public InputAction enemyKillKeyBind2;

    public TurnManager TurnManager;


    private void OnEnable()
    {
        playerKillKeyBind.performed += KillPlayer;
        playerKillKeyBind.Enable();

        enemyKillKeyBind.performed += KillEnemy;
        enemyKillKeyBind.Enable();

        enemyKillKeyBind2.performed += KillOnlyOneCharacter;
        enemyKillKeyBind2.Enable();
    }

    private void OnDisable()
    {
        playerKillKeyBind.performed -= KillPlayer;
        playerKillKeyBind.Disable();

        enemyKillKeyBind.performed -= KillEnemy;
        enemyKillKeyBind.Disable();

        enemyKillKeyBind2.performed -= KillOnlyOneCharacter;
        enemyKillKeyBind2.Disable();
    }

    private void KillPlayer(InputAction.CallbackContext context)
    {

        CharacterStatus[] characterStatuses = FindObjectsByType<CharacterStatus>(FindObjectsSortMode.None);

        foreach (CharacterStatus status in characterStatuses)
        {
            if (status != null)
            {
                status.OnDeath();
            }
        }

        TurnManager.OnBattleLose();
    }

    private void KillEnemy(InputAction.CallbackContext context)
    {

        EnemyStatus[] enemyStatuses = FindObjectsByType<EnemyStatus>(FindObjectsSortMode.None);

        foreach (EnemyStatus status in enemyStatuses)
        {
            if(status != null)
            {
                status.OnDeath();
            }
        }

        TurnManager.OnBattleWin();
    }

    private void KillOnlyOneCharacter(InputAction.CallbackContext context)
    {
        CharacterStatus[] characterStatuses = FindObjectsByType<CharacterStatus>(FindObjectsSortMode.None);
        if (characterStatuses[0] != null)
        {
            characterStatuses[0].OnDeath();
            KillEnemy(context);
        }
    }
}
