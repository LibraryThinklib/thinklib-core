using UnityEngine;
using Thinklib.Platformer.Enemy.Core;

namespace Thinklib.Platformer.Enemy.Types
{
    [RequireComponent(typeof(ProjectileShooterBase))]
    public class EnemyShooterAI : MonoBehaviour
    {
        [Header("Referências")]
        public Transform player;

        [Header("Configuração de Disparo")]
        public float raioDeDisparo = 5f;
        public float tempoEntreTiros = 1.5f;

        [Header("Modo de Disparo")]
        public bool mirarNoAlvo = false;


        [Header("Modo de Funcionamento")]
        public bool isEstatico = true;
        public bool isPatrulheiro = false;

        [Header("Pontos de Patrulha (se patrulheiro)")]
        public Transform pontoA;
        public Transform pontoB;
        public float velocidadePatrulha = 2f;
        public float tolerancia = 0.1f;

        private ProjectileShooterBase shooter;
        private Animator animator;
        private Transform destinoAtual;
        private float cooldownAtual;
        private bool indoParaA = false;

        private void Awake()
        {
            shooter = GetComponent<ProjectileShooterBase>();
            animator = GetComponent<Animator>();
            destinoAtual = pontoB;
        }

        private void Update()
        {
            if (player == null) return;

            float distancia = Vector2.Distance(transform.position, player.position);

            if (distancia <= raioDeDisparo)
            {
                if (cooldownAtual <= 0f)
                {
                    Vector2 direcao;

                    if (mirarNoAlvo)
                        direcao = (player.position - shooter.launchPosition.position).normalized;
                    else
                        direcao = new Vector2(player.position.x > transform.position.x ? 1 : -1, 0);

                    shooter.ShootProjectile(direcao);
                    cooldownAtual = tempoEntreTiros;
                }

                animator.SetBool("IsWalking", false);
            }
            else if (isPatrulheiro)
            {
                Patrulhar();
            }

            cooldownAtual -= Time.deltaTime;
        }

        private void Patrulhar()
        {
            if (pontoA == null || pontoB == null) return;

            animator.SetBool("IsWalking", true);
            transform.position = Vector2.MoveTowards(transform.position, destinoAtual.position, velocidadePatrulha * Time.deltaTime);

            if (Vector2.Distance(transform.position, destinoAtual.position) <= tolerancia)
            {
                destinoAtual = (destinoAtual == pontoA) ? pontoB : pontoA;
                Flip();
            }
        }

        private void Flip()
        {
            Vector3 escala = transform.localScale;
            float direcao = destinoAtual.position.x - transform.position.x;

            if (direcao > 0f)
                escala.x = Mathf.Abs(escala.x);
            else if (direcao < 0f)
                escala.x = -Mathf.Abs(escala.x);

            transform.localScale = escala;
        }
    }
}
