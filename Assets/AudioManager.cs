using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("----------- Audio Sources ----------- ")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("----------- Audio Clip ----------- ")]
    public AudioClip backgroundMusic;
    public AudioClip gameover; 

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true; 
            musicSource.Play();
        }
    }

    public void PlayGameoverSound()
    {
        // Stop the background music
        musicSource.Stop();

        // Play the gameover sound
        if (gameover != null)
        {
            sfxSource.clip = gameover;
            sfxSource.Play();
        }
    }

    public void StopGameoverSound()
    {
        sfxSource.Stop();
    }
}
