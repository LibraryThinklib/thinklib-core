using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/Platformer/Collectibles/Collectibles Manager", -99)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Moedas")]
    public int moedas = 0;
    public Text moedasText;

    // Telemetry
    private const string MechanicName = "Platformer/Collectibles/Manager";
    private bool _sentUsed = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // telemetry: componente instanciado
            ThinklibTelemetry.Track(
                "mechanic_instantiated",
                MechanicName,
                nameof(GameManager),
                new Dictionary<string, object> {
                    { "startCoins", moedas },
                    { "hasCoinsText", moedasText != null }
                }
            );
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void AdicionarColetavel(TipoColetavel tipo, int valor)
    {
        try
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

            // telemetry: primeiro uso efetivo (primeira coleta processada pelo manager)
            if (!_sentUsed)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(GameManager),
                    new Dictionary<string, object> {
                        { "tipo", tipo.ToString() },
                        { "valor", valor },
                        { "coinsAfter", moedas }
                    }
                );
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(GameManager),
                new Dictionary<string, object> {
                    { "where", "AdicionarColetavel" },
                    { "tipo", tipo.ToString() },
                    { "valor", valor },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void AtualizarUI()
    {
        if (moedasText != null)
            moedasText.text = "" + moedas;
    }
}
