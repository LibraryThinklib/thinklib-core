using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [Header("Configuração")]
    public float speed = 20f;
    [Tooltip("Tempo em segundos antes do projétil se auto-destruir (se não atingir nada).")]
    public float lifetime = 5.0f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        GetComponent<Collider2D>().isTrigger = true;
        
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector2 direction)
    {
        if (rb != null)
        {
            rb.velocity = direction.normalized * speed;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Target target = collision.GetComponent<Target>();

        if (target != null)
        {
            ScoreManager.instance.AddScore(target.scoreValue);
            
            target.OnHit();
        }

        Destroy(gameObject);
    }
}