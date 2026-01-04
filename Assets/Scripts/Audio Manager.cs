using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [SerializeField] private AudioSource audioPrefab;

    [Header("BGM Settings")]
    public AudioSource bgmSource;
    public AudioClip defaultBGM;
    public AudioClip menuBGM;
    [SerializeField][Range(0, 1)] private float bgmVolume = 1f;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        defaultBGM.LoadAudioData();
        menuBGM.LoadAudioData();

        bgmSource.volume = bgmVolume;
    }
    public void PlayBGM(AudioClip bgm)
    {
        if (bgmSource.isPlaying) bgmSource.Stop();

        bgmSource.clip = bgm;
        bgmSource.Play();
        bgmSource.loop = true;
    }
    public void PlaySFX(AudioClip clip, Transform transform, float volume)
    {
        AudioSource audioSource = Instantiate(audioPrefab, transform.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audioSource.gameObject, audioSource.clip.length);
    }
}
