using UnityEngine;

public class UILockerAndFollower : MonoBehaviour
{
    [Tooltip("Transform do alvo (objeto que será seguido)")]
    public Transform target;

    [Tooltip("Offset da UI em relação ao alvo")]
    public Vector3 offset = new Vector3(0, 1.5f, 0);

    private Vector3 initialScale;
    private Quaternion initialRotation;

    private void Start()
    {
        initialScale = transform.localScale;
        initialRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Posicionar a UI no local desejado
        transform.position = target.position + offset;

        // Manter rotação e escala originais (impede que a UI vire junto com o alvo)
        transform.rotation = initialRotation;

        // Corrigir a escala para não flipar com o alvo
        Vector3 correctedScale = initialScale;
        correctedScale.x = Mathf.Abs(initialScale.x); // força a UI a manter X positivo
        transform.localScale = correctedScale;
    }
}
