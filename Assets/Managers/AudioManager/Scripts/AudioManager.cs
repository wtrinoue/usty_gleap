using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource seSource;
    [SerializeField] private AudioSource bgmSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySE(SoundData sound)
    {
        if (sound == null || sound.clip == null) return;

        float pitch = sound.pitch;

        if (sound.randomPitch)
        {
            pitch = Random.Range(sound.pitchMin, sound.pitchMax);
        }

        seSource.pitch = pitch;
        seSource.PlayOneShot(sound.clip, sound.volume);
    }

    public void PlayBGM(SoundData sound)
    {
        if (sound == null || sound.clip == null) return;

        bgmSource.clip = sound.clip;
        bgmSource.volume = sound.volume;
        bgmSource.pitch = sound.pitch;
        bgmSource.loop = true;
        bgmSource.Play();
    }
}