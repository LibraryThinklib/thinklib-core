using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;          // Prefab do inimigo a ser instanciado
    public Transform[] waypoints;           // Caminho que será passado para o inimigo
    public int enemiesToSpawn = 5;          // Quantos inimigos criar
    public float spawnInterval = 2f;        // Intervalo entre cada inimigo

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

            // Passa os waypoints para o inimigo
            EnemyPath enemyPath = enemy.GetComponent<EnemyPath>();
            if (enemyPath != null)
            {
                enemyPath.waypoints = waypoints;
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
