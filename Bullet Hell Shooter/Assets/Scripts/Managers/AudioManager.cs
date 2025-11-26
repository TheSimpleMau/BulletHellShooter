using UnityEngine;

/// <summary>
/// Sistema Singleton para reproducir música y efectos de sonido.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Fuentes de Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Reproduce un efecto de sonido una sola vez.
    /// </summary>
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    /// <summary>
    /// Cambia la música de fondo si es diferente a la actual.
    /// </summary>
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return; // Si ya suena esa canción, no hacer nada

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }
    
    /// <summary>
    ///  Función para detener música
    /// </summary>
    public void StopMusic()
    {
        musicSource.Stop();
    }
}