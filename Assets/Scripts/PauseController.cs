using UnityEngine;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance { get; private set; }

    [HideInInspector]
    public bool isPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;

        Time.timeScale = 0;
        AudioManager.Instance.PauseAll();
    }

    private void ResumeGame()
    {
        isPaused = false;

        Time.timeScale = 1;
        AudioManager.Instance.ResumeAll();
    }
}
