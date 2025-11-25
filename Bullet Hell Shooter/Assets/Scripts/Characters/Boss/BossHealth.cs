using UnityEngine;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    [Header("Configuración Boss")]
    public int maxHealth = 250;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;
    
    [Header("Audio")]
    public AudioClip damageSound; 
    private AudioSource audioSource;

    void Awake()
    {
        // Obtenemos el componente para poder cambiarle el color
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

    void Die()
    {
        GetComponent<BossWeapon>().enabled = false;
        
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