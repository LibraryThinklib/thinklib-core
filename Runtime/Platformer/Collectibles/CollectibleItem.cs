// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using UnityEngine;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/Platformer/Collectibles/Collectible Item", -100)]
[RequireComponent(typeof(Collider2D))]
public class CollectibleItem : MonoBehaviour
{
    public TipoColetavel tipo;
    public int valor = 1;

    [Header("Feedbacks")]
    public AudioClip somColeta;
    public ParticleSystem efeitoColeta;
    public bool destruirAutomaticamente = true;

    private bool jaColetado = false;

    // Telemetry
    private const string MechanicName = "Platformer/Collectibles/CollectibleItem";
    private bool _sentUsed = false;

    private void Awake()
    {
        // mechanic_instantiated
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(CollectibleItem),
            new Dictionary<string, object> {
                { "tipo", tipo.ToString() },
                { "valor", valor },
                { "hasSound", somColeta != null },
                { "hasEffect", efeitoColeta != null },
                { "destroyOnCollect", destruirAutomaticamente }
            }
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (jaColetado) return;

        try
        {
            if (other.CompareTag("Player"))
            {
                jaColetado = true;

                if (GameManager.Instance != null)
                    GameManager.Instance.AdicionarColetavel(tipo, valor);

                if (somColeta != null)
                    AudioSource.PlayClipAtPoint(somColeta, transform.position);

                if (efeitoColeta != null)
                {
                    ParticleSystem efeito = Instantiate(efeitoColeta, transform.position, Quaternion.identity);
                    Destroy(efeito.gameObject, efeito.main.duration + efeito.main.startLifetime.constantMax);
                }

                // mechanic_used (primeira coleta efetiva)
                if (!_sentUsed)
                {
                    _sentUsed = true;
                    ThinklibTelemetry.Track(
                        "mechanic_used",
                        MechanicName,
                        nameof(CollectibleItem),
                        new Dictionary<string, object> {
                            { "tipo", tipo.ToString() },
                            { "valor", valor }
                        }
                    );
                }

                if (destruirAutomaticamente)
                    Destroy(gameObject);
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(CollectibleItem),
                new Dictionary<string, object> {
                    { "where", "OnTriggerEnter2D" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }
}
