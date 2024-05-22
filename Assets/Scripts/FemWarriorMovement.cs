using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;

    private void Awake()
    {
        // Grab references for Rigidbody2D and Animator from the object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        // Flip player when moving left-right
        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(10, 10, 10);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-10, 10, 10);

        // Set animator parameters
        anim.SetBool("Run", horizontalInput != 0);
        anim.SetBool("Grounded", isGrounded());

        // Prioritize wall animations over falling
        if (onWall())
        {
            anim.SetBool("OnSlide", onSlide());
            anim.SetBool("OnWall", onWall());
            anim.SetBool("OnFall", false);
        }
        else
        {
            anim.SetBool("OnWall", false);
            anim.SetBool("OnSlide", false);
            anim.SetBool("OnFall", onFall());
        }

        // Wall jump logic
        if (wallJumpCooldown > 0.2f)
        {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (onWall() && !isGrounded())
            {
                if (onSlide())
                {
                    WallSlide();
                }
                else
                {
                    body.gravityScale = 0;
                    body.velocity = Vector2.zero;
                }
            }
            else
            {
                body.gravityScale = 7;
            }

            if (Input.GetKey(KeyCode.Space))
                Jump();
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
        }
        else if (onWall() && !isGrounded())
        {
            if (horizontalInput == 0)
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x) * 10, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
            }

            wallJumpCooldown = 0;
            anim.SetTrigger("Jump");
        }
    }

    private void WallSlide()
    {
        body.velocity = new Vector2(0, -wallSlideSpeed);
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
        return !isGrounded() && !onWall() && body.velocity.y < 0;
    }

    private bool onSlide()
    {
        // Check if the player is on the wall, not grounded, and pressing the down key or 's' key
        return onWall() && !isGrounded() && (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S));
    }
}
