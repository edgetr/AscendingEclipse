using System.IO;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallSlideSpeed;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private bool isJumping;

    private void Awake()
    {
        // References for RigidBody2D and Animator
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

        // Flip player right-left according to where it goes
        if (horizontalInput > 0.01f)
        {
            transform.localScale = new Vector3(10, 10, 10);
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-10, 10, 10);
        }

        anim.SetBool("Run", horizontalInput != 0);
        anim.SetBool("Grounded", isGrounded());
        anim.SetBool("OnWall", onWall());

        if (!isGrounded() && body.velocity.y < 0 && !onWall())
        {
            anim.SetBool("OnFall", true);
        }
        else
        {
            anim.SetBool("OnFall", false);
        }

        if (wallJumpCooldown > 0.2f)
        {
            if (onWall() && !isGrounded())
            {
                if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                {
                    WallSlide();
                }
                else
                {
                    anim.SetBool("WallSlide", false);
                    anim.SetBool("OnWall", true);
                    body.gravityScale = 0;
                    body.velocity = Vector2.zero;
                }
            }
            else
            {
                anim.SetBool("WallSlide", false);
                anim.SetBool("OnWall", false);
                body.gravityScale = 7;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }
        }
        else
        {
            wallJumpCooldown += Time.deltaTime;
        }
    }

    private void Jump()
    {
        if (isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            anim.SetTrigger("Jump");
            isJumping = true;
        }
        else if (onWall() && !isGrounded())
        {
            wallJumpCooldown = 0;
            body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 20, 6);
            anim.SetTrigger("Jump");
        }
    }

    private void WallSlide()
    {
        body.velocity = new Vector2(body.velocity.x, -wallSlideSpeed);
        anim.SetBool("WallSlide", true);
        anim.SetBool("OnWall", false);  // Ensure OnWall is false when sliding
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    private bool onFall()
    {
        // Character is falling if not grounded, not on a wall, and has negative y-velocity
        return !isGrounded() && !onWall() && body.velocity.y < 0;
    }
}
