using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AudioSource musicSource;

    [Header("Universal Tracks")]
    public AudioClip defaultMusic;
    public AudioClip bossMusic;
    public AudioClip specialMusic;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayDefault()
    {
        musicSource.clip = defaultMusic;
        musicSource.Play();
    }

    public void PlayBoss()
    {
        musicSource.clip = bossMusic;
        musicSource.Play();
    }

    public void PlaySpecial()
    {
        musicSource.clip = specialMusic;
        musicSource.Play();
    }
}
