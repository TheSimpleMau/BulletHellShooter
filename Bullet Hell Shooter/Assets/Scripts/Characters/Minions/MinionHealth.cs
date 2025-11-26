using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Salud de los minions con evento OnMinionDeath y feedback visual/audio.
/// </summary>
public class MinionHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 3;
    public bool destroyOnDeath = true;
    
    [Header("Audio VFX")]
    public AudioClip deathVFX;
    public AudioClip damageVFX;
    private AudioSource audioSource;

    public int currentHealth;

    // Propiedad pública (TryGetHealth busca "CurrentHealth" como property)
    public int CurrentHealth { get { return currentHealth; } }
    private SpriteRenderer spriteRenderer;

    public static event Action OnMinionDeath;

    void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.enabled = true;
    }

    /// <summary>
    /// Aplica daño y si llega a 0 invoca OnMinionDeath y destruye el objeto.
    /// </summary>
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (damageVFX != null && audioSource != null && audioSource.isActiveAndEnabled) audioSource.PlayOneShot(damageVFX);
        StartCoroutine(FlashRed()); 
        if (currentHealth <= 0)
            Die();
    }


    void Die()
    {
        OnMinionDeath?.Invoke();

        if (deathVFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(deathVFX);
        }
        if (destroyOnDeath)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
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
