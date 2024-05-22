using System.IO;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D body;
    private Animator anim;
    private bool grounded;

    private void Awake()
    {
        //References for RigidBody2D and Animator
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(horizontalInput*speed, body.velocity.y);

        // flips player right-left according to where it goes
        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(10, 10, 10);
        else if (horizontalInput < 0.01f)
            transform.localScale = new Vector3(-10, 10, 10);


        if (Input.GetKey(KeyCode.Space) && grounded)
            Jump();

        anim.SetBool("Run", horizontalInput != 0);
        anim.SetBool("Grounded", grounded);
    }

    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, speed);
        anim.SetTrigger("Jump");
        grounded = false;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
            grounded = true;
    }
}
