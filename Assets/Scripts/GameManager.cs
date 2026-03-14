using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private string sceneName = "NextScene";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SwitchScene()
    {
        Time.timeScale = 1;

        ScreenFader screenFader = Object.FindFirstObjectByType<ScreenFader>();

        if (screenFader != null)
            screenFader.FadeToScene(sceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void SwitchSceneManual(string scene)
    {
        Time.timeScale = 0;

        ScreenFader screenFader = Object.FindFirstObjectByType<ScreenFader>();

        if (screenFader != null)
            screenFader.FadeToScene(scene);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }

    public void QuitGame()
    {
        Application.Quit();

        /* Most testing is done in the Unity editor, so we should probably keep this statement.
         * This block simply adds support for quitting the game if it was started in the Unity editor.
         */
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
