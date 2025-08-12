using UnityEngine;

[AddComponentMenu("Thinklib/TowerDefense/Defense/Bullet", -100)]
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
            Destroy(gameObject); // Destr�i a bala se o alvo n�o existir
            return;
        }

        // Dire��o do movimento (do centro da bala at� o alvo)
        Vector3 direction = (target.position - transform.position).normalized;

        // Movimenta a bala em dire��o ao alvo
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

            Destroy(gameObject); // Destr�i a bala
        }
    }
}
