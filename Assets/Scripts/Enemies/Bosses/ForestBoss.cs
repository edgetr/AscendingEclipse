using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class ForestBoss : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float attackCoolDown;
    [SerializeField] private float range;
    [SerializeField] private float colliderDistance;
    [SerializeField] private int damage;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private int health;
    [SerializeField] private float deathDelay;
    [SerializeField] private Vector2 deathTeleportLocation = new Vector2(-1000, -1000);
    [SerializeField] private Transform player;
    [SerializeField] private float movementSpeed = 2f; // Movement speed of the boss
    [SerializeField] private float stopDistance = 1f; // Distance to stop from player

    private float cooldownTimer = Mathf.Infinity;
    private Animator anim;
    private Health playerHealth;
    private Rigidbody2D rb;
    private bool facingRight = true;
    private bool isGrounded = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (anim == null)
        {
            Debug.LogError("Animator component not found on the GameObject");
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on the GameObject");
        }

        if (boxCollider == null)
        {
            Debug.LogError("BoxCollider2D component not assigned in the Inspector");
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            if (player == null)
            {
                Debug.LogError("Player Transform not assigned and no GameObject with tag 'Player' found.");
            }
        }
    }

    private void Start()
    {
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; // Set to Dynamic to enable gravity
        }
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        isGrounded = IsGrounded();

        if (player != null)
        {
            MoveTowardsPlayer();
            FlipTowardsPlayer();
        }

        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCoolDown)
            {
                cooldownTimer = 0;
                if (anim != null)
                {
                    anim.SetTrigger("Attack");
                }
            }
        }

        anim.SetBool("Grounded", isGrounded);
    }

    private void MoveTowardsPlayer()
    {
        if (isGrounded)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer > stopDistance)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                rb.velocity = new Vector2(direction.x * movementSpeed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        if (hit.collider != null)
        {
            playerHealth = hit.transform.GetComponent<Health>();
            if (playerHealth == null)
            {
                Debug.LogError("Health component not found on the player");
            }
        }

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
        if (PlayerInSight() && playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }

        boxCollider.enabled = false;
        rb.simulated = false;
        transform.position = deathTeleportLocation;
        boxCollider.enabled = false;
        Destroy(gameObject);
    }

    private void FlipTowardsPlayer()
    {
        if (player != null)
        {
            if (player.position.x > transform.position.x && !facingRight)
            {
                Flip();
            }
            else if (player.position.x < transform.position.x && facingRight)
            {
                Flip();
            }
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
