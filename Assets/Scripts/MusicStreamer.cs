using System.Collections;
using UnityEngine;

public class MusicStreamer : MonoBehaviour
{
    public static MusicStreamer Instance { get; private set; }

    [SerializeField]
    private BackgroundTrack[] backgroundTracks;

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
        StartCoroutine(RandomMusicLoop());
    }

    private IEnumerator RandomMusicLoop()
    {
        while (true)
        {
            int idx = Random.Range(0, backgroundTracks.Length);
            BackgroundTrack track = backgroundTracks[idx];

            AudioManager.Instance.PlayAudio(track.audioClip, track.volume, false, true, true, 15f);

            yield return new WaitForSeconds(track.audioClip.length);
        }
    }
}
