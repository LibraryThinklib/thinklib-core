using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 1;
    private Transform target;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // Destrói a bala se o alvo não existir
            return;
        }

        // Direção do movimento (do centro da bala até o alvo)
        Vector3 direction = (target.position - transform.position).normalized;

        // Movimenta a bala em direção ao alvo
        transform.position += direction * speed * Time.deltaTime;

        // Rotaciona a bala para que a ponta esteja sempre voltada para o alvo
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // Se a bala atingir o inimigo
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance < 0.1f)
        {
            EnemyHealth eh = target.GetComponent<EnemyHealth>();
            if (eh != null)
                eh.TakeDamage(damage);

            Destroy(gameObject); // Destrói a bala
        }
    }
}
