using UnityEngine;
using UnityEngine.Events;

public class LifeSystemController : MonoBehaviour
{
    [Header("Configuração Geral")]
    public int vidaMaxima = 5;
    private int vidaAtual;

    [Header("Eventos")]
    public UnityEvent aoMorrer;

    [Header("Referência à UI")]
    public LifeUIBar barraDeVida;
    public LifeUIIcons iconesDeVida;

    [Header("Feedback Visual")]
    [SerializeField] private bool ativarTremor = false;

    [Tooltip("Intensidade do tremor ao perder vida")]
    [SerializeField, Range(0f, 10f)] private float intensidadeTremor = 0.05f;

    private void Start()
    {
        vidaAtual = vidaMaxima;
        AtualizarUI();
    }

    public void SetVidaMaxima(int novaVidaMaxima)
    {
        vidaMaxima = novaVidaMaxima;
        vidaAtual = Mathf.Min(vidaAtual, vidaMaxima);
        AtualizarUI();
    }

    public void PerderVida(int quantidade)
    {
        vidaAtual -= quantidade;
        vidaAtual = Mathf.Clamp(vidaAtual, 0, vidaMaxima);
        AtualizarUI();

        if (ativarTremor)
        {
            if (barraDeVida != null)
                barraDeVida.Tremer(intensidadeTremor);

            if (iconesDeVida != null)
                iconesDeVida.Tremer(intensidadeTremor);
        }

        if (vidaAtual <= 0)
        {
            aoMorrer?.Invoke();
        }
    }

    public void GanharVida(int quantidade)
    {
        vidaAtual += quantidade;
        vidaAtual = Mathf.Clamp(vidaAtual, 0, vidaMaxima);
        AtualizarUI();
    }

    private void AtualizarUI()
    {
        if (barraDeVida != null)
            barraDeVida.AtualizarBarra(vidaAtual, vidaMaxima);

        if (iconesDeVida != null)
            iconesDeVida.AtualizarIcones(vidaAtual, vidaMaxima);
    }
}
