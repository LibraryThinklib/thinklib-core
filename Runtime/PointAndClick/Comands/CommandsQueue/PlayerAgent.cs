using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/Game/PlayerAgent")]
public class PlayerAgent : MonoBehaviour
{
    [Header("Velocidades")]
    public float moveSpeed = 3.0f;
    public float turnSpeed = 180.0f;

    private const string MechanicName = "PointAndClick/CommandsQueue/PlayerAgent";

    private bool isExecuting = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool _sentUsed = false;

    private void Awake()
    {
        ThinklibTelemetry.Track("mechanic_instantiated", MechanicName, nameof(PlayerAgent),
            new Dictionary<string, object>
            {
                { "moveSpeed", moveSpeed },
                { "turnSpeed", turnSpeed }
            });
    }

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public IEnumerator MoveForward()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = transform.position + transform.up;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        transform.position = targetPos;
    }

    public IEnumerator TurnLeft()
    {
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = transform.rotation * Quaternion.Euler(0, 0, 90);

        float t = 0;
        float duration = 90.0f / turnSpeed;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }
        transform.rotation = targetRot;
    }

    public IEnumerator TurnRight()
    {
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = transform.rotation * Quaternion.Euler(0, 0, -90);

        float t = 0;
        float duration = 90.0f / turnSpeed;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }
        transform.rotation = targetRot;
    }

    public void RunCommandQueue(List<ICommand> commandQueue)
    {
        if (isExecuting) return;

        try
        {
            if (!_sentUsed)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track("mechanic_used", MechanicName, nameof(PlayerAgent),
                    new Dictionary<string, object>
                    {
                        { "action", "run_queue" },
                        { "commandCount", commandQueue.Count }
                    });
            }

            StartCoroutine(ExecuteCommandQueue(commandQueue));
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track("mechanic_error", MechanicName, nameof(PlayerAgent),
                new Dictionary<string, object>
                {
                    { "where", "RunCommandQueue" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                });
            throw;
        }
    }

    private IEnumerator ExecuteCommandQueue(List<ICommand> commandQueue)
    {
        isExecuting = true;
        foreach (ICommand command in commandQueue)
        {
            yield return StartCoroutine(command.Execute(this));
        }
        isExecuting = false;
        Debug.Log("Fila concluída!");
    }

    public void ResetAgent()
    {
        StopAllCoroutines();
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        isExecuting = false;
    }
}
