using UnityEngine;
using System.Collections;

/// <summary>
/// Salud del jugador: manejo de invulnerabilidad temporal (i-frames) y muerte.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Audio")]
    public AudioClip damageSound;
    private AudioSource audioSource;

    [Header("Invulnerabilidad")]
    public float iframeDuration = 1f;
    public float blinkInterval = 0.1f;
    private bool isInvulnerable = false;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D myCollider;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        myCollider = GetComponent<BoxCollider2D>();
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        if (UIManager.Instance != null) 
            UIManager.Instance.UpdatePlayerHealth(currentHealth, maxHealth);
    }

    /// <summary>
    /// Resta vida, reproduce sonido y activa i-frames si no muere.
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;
        currentHealth -= damage;

        if (damageSound != null && audioSource != null)
            audioSource.PlayOneShot(damageSound);

        if (UIManager.Instance != null) UIManager.Instance.UpdatePlayerHealth(currentHealth, maxHealth);

        if (currentHealth <= 0) Die();
        else StartCoroutine(InvulnerabilityRoutine());
    }

    void Die()
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.TriggerDefeat();
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// Corrutina que hace parpadeo mientras dura la invulnerabilidad.
    /// </summary>
    IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        
        float timer = 0;
        while (timer < iframeDuration)
        {
            if (spriteRenderer != null) 
            {
                Color c = spriteRenderer.color;
                c.a = (c.a == 1f) ? 0.2f : 1f; 
                spriteRenderer.color = c;
            }

            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        if (spriteRenderer != null)
        {
            Color finalColor = spriteRenderer.color;
            finalColor.a = 1f;
            spriteRenderer.color = finalColor;
        }
        
        isInvulnerable = false;
    }
}