using UnityEngine;

public class FemWarriorAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackRange;
    [SerializeField] private int attackDamage;
    [SerializeField] private LayerMask enemyLayer;

    private Animator anim;
    private PlayerMovement playerMovement;
    private float cooldownTimer = Mathf.Infinity;
    private bool isAttacking = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
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

        if(Input.GetMouseButtonUp(0))
        {
            isAttacking = false;
        }
    }

    private void Attack()
    {
        anim.SetTrigger("Attack");
        cooldownTimer = 0;
        // Perform attack and damage enemies
        DealDamage();
    }

    private void DashAttack()
    {
        anim.SetTrigger("DashAttack");
        cooldownTimer = 0;
        // Perform dash attack and damage enemies
        DealDamage();
    }

    private void DealDamage()
    {
        // Get all enemies in the attack range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        // Deal damage to each enemy
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Health>().TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a sphere around the player to show the attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}