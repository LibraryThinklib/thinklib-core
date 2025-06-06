using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LifeUIIcons : MonoBehaviour
{
    [Header("Configura��o de �cones")]
    [SerializeField] private Image iconeOriginal; // Image j� presente na hierarquia
    [SerializeField] private Sprite iconeAtivo;
    [SerializeField] private Sprite iconeInativo;

    private List<Image> icones = new List<Image>();

    public void AtualizarIcones(int vidaAtual, int vidaMaxima)
    {
        if (iconeOriginal == null)
        {
            Debug.LogWarning("Nenhum icone original foi atribuido.");
            return;
        }

        if (icones.Count != vidaMaxima)
        {
            GerarIcones(vidaMaxima);
        }

        for (int i = 0; i < icones.Count; i++)
        {
            if (i < vidaAtual)
                icones[i].sprite = iconeAtivo;
            else
                icones[i].sprite = iconeInativo;

            icones[i].enabled = i < vidaMaxima;
        }
    }

    private void GerarIcones(int quantidade)
    {
        // Oculta o icone original
        iconeOriginal.gameObject.SetActive(false);

        // Remove todos os icones antigos (exceto o original)
        foreach (Transform filho in transform)
        {
            if (filho != iconeOriginal.transform)
                Destroy(filho.gameObject);
        }

        icones.Clear();

        for (int i = 0; i < quantidade; i++)
        {
            Image novoIcone = Instantiate(iconeOriginal, transform);
            novoIcone.gameObject.SetActive(true);
            icones.Add(novoIcone);
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
