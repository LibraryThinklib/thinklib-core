using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemCollector : MonoBehaviour
{
    [Header("Configuração de Movimento")]
    [Tooltip("A 'dropzone' deve seguir o mouse?")]
    public bool followMouse = true;
    
    [Tooltip("Limites de movimento horizontal (coordenadas de mundo)")]
    public float minX = -8f;
    public float maxX = 8f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        // Garante que o collider é um trigger
        GetComponent<Collider2D>().isTrigger = true;
    }

    void Update()
    {
        if (followMouse)
        {
            MoveWithMouse();
        }
    }

    private void MoveWithMouse()
    {
        if (mainCamera == null) return;

        // Converte a posição do mouse na tela para uma posição no mundo 2D
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // Limita o movimento horizontal
        float clampedX = Mathf.Clamp(mouseWorldPos.x, minX, maxX);

        // Atualiza a posição do coletor (mantendo o Y e Z originais)
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    // Onde a mágica acontece!
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que colidiu tem o script FallingItem
        FallingItem fallingItem = other.GetComponent<FallingItem>();

        if (fallingItem != null)
        {
            // Pega os dados do item
            Item collectedItem = fallingItem.itemData;

            if (collectedItem != null)
            {
                // Adiciona o item ao inventário
                InventoryManager.instance.AddItem(collectedItem, 1);
                
                Debug.Log($"Item coletado: {collectedItem.name}");

                // (Opcional) Adicione um som de coleta aqui
                // AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            // Destroi o objeto do item que caiu
            Destroy(other.gameObject);
        }
    }
}