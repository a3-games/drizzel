using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [SerializeField]
    private Image fadeImage;

    [SerializeField]
    private float fadeDuration = 1f;

    [SerializeField]
    private bool fadeIn = true;

    [SerializeField]
    private bool fadeOut = true;

    private void Awake()
    {
        fadeImage.raycastTarget = false;
    }

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.FadeOutAll(fadeDuration);

        yield return StartCoroutine(FadeOut());

        if (!fadeOut)
            yield return new WaitForSeconds(fadeDuration);

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeOut()
    {
        if (fadeOut)
        {
            SetAlpha(0f);
            fadeImage.raycastTarget = true; // block input during fade

            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                SetAlpha(Mathf.Clamp01(t / fadeDuration));
                yield return null;
            }

            SetAlpha(1f);
        }
    }

    private IEnumerator FadeIn()
    {
        if (fadeIn)
        {
            SetAlpha(1f);
            fadeImage.raycastTarget = true;

            for (int i = 0; i < 5; i++)
                yield return null;

            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                SetAlpha(1f - Mathf.Clamp01(t / fadeDuration));
                yield return null;
            }

            SetAlpha(0f);
            fadeImage.raycastTarget = false; // re-enable input
        }
    }

    private void SetAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
        }
    }
}
