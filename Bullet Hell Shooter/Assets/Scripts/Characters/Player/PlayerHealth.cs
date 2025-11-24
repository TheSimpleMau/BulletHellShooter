using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Invulnerabilidad")]
    public float iframeDuration = 2f; // Tiempo de inmunidad
    public float blinkInterval = 0.1f; // Velocidad del parpadeo
    private bool isInvulnerable = false;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D myCollider; // Para desactivar colisiones si quieres, o solo lógica

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        myCollider = GetComponent<BoxCollider2D>();
        currentHealth = maxHealth;
        if (UIManager.Instance != null) 
            UIManager.Instance.UpdatePlayerHealth(currentHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;

        currentHealth -= damage;
        Debug.Log($"Vida actual: {currentHealth}"); // Debug 1

        // --- BLOQUE DE DIAGNÓSTICO ---
        if (UIManager.Instance != null)
        {
            Debug.Log("✅ Encontré al UIManager. Llamando a actualizar vida...");
            UIManager.Instance.UpdatePlayerHealth(currentHealth);
        }
        else
        {
            // SI SALE ESTE MENSAJE ROJO, EL PROBLEMA ES QUE EL MANAGER NO EXISTE
            Debug.LogError("❌ ERROR CRÍTICO: UIManager.Instance es NULL. El script no encuentra el Manager en la escena.");
        }
        // -----------------------------

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvulnerabilityRoutine());
        }
    }

    void Die()
    {
        Debug.Log("¡GAME OVER!");
        // Aquí podrías llamar a StageManager para pausar el juego o mostrar pantalla de derrota
        Destroy(gameObject); 
    }

    // Corrutina para el efecto de parpadeo
    IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;
        
        float timer = 0;
        while (timer < iframeDuration)
        {
            // PROTECCIÓN: Solo intentamos cambiar color si el spriteRenderer existe
            if (spriteRenderer != null) 
            {
                Color c = spriteRenderer.color;
                c.a = (c.a == 1f) ? 0.2f : 1f; 
                spriteRenderer.color = c;
            }

            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        // Restauramos al final con protección
        if (spriteRenderer != null)
        {
            Color finalColor = spriteRenderer.color;
            finalColor.a = 1f;
            spriteRenderer.color = finalColor;
        }
        
        isInvulnerable = false;
    }
}