using System.Collections;
using UnityEngine;

[System.Serializable]
public class BackgroundLoop
{
    public AudioClip audioClip;
    public float volume;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField]
    private BackgroundLoop[] backgroundLoops;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (BackgroundLoop loop in backgroundLoops)
            PlayAudio(loop.audioClip, loop.volume, true);
    }

    public void PlayAudio(AudioClip audioClip, float volume = 1f, bool loop = false)
    {
        StartCoroutine(PlayCoroutine(audioClip, volume, loop));
    }

    private IEnumerator PlayCoroutine(AudioClip audioClip, float volume = 1f, bool loop = false)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.Play();

        if (!loop)
        {
            yield return new WaitForSeconds(audioSource.clip.length);
            Destroy(audioSource);
        }
        else
            yield break;
    }
}
