using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Aplica daño por contacto (trigger/collision), con cooldown por target, knockback y eventos.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ContactDamage : MonoBehaviour
{
    [Header("Daño")]
    public int damage = 1;
    public float damageCooldown = 0.6f;
    public bool damageOnStay = true;

    [Header("Identificación del objetivo")]
    public bool useTag = true;
    public string playerTag = "Player";
    public LayerMask targetLayerMask = ~0;

    [Header("Comportamiento")]
    public bool applyKnockback = false;
    public float knockbackStrength = 5f;
    public bool destroyTargetOnContact = false;
    public bool keepThisOnContact = true;

    [Header("Eventos")]
    public UnityEvent onDamageApplied;
    System.Collections.Generic.Dictionary<int, float> lastDamageTimeByTarget = new System.Collections.Generic.Dictionary<int, float>();

    void OnTriggerEnter2D(Collider2D other) => TryApplyDamage(other, onEnter: true);
    void OnTriggerStay2D(Collider2D other)
    {
        if (damageOnStay)
            TryApplyDamage(other, onEnter: false);
    }

    void OnCollisionEnter2D(Collision2D collision) => TryApplyDamage(collision.collider, onEnter: true);
    void OnCollisionStay2D(Collision2D collision)
    {
        if (damageOnStay)
            TryApplyDamage(collision.collider, onEnter: false);
    }

    /// <summary>
    /// Intento de aplicar daño cuando choca con otro collider.
    /// </summary>
    void TryApplyDamage(Collider2D other, bool onEnter)
    {
        if (other == null) return;

        if (useTag)
        {
            if (!other.CompareTag(playerTag)) return;
        }
        else
        {
            if (((1 << other.gameObject.layer) & targetLayerMask) == 0) return;
        }

        int id = other.gameObject.GetInstanceID();
        float now = Time.time;
        if (lastDamageTimeByTarget.TryGetValue(id, out float last))
        {
            if (now - last < damageCooldown) return;
        }

        var ph = other.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            ph.TakeDamage(damage);
        }
        else
        {
            var genericHealth = other.GetComponent("Health") as MonoBehaviour;
            if (genericHealth != null)
            {
                var mi = genericHealth.GetType().GetMethod("TakeDamage");
                if (mi != null) mi.Invoke(genericHealth, new object[] { damage });
            }
        }
        if (applyKnockback && knockbackStrength > 0f)
        {
            var rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 dir = (other.transform.position - transform.position).normalized;
                rb.AddForce(dir * knockbackStrength, ForceMode2D.Impulse);
            }
        }

        onDamageApplied?.Invoke();
        lastDamageTimeByTarget[id] = now;
        if (destroyTargetOnContact)
        {
            Destroy(other.gameObject);
        }
        if (!keepThisOnContact)
        {
            Destroy(gameObject);
        }
    }
}
