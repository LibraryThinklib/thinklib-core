using UnityEngine;

[AddComponentMenu("Thinklib/TowerDefense/Defense/Tower Placement", -99)]
public class TowerPlacement : MonoBehaviour
{
    public GameObject towerPrefab;
    private TowerShop towerShop;

    void Start()
    {
        towerShop = FindObjectOfType<TowerShop>();
    }

    void Update()
    {
        if (towerShop.IsPlacingTower())
        {
            if (Input.GetMouseButtonDown(0))  // Verifica se o jogador clicou com o bot�o esquerdo
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;  // Garante que a torre ser� posicionada no plano 2D (z = 0)

                // Realiza um Raycast no local clicado para verificar se � uma �rea v�lida para colocar a torre
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

                if (hit.collider != null)
                {
                    // Verifica se o local tem a tag "Blocked"
                    if (hit.collider.CompareTag("Blocked"))
                    {
                        Debug.Log("�rea bloqueada! N�o � poss�vel colocar a torre aqui.");
                    }
                    else
                    {
                        // Coloca a torre onde o jogador clicou, se n�o for uma �rea bloqueada
                        Instantiate(towerPrefab, mousePosition, Quaternion.identity);
                        Debug.Log("Torre colocada!");

                        towerShop.StopPlacingTower();  // Para o modo de constru��o ap�s colocar a torre
                    }
                }
            }
        }
    }
}