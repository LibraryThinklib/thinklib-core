using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/Grid/GridAgent")]
public class GridAgent : MonoBehaviour
{
    [Header("Velocidades")]
    public float moveSpeed = 5.0f;

    private const string MechanicName = "PointAndClick/GridCoordinates/GridAgent";

    private bool isExecuting = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private int currentRow = 0;
    private int currentCol = 0;
    private bool _sentUsed = false;

    private void Awake()
    {
        ThinklibTelemetry.Track("mechanic_instantiated", MechanicName, nameof(GridAgent),
            new Dictionary<string, object>
            {
                { "moveSpeed", moveSpeed }
            });
    }

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        currentRow = 0;
        currentCol = 0;
    }

    public IEnumerator GoToCoordinates(int targetRow, int targetCol)
    {
        if (GridManager.instance == null)
        {
            Debug.LogError("GridManager não encontrado!");
            yield break;
        }

        while (currentRow != targetRow)
        {
            int direction = (targetRow > currentRow) ? 1 : -1;
            currentRow += direction;
            Vector3 nextPos = GridManager.instance.GetWorldPosition(currentRow, currentCol);
            yield return StartCoroutine(MoveToPosition(nextPos));
        }

        while (currentCol != targetCol)
        {
            int direction = (targetCol > currentCol) ? 1 : -1;
            currentCol += direction;
            Vector3 nextPos = GridManager.instance.GetWorldPosition(currentRow, currentCol);
            yield return StartCoroutine(MoveToPosition(nextPos));
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float distance = Vector3.Distance(startPos, targetPos);

        if (distance <= 0.001f) yield break;

        float duration = distance / moveSpeed;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        transform.position = targetPos;
    }

    public void RunCommandQueue(List<IGridCommand> commandQueue)
    {
        if (isExecuting) return;

        try
        {
            if (!_sentUsed)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track("mechanic_used", MechanicName, nameof(GridAgent),
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
            ThinklibTelemetry.Track("mechanic_error", MechanicName, nameof(GridAgent),
                new Dictionary<string, object>
                {
                    { "where", "RunCommandQueue" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                });
            throw;
        }
    }

    private IEnumerator ExecuteCommandQueue(List<IGridCommand> commandQueue)
    {
        isExecuting = true;
        foreach (IGridCommand command in commandQueue)
        {
            yield return StartCoroutine(command.Execute(this));
            yield return new WaitForSeconds(0.1f);
        }
        isExecuting = false;
    }

    public void ResetAgent()
    {
        StopAllCoroutines();
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        currentRow = 0;
        currentCol = 0;
        isExecuting = false;
    }
}
