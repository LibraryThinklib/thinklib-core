using UnityEngine;
using System.Collections;

[AddComponentMenu("Thinklib/Common/Effects/Death Effect", -100)]
[RequireComponent(typeof(SpriteRenderer))]
public class DeathEffect : MonoBehaviour
{
    [Header("Effect Settings")]
    public float blinkDuration = 1.2f;
    public float blinkInterval = 0.1f;

    [Header("UI to Hide on Death (Optional)")]
    public GameObject[] uiElementsToHide;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Starts the death effect with blinking and destroys the object.
    /// </summary>
    public void PlayDeathEffect()
    {
        HideUIElements();
        StartCoroutine(BlinkAndDestroy());
    }

    private void HideUIElements()
    {
        foreach (var ui in uiElementsToHide)
        {
            if (ui != null)
                ui.SetActive(false);
        }
    }

    private IEnumerator BlinkAndDestroy()
    {
        float timer = 0f;

        while (timer < blinkDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        Destroy(gameObject);
    }
}
