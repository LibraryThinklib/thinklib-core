using UnityEngine;

[AddComponentMenu("Thinklib/Platformer/Enemy/Shooter/Projectile Damage Dealer", -89)]
[RequireComponent(typeof(Collider2D))]
public class ProjectileDamageDealer : MonoBehaviour
{
    [Header("Dano")]
    public int damage = 1;

    [Header("Configura��o de colis�o")]
    public string targetTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            LifeSystemController life = other.GetComponent<LifeSystemController>();
            PlayerHurtEffect hurtEffect = other.GetComponent<PlayerHurtEffect>();

            if (life != null && (hurtEffect == null || !hurtEffect.IsInvulnerable))
            {
                life.TakeDamage(damage);

                if (hurtEffect != null)
                    hurtEffect.TriggerInvulnerability();
            }

            Destroy(gameObject);
        }
    }
}
