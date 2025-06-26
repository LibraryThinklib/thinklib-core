using UnityEngine;

public class EnemyUILockerAndFollower : MonoBehaviour
{
    [Tooltip("Transform do inimigo (objeto que ser� seguido)")]
    public Transform target;

    [Tooltip("Offset da UI em rela��o ao inimigo")]
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

        // Manter rota��o e escala originais (impede que a UI vire junto com o inimigo)
        transform.rotation = initialRotation;

        // Corrigir a escala para n�o flipar com o inimigo
        Vector3 correctedScale = initialScale;
        correctedScale.x = Mathf.Abs(initialScale.x); // for�a a UI a manter X positivo
        transform.localScale = correctedScale;
    }
}
