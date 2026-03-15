using UnityEngine;

public class GameControls : MonoBehaviour
{
    public void UPauseGame()
    {
        PauseController.Instance.PauseGame();
    }
}
