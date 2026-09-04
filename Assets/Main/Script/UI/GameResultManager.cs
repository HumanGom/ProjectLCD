using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResultManager : MonoBehaviour
{
    [Header("모든 canvasGroup")]
    [SerializeField] private CanvasGroup[] canvasGroups;
    [Header("승리 UI")]
    [SerializeField] private GameObject winUIOBJ;
    [Header("패배 UI")]
    [SerializeField] private GameObject loseUIOBJ;

    public void OnGameResult(float seconds, bool isWin, string secneName)
    {
        StartCoroutine(GameResultUIRoutine(seconds, isWin, secneName));
    }

    private IEnumerator GameResultUIRoutine(float seconds, bool isWin, string secneName)
    {
        SetInteractable(false);
        GameResultUISetter(isWin, false);

        yield return new WaitForSeconds(seconds);

        SetInteractable(true);
        GameResultUISetter(isWin, true);
        if (GoodsManager.Instance == null)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
        SceneManager.LoadScene("TestRougelike");
    }

    private void SetInteractable(bool value)
    {
        foreach (CanvasGroup group in canvasGroups)
        {
            if (group == null) continue;
            group.blocksRaycasts = value;
            group.interactable = value;
        }
    }

    private void GameResultUISetter(bool isWin, bool value)
    {
        if(isWin)
        {
            winUIOBJ.SetActive(!value);
            return;
        }
        else
        {
            loseUIOBJ.SetActive(!value);
            return;
        }
    }

    private void Start()
    {
        SetInteractable(true );
        if (winUIOBJ != null) winUIOBJ.SetActive(false);
        if (loseUIOBJ != null) loseUIOBJ.SetActive(false);
    }
}

