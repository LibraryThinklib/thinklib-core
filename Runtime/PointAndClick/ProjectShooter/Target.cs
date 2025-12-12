using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[AddComponentMenu("Thinklib/Game/Target")]
public class Target : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("O valor 'n' que este alvo adiciona à pontuação.")]
    public int scoreValue = 10;

    public void OnHit()
    {
        Debug.Log($"Alvo '{gameObject.name}' atingido, +{scoreValue} pontos!");
        Destroy(gameObject);
    }
}