using UnityEngine;

[AddComponentMenu("Thinklib/Common/Effects/Player Hurt Effect", -99)]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerHurtEffect : MonoBehaviour
{
    [Header("Invulnerability Time")]
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
        originalLayer = gameObject.layer; // store original layer
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
                gameObject.layer = originalLayer; // restore original layer
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
            Debug.LogWarning("⚠️ Layer 'PlayerInvulnerable' not found. Make sure it exists in the project.");
        }
    }
}
