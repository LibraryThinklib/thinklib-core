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

    private bool jaColetado = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (jaColetado) return;

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

            if (destruirAutomaticamente)
                Destroy(gameObject);
        }
    }
}
