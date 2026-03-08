using System.Collections;
using TMPro;
using UnityEngine;

public class StoryWriter : MonoBehaviour
{
    public static StoryWriter Instance { get; private set; }

    [SerializeField]
    private TMP_Text storyboardText;

    [SerializeField]
    private StoryLine[] storyLines;

    private StoryLine currentLine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(BeginStory());
    }

    public IEnumerator BeginStory()
    {
        // Fade in and fade out each line
        foreach (StoryLine line in storyLines)
        {
            currentLine = line;

            line.onLineStart.Invoke();
            yield return StartCoroutine(FadeInLine());
            yield return new WaitForSeconds(line.advanceDelay);

            yield return StartCoroutine(FadeOutLine());
            yield return new WaitForSeconds(line.intermissionDuration);
            line.onLineEnd.Invoke();
        }
    }

    private IEnumerator FadeInLine()
    {
        SetAlpha(0);
        storyboardText.text = currentLine.line;

        float t = 0f;
        while (t < currentLine.fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            storyboardText.alpha = Mathf.Clamp01(t / currentLine.fadeDuration);
            yield return null;
        }

        SetAlpha(1);
    }

    private IEnumerator FadeOutLine()
    {
        SetAlpha(1);

        float t = 1f;
        while (t < currentLine.fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            storyboardText.alpha = Mathf.Clamp01(1 - t / currentLine.fadeDuration);
            yield return null;
        }

        SetAlpha(0);
        storyboardText.text = "";
    }

    private void SetAlpha(float alpha)
    {
        if (storyboardText != null)
        {
            Color c = storyboardText.color;
            c.a = alpha;
            storyboardText.color = c;
        }
    }
}
