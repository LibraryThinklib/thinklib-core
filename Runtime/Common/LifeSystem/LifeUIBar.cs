using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu("Thinklib/Common/LifeSystem/Life UI Bar", -89)]
public class LifeUIBar : MonoBehaviour
{
    [SerializeField] private Image healthFill;

    [Header("Color Settings")]
    [SerializeField] private Color highHealthColor = new Color(0f, 1f, 0f);      // green
    [SerializeField] private Color mediumHealthColor = new Color(1f, 0.64f, 0f);  // orange
    [SerializeField] private Color lowHealthColor = new Color(1f, 0f, 0f);        // red

    public void UpdateBar(int currentHealth, int maxHealth)
    {
        if (healthFill != null && maxHealth > 0)
        {
            float percent = (float)currentHealth / maxHealth;
            healthFill.fillAmount = percent;

            // Change color based on remaining health
            if (percent >= 0.7f)
                healthFill.color = highHealthColor;
            else if (percent >= 0.3f)
                healthFill.color = mediumHealthColor;
            else
                healthFill.color = lowHealthColor;
        }
    }

    public void Shake(float intensity)
    {
        StartCoroutine(ShakeUI(transform, intensity));
    }

    private IEnumerator ShakeUI(Transform target, float intensity)
    {
        Vector3 originalPos = target.localPosition;

        for (int i = 0; i < 5; i++)
        {
            target.localPosition = originalPos + (Vector3)Random.insideUnitCircle * intensity;
            yield return new WaitForSeconds(0.02f);
        }

        target.localPosition = originalPos;
    }
}
