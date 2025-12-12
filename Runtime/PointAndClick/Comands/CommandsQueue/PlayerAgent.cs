using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Thinklib/Game/PlayerAgent")]
public class PlayerAgent : MonoBehaviour
{
    [Header("Velocidades")]
    public float moveSpeed = 3.0f;
    public float turnSpeed = 180.0f;

    private bool isExecuting = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

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
        StartCoroutine(ExecuteCommandQueue(commandQueue));
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