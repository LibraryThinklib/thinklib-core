using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Contadores")]
    public int moedas = 0;
    public int vidas = 3;

    [Header("Referência da UI")]
    public Text moedasText;
    public Text vidasText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AdicionarColetavel(TipoColetavel tipo, int valor)
    {
        switch (tipo)
        {
            case TipoColetavel.Moeda:
                moedas += valor;
                break;
            case TipoColetavel.Vida:
                vidas += valor;
                break;
        }

        AtualizarUI();
    }

    private void AtualizarUI()
    {
        if (moedasText != null)
            moedasText.text = "Moedas: " + moedas;

        if (vidasText != null)
            vidasText.text = "Vidas: " + vidas;
    }
}
