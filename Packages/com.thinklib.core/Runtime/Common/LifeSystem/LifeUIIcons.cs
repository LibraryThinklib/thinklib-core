using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LifeUIIcons : MonoBehaviour
{
    [Header("Icon Configuration")]
    [SerializeField] private Image originalIcon; // Image already present in the hierarchy
    [SerializeField] private Sprite activeIcon;
    [SerializeField] private Sprite inactiveIcon;

    private List<Image> iconList = new List<Image>();

    public void UpdateIcons(int currentHealth, int maxHealth)
    {
        if (originalIcon == null)
        {
            Debug.LogWarning("No original icon assigned.");
            return;
        }

        if (iconList.Count != maxHealth)
        {
            GenerateIcons(maxHealth);
        }

        for (int i = 0; i < iconList.Count; i++)
        {
            iconList[i].sprite = i < currentHealth ? activeIcon : inactiveIcon;
            iconList[i].enabled = i < maxHealth;
        }
    }

    private void GenerateIcons(int amount)
    {
        // Hide the original icon
        originalIcon.gameObject.SetActive(false);

        // Remove all old icons (except the original)
        foreach (Transform child in transform)
        {
            if (child != originalIcon.transform)
                Destroy(child.gameObject);
        }

        iconList.Clear();

        for (int i = 0; i < amount; i++)
        {
            Image newIcon = Instantiate(originalIcon, transform);
            newIcon.gameObject.SetActive(true);
            iconList.Add(newIcon);
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
