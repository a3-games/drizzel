using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private void Start()
    {
        PauseController.Instance.RegisterPauseMenu(GetComponent<Canvas>());
    }

    public void UResumeGame()
    {
        PauseController.Instance.ResumeGame();
    }

    public void UQuitToMenu()
    {
        GameManager.Instance.SwitchSceneManual("MainMenu");
    }
}
