using UnityEngine;
using System.Collections;

/// <summary>
/// Controla vida, sonidos y feedback visual del boss.
/// </summary>
public class BossHealth : MonoBehaviour
{
    [Header("Configuración Boss")]
    public int maxHealth = 500;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    
    [Header("Audio")]
    public AudioClip damageSound; 
    public AudioClip deathSound;
    private AudioSource audioSource;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        if (UIManager.Instance != null) 
            UIManager.Instance.UpdateBossHealth(currentHealth, maxHealth);
    }

    /// <summary>
    /// Aplica daño, reproduce sonido, actualiza UI y comprueba muerte.
    /// </summary>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (damageSound != null && audioSource != null) audioSource.PlayOneShot(damageSound);
        
        // Opcional: Feedback visual simple (Color rojo momentáneo)
        StartCoroutine(FlashRed()); 
        
        if (UIManager.Instance != null) UIManager.Instance.UpdateBossHealth(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Acciones al morir: desactivar armas, reproducir SFX y notificar victoria.
    /// </summary>
    void Die()
    {
        GetComponent<BossWeapon>().enabled = false;
        
        if (deathSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(deathSound);
        }
        
        if (StageManager.Instance != null) StageManager.Instance.PlayVictoryMusic();

        Destroy(gameObject, 0.5f);
    }


    IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }
}