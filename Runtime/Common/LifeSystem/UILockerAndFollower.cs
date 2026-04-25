using System;
using System.Collections.Generic;
using UnityEngine;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/Common/LifeSystem/UI Locker And Follower", -87)]
public class UILockerAndFollower : MonoBehaviour
{
    [Tooltip("Transform do alvo (objeto que será seguido)")]
    public Transform target;

    [Tooltip("Offset da UI em relação ao alvo")]
    public Vector3 offset = new Vector3(0, 1.5f, 0);

    private Vector3 initialScale;
    private Quaternion initialRotation;

    // Telemetry
    private const string MechanicName = "Common/LifeSystem/UILockerAndFollower";
    private bool _sentUsed = false;

    private void Awake()
    {
        // mechanic_instantiated
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(UILockerAndFollower),
            new Dictionary<string, object> {
                { "hasTarget", target != null },
                { "offsetX", offset.x },
                { "offsetY", offset.y },
                { "offsetZ", offset.z }
            }
        );
    }

    private void Start()
    {
        initialScale = transform.localScale;
        initialRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        try
        {
            // Posicionar a UI no local desejado
            transform.position = target.position + offset;

            // Manter rotação e escala originais (impede que a UI vire junto com o alvo)
            transform.rotation = initialRotation;

            // Corrigir a escala para não flipar com o alvo
            Vector3 correctedScale = initialScale;
            correctedScale.x = Mathf.Abs(initialScale.x); // força a UI a manter X positivo
            transform.localScale = correctedScale;

            // mechanic_used: primeira vez que efetivamente acompanha o alvo
            if (!_sentUsed)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(UILockerAndFollower),
                    new Dictionary<string, object> {
                        { "trigger", "first_follow" }
                    }
                );
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(UILockerAndFollower),
                new Dictionary<string, object> {
                    { "where", "LateUpdate" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }
}
