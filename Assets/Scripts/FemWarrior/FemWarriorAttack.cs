using UnityEngine;

public class FemWarriorAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackRange;
    [SerializeField] private int attackDamage;
    [SerializeField] private LayerMask enemyLayer;

    AudioManager audioManager;


    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;
    private bool isAttacking = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (!isAttacking && (Input.GetMouseButtonDown(0) && cooldownTimer > attackCooldown))
        {
            if (playerMovement.canAttack())
            {
                Attack();
                isAttacking = true;

            }
            else
            {
                DashAttack();
                isAttacking = true;

            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isAttacking = false;
        }
    }

    private void Attack()
    {
        anim.SetTrigger("Attack");
        cooldownTimer = 0;
        DealDamage();
        audioManager.PlaySFX(audioManager.attack);

    }

    private void DashAttack()
    {
        anim.SetTrigger("DashAttack");
        cooldownTimer = 0;
        DealDamage();
        audioManager.PlaySFX(audioManager.attack);
    }

    private void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Health>().TakeDamage(attackDamage);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}