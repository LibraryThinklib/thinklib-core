using UnityEngine;

[AddComponentMenu("Thinklib/Game/PlayerShooter")]
public class PlayerShooter : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("O prefab do projétil que será disparado.")]
    public GameObject projectilePrefab;
    
    [Tooltip("O ponto de onde o projétil deve sair.")]
    public Transform firePoint;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (firePoint == null)
        {
            Debug.LogError("O 'firePoint' não foi atribuído no PlayerShooter!");
            firePoint = transform;
        }
    }

    void Update()
    {
        Aim();

        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Aim()
    {
        if (mainCamera == null) return;

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector2 lookDirection = (Vector2)mouseWorldPos - (Vector2)transform.position;

        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Shoot()
    {
        if (projectilePrefab == null) return;

        GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Projectile projectileScript = projectileObj.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(firePoint.right);
        }
    }
}