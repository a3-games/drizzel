using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField]
    private BackgroundTrack[] backgroundLoops;

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
        foreach (BackgroundTrack loop in backgroundLoops)
            PlayAudio(loop.audioClip, loop.volume, true);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void PlayAudio(
        AudioClip audioClip,
        float volume = 1f,
        bool loop = false,
        bool fadeIn = false,
        bool fadeOut = false,
        float fadeTime = 0f
    )
    {
        StartCoroutine(PlayCoroutine(audioClip, volume, loop, fadeIn, fadeOut, fadeTime));
    }

    private IEnumerator PlayCoroutine(
        AudioClip audioClip,
        float volume = 1f,
        bool loop = false,
        bool fadeIn = false,
        bool fadeOut = false,
        float fadeTime = 0f
    )
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = loop;

        if (fadeIn)
            StartCoroutine(FadeInAudio(audioSource, fadeTime, volume));
        else
            audioSource.Play();

        if (!loop)
        {
            if (fadeOut && fadeTime > 0f)
            {
                float waitTime = Mathf.Max(0, audioSource.clip.length - fadeTime);
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(FadeOutAudio(audioSource, fadeTime));
            }
            else
                yield return new WaitForSeconds(audioSource.clip.length);

            Destroy(audioSource);
        }
    }

    private IEnumerator FadeInAudio(AudioSource audioSource, float fadeTime, float targetVolume)
    {
        audioSource.Play();
        float timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, timer / fadeTime);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    private IEnumerator FadeOutAudio(AudioSource audioSource, float fadeTime)
    {
        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeTime);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
    }

    public void FadeOutAll(float fadeTime)
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(FadeOutAllAndDestroy(fadeTime));
    }

    private IEnumerator FadeOutAllAndDestroy(float fadeTime)
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        float[] startVolumes = System.Array.ConvertAll(sources, s => s.volume);
        float timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / fadeTime);
            for (int i = 0; i < sources.Length; i++)
            {
                if (sources[i] != null)
                    sources[i].volume = Mathf.Lerp(startVolumes[i], 0f, t);
            }
            yield return null;
        }

        Instance = null;
        Destroy(gameObject);
    }

    public void PauseAll()
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        foreach (AudioSource source in sources)
            source.Pause();
    }

    public void ResumeAll()
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        foreach (AudioSource source in sources)
            source.UnPause();
    }
}
