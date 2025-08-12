using UnityEngine;

[AddComponentMenu("Thinklib/Platformer/Enemy/Patroller/Damage On Touch", -100)]
[RequireComponent(typeof(Collider2D))]
public class DamageOnTouch : MonoBehaviour
{
    [Header("Dano ao tocar no jogador")]
    public int damage = 1;

    [Header("Tag do jogador")]
    public string targetTag = "Player";

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(targetTag))
        {
            LifeSystemController life = collision.collider.GetComponent<LifeSystemController>();
            PlayerHurtEffect hurt = collision.collider.GetComponent<PlayerHurtEffect>();

            if (life != null && (hurt == null || !hurt.IsInvulnerable))
            {
                life.TakeDamage(damage);

                if (hurt != null)
                    hurt.TriggerInvulnerability();
            }
        }
    }
}
