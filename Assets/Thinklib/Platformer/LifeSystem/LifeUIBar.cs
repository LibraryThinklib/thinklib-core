using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LifeUIBar : MonoBehaviour
{
    [SerializeField] private Image barraDeVida;

    [Header("Cores configuráveis")]
    [SerializeField] private Color corVidaAlta = new Color(0f, 1f, 0f);     // verde
    [SerializeField] private Color corVidaMedia = new Color(1f, 0.64f, 0f);  // laranja
    [SerializeField] private Color corVidaBaixa = new Color(1f, 0f, 0f);     // vermelho

    public void AtualizarBarra(int vidaAtual, int vidaMaxima)
    {
        if (barraDeVida != null && vidaMaxima > 0)
        {
            float percentual = (float)vidaAtual / vidaMaxima;
            barraDeVida.fillAmount = percentual;

            // Trocar cor com base na vida restante
            if (percentual >= 0.7f)
                barraDeVida.color = corVidaAlta;
            else if (percentual >= 0.3f)
                barraDeVida.color = corVidaMedia;
            else
                barraDeVida.color = corVidaBaixa;
        }
    }

    public void Tremer(float intensidade)
    {
        StartCoroutine(TremerUI(transform, intensidade));
    }

    private IEnumerator TremerUI(Transform alvo, float intensidade)
    {
        Vector3 posOriginal = alvo.localPosition;

        for (int i = 0; i < 5; i++)
        {
            alvo.localPosition = posOriginal + (Vector3)Random.insideUnitCircle * intensidade;
            yield return new WaitForSeconds(0.02f);
        }

        alvo.localPosition = posOriginal;
    }
}
