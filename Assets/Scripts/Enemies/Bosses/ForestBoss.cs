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
    [SerializeField] private float movementSpeed;
    [SerializeField] private float stopDistance;
    [SerializeField] private float detectionRange;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float obstacleDetectionRange = 1f;
    [SerializeField] private LayerMask obstacleLayer;



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
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        isGrounded = IsGrounded();
        Debug.Log("Is Grounded: " + isGrounded);

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
    }

    private void MoveTowardsPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange && distanceToPlayer > stopDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * movementSpeed, rb.velocity.y);
            Debug.Log("Moving towards player");
            anim.SetBool("moving", true);

            if (IsObstacleAhead() && isGrounded)
            {
                Jump();
            }
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            Debug.Log("Stopping near player or out of detection range");
            anim.SetBool("moving", false);
        }
    }
    private bool IsObstacleAhead()
    {
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, obstacleDetectionRange, obstacleLayer);
        Debug.DrawRay(transform.position, direction * obstacleDetectionRange, Color.yellow); // Visualize the raycast
        return hit.collider != null;
    }


    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        Debug.Log("Jumping");
        anim.SetTrigger("Jump");
    }


    private bool IsGrounded()
    {
        Vector2 boxCenter = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y - 0.1f);
        Vector2 boxSize = new Vector2(boxCollider.bounds.size.x, 0.1f);
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCenter, boxSize, 0, Vector2.down, 0.1f, groundLayer);

        Debug.DrawRay(boxCenter, Vector2.down * 0.1f, Color.red);
        Debug.DrawRay(boxCenter + new Vector2(-boxSize.x / 2, 0), Vector2.down * 0.1f, Color.red);
        Debug.DrawRay(boxCenter + new Vector2(boxSize.x / 2, 0), Vector2.down * 0.1f, Color.red);
        Debug.Log("Ground check hit: " + (raycastHit.collider != null ? raycastHit.collider.name : "null"));

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
