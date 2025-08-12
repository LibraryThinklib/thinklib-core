using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Thinklib/Platformer/Collectibles/Collectibles Manager", -99)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Moedas")]
    public int moedas = 0;
    public Text moedasText;

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
                AtualizarUI();
                break;

            case TipoColetavel.Vida:
                GameObject jogador = GameObject.FindGameObjectWithTag("Player");
                if (jogador != null)
                {
                    LifeSystemController vida = jogador.GetComponent<LifeSystemController>();
                    if (vida != null)
                    {
                        Debug.Log("Chamando GanharVida()");
                        vida.Heal(valor);
                    }
                }
                break;
        }
    }

    private void AtualizarUI()
    {
        if (moedasText != null)
            moedasText.text = "" + moedas;
    }
}
