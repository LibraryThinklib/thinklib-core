using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class TimedPlatform : MonoBehaviour
{
    public enum PlatformBehavior
    {
        Fall,       // A plataforma cai
        Disappear   // A plataforma desaparece gradualmente
    }

    public PlatformBehavior behavior = PlatformBehavior.Disappear;

    public float delayBeforeAction = 1f;   // Tempo antes da ação ocorrer
    public float fadeDuration = 1f;        // Tempo para desaparecer (fade out)

    public bool enableRespawn = false;     // Define se a plataforma reaparece
    public float respawnDelay = 2f;        // Tempo até reaparecer

    public string activatorTag = "Player"; // Quem pode ativar a plataforma

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;
    private Color originalColor;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isTriggered = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        originalColor = sr.color;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ativa a plataforma se colidir com o jogador e ainda não tiver sido ativada
        if (isTriggered) return;

        if (collision.gameObject.CompareTag(activatorTag))
        {
            isTriggered = true;
            Invoke(nameof(TriggerAction), delayBeforeAction);
        }
    }

    private void TriggerAction()
    {
        if (behavior == PlatformBehavior.Fall)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;

            if (enableRespawn)
                Invoke(nameof(ResetPlatform), respawnDelay);
        }
        else if (behavior == PlatformBehavior.Disappear)
        {
            StartCoroutine(FadeOutAndDisable());
        }
    }

    private IEnumerator FadeOutAndDisable()
    {
        float elapsed = 0f;
        Color c = sr.color;

        // Faz a plataforma desaparecer gradualmente
        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsed / fadeDuration);
            sr.color = new Color(c.r, c.g, c.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        sr.color = new Color(c.r, c.g, c.b, 0f);
        sr.enabled = false;
        col.enabled = false;

        if (enableRespawn)
            Invoke(nameof(ResetPlatform), respawnDelay);
    }

    private void ResetPlatform()
    {
        // Restaura a plataforma ao estado original
        sr.enabled = true;
        col.enabled = true;
        sr.color = originalColor;

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        rb.bodyType = RigidbodyType2D.Kinematic;

        isTriggered = false;
    }
}
