using UnityEngine;
using UnityEngine.InputSystem;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField]
    private float offsetMultiplier = 50f;

    [SerializeField]
    private float smoothTime = 0.2f;

    [SerializeField]
    private bool horizontalParallax = true;

    [SerializeField]
    private bool verticalParallax = true;

    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Vector2 velocity;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        float offsetX = -1 * (mousePos.x - Screen.width / 2f) / (Screen.width / 2f);
        float offsetY = -1 * (mousePos.y - Screen.height / 2f) / (Screen.height / 2f);

        Vector2 targetPosition =
            startPosition
            + new Vector2(
                horizontalParallax ? offsetX * offsetMultiplier : 0f,
                verticalParallax ? offsetY * offsetMultiplier : 0f
            );

        float halfWidth = rectTransform.rect.width * rectTransform.lossyScale.x / 2f;
        float halfHeight = rectTransform.rect.height * rectTransform.lossyScale.y / 2f;

        // Clamp to stop image from escaping view
        float clampX = horizontalParallax ? offsetMultiplier : 0f;
        float clampY = verticalParallax ? offsetMultiplier : 0f;

        targetPosition.x = Mathf.Clamp(
            targetPosition.x,
            startPosition.x - clampX,
            startPosition.x + clampX
        );
        targetPosition.y = Mathf.Clamp(
            targetPosition.y,
            startPosition.y - clampY,
            startPosition.y + clampY
        );

        rectTransform.anchoredPosition = Vector2.SmoothDamp(
            rectTransform.anchoredPosition,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}
