using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollectibleItem : MonoBehaviour
{
    public TipoColetavel tipo;
    public int valor = 1;

    [Header("Feedbacks")]
    public AudioClip somColeta;
    public ParticleSystem efeitoColeta;
    public bool destruirAutomaticamente = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AdicionarColetavel(tipo, valor);

            if (somColeta != null)
                AudioSource.PlayClipAtPoint(somColeta, transform.position);

            if (efeitoColeta != null)
                Instantiate(efeitoColeta, transform.position, Quaternion.identity);

            if (destruirAutomaticamente)
                Destroy(gameObject);
        }
    }
}
