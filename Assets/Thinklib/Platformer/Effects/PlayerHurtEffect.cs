using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerHurtEffect : MonoBehaviour
{
    [Header("Tempo de Invulnerabilidade")]
    public float invulnerabilityDuration = 1.5f;
    public float blinkInterval = 0.1f;

    private SpriteRenderer spriteRenderer;
    private bool isInvulnerable = false;
    private float invulnerabilityTimer = 0f;
    private float blinkTimer = 0f;

    private int originalLayer;

    public bool IsInvulnerable => isInvulnerable;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalLayer = gameObject.layer; // salva a layer original
    }

    private void Update()
    {
        if (isInvulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;
            blinkTimer -= Time.deltaTime;

            if (blinkTimer <= 0f)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                blinkTimer = blinkInterval;
            }

            if (invulnerabilityTimer <= 0f)
            {
                isInvulnerable = false;
                spriteRenderer.enabled = true;
                gameObject.layer = originalLayer; // volta à layer normal
            }
        }
    }

    public void TriggerInvulnerability()
    {
        isInvulnerable = true;
        invulnerabilityTimer = invulnerabilityDuration;
        blinkTimer = 0f;

        originalLayer = gameObject.layer;
        int invulnerableLayer = LayerMask.NameToLayer("PlayerInvulnerable");

        if (invulnerableLayer != -1)
        {
            gameObject.layer = invulnerableLayer;
        }
        else
        {
            Debug.LogWarning("⚠️ Layer 'PlayerInvulnerable' não encontrada. Certifique-se de que ela foi criada.");
        }
    }
}
