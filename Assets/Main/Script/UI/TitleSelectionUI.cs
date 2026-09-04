using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSelectionUI : MonoBehaviour
{
    [Header("NewGame버튼을 누른후 가는 씬 이름")]
    [SerializeField] private string newGameScene = "TestRougeLike";
    [Header("Continue버튼을 누른후 가는 씬 이름(미정)")]
    [SerializeField] private string continueScene = "";

    public void OnNewGame()
    {
        SceneManager.LoadScene(newGameScene);
    }

    public void OnContinue()
    {
        SceneManager.LoadScene(continueScene);
    }

    public void OnExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void Start()
    {
        //gameObject.SetActive(false);
    }
}
