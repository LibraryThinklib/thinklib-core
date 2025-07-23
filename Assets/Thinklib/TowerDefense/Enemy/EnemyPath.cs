using UnityEngine;

public class EnemyPath : MonoBehaviour
{
    public Transform[] waypoints; // pontos do caminho
    public float speed = 2f;

    private int currentWaypointIndex = 0;

    void Update()
    {
        if (currentWaypointIndex < waypoints.Length)
        {
            // Move o inimigo na dire��o do waypoint atual
            Transform target = waypoints[currentWaypointIndex];
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Verifica se chegou perto o suficiente para mudar para o pr�ximo ponto
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < 0.1f)
            {
                currentWaypointIndex++;
            }
        }
        else
        {
            // Chegou ao fim do caminho
            Destroy(gameObject); // Destr�i o inimigo (poderia chamar um sistema de dano aqui)
        }
    }
}
