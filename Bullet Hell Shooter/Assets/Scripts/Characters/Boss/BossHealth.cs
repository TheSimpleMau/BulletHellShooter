using UnityEngine;
using System.Collections;

public class BossHealth : MonoBehaviour
{
    [Header("Configuración Boss")]
    public int maxHealth = 250;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        // Obtenemos el componente para poder cambiarle el color
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        if (UIManager.Instance != null) 
            UIManager.Instance.UpdateBossHealth(currentHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        // Opcional: Feedback visual simple (Color rojo momentáneo)
        StartCoroutine(FlashRed()); 
        
        if (UIManager.Instance != null) 
            UIManager.Instance.UpdateBossHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("¡VICTORIA! El Boss ha sido derrotado.");
        // Detener el BossWeapon para que no siga disparando
        GetComponent<BossWeapon>().enabled = false;
        
        // Efectos de explosión aquí...
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