using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private AudioSource effectAudioSource;

    [SerializeField] private AudioClip backgroundClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip coinClip;

    private static AudioManager instance;

    private void Awake()
    {
        // Đảm bảo chỉ có một instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Hủy bản sao thừa
        }
    }

    void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundAudioSource != null && backgroundClip != null)
        {
            backgroundAudioSource.clip = backgroundClip;
            backgroundAudioSource.Play();
        }
    }

    public void PlayCoinSound()
    {
        if (effectAudioSource != null && coinClip != null)
        {
            effectAudioSource.PlayOneShot(coinClip);
        }
    }

    public void PlayJumpSound()
    {
        if (effectAudioSource != null && jumpClip != null)
        {
            effectAudioSource.PlayOneShot(jumpClip);
        }
    }
}