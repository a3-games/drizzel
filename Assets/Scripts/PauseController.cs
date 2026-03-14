using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance { get; private set; }

    [SerializeField]
    private float menuFadeTime = 0.15f;

    [HideInInspector]
    public bool isPaused = false;

    [HideInInspector]
    public bool sceneIsTransitioning = false;

    private Canvas pauseMenu;
    private bool menuIsFading = false;

    public void RegisterPauseMenu(Canvas canvas)
    {
        pauseMenu = canvas;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (
            Keyboard.current != null
            && Keyboard.current.escapeKey.wasPressedThisFrame
            && !menuIsFading
            && !sceneIsTransitioning
        )
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(
        UnityEngine.SceneManagement.Scene scene,
        UnityEngine.SceneManagement.LoadSceneMode mode
    )
    {
        sceneIsTransitioning = false;
        pauseMenu = null;
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        isPaused = true;

        StartCoroutine(FadeInCanvas(menuFadeTime));

        Time.timeScale = 0f;
        AudioManager.Instance.PauseAll();
    }

    public void ResumeGame()
    {
        isPaused = false;

        Time.timeScale = 1f;
        AudioManager.Instance.ResumeAll();

        StartCoroutine(FadeOutCanvas(menuFadeTime));
    }

    private IEnumerator FadeInCanvas(float duration)
    {
        if (pauseMenu == null)
            yield break;

        CanvasGroup canvasGroup = pauseMenu.GetComponent<CanvasGroup>();
        if (!canvasGroup)
            yield break;

        pauseMenu.gameObject.SetActive(true);

        float t = 0f;

        while (t < 1f)
        {
            menuIsFading = true;
            t += Time.unscaledDeltaTime / duration;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        menuIsFading = false;
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutCanvas(float duration)
    {
        if (pauseMenu == null)
            yield break;

        CanvasGroup canvasGroup = pauseMenu.GetComponent<CanvasGroup>();
        if (!canvasGroup)
            yield break;

        float t = 0f;

        while (t < 1f)
        {
            menuIsFading = true;
            t += Time.unscaledDeltaTime / duration;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        menuIsFading = false;
        canvasGroup.alpha = 0f;
        pauseMenu.gameObject.SetActive(false);
    }
}
