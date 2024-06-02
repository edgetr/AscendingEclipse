using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class ForestBoss : MonoBehaviour
{
    [SerializeField] private float attackCoolDown;
    [SerializeField] private float range;
    [SerializeField] private float colliderDistance;
    [SerializeField] private int damage;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private BoxCollider2D attackCollider;

    [SerializeField] private GameOverScreen gameOverScreen;

    private float cooldownTimer = Mathf.Infinity;
    private Animator anim;
    private Health playerHealth;
    private Rigidbody2D rb;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        attackCollider.isTrigger = true; // Ensure the attack collider is set as a trigger
        playerHealth = GetComponent<Health>();
    }

    private void Start()
    {
        rb.bodyType = RigidbodyType2D.Kinematic; // Set to Kinematic to avoid physics interactions
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        // Attack only when player is in sight
        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCoolDown)
            {
                cooldownTimer = 0;
                anim.SetTrigger("Attack");
            }
        }
        if (playerHealth.currentHealth <= 0)
        {
            HandleBossDeath();
        }
    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        if (hit.collider != null)
            playerHealth = hit.transform.GetComponent<Health>();

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

    private void DamagePlayer()
    {
        if (PlayerInSight())
            playerHealth.TakeDamage(damage);
    }

    private void HandleBossDeath()
    {
        gameOverScreen.Setup();
    }

}
