using UnityEngine;

// Controls player movement, jumping, and animations
// Also handles interactions with platforms and game borders

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4.5f; // Horizontal movement speed
    public float jumpForce = 6.2f; // Upward force applied when jumping

    private Rigidbody2D rb;
    private Animator animator;

    private bool isGrounded = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        // Apply horizontal movement
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Flip sprite depending on direction
        if (moveInput != 0)
        {
            Vector3 newScale = transform.localScale;
            newScale.x = Mathf.Abs(newScale.x) * Mathf.Sign(moveInput);
            transform.localScale = newScale;
        }

        // Jump logic
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Animation state handling
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("yVelocity", rb.velocity.y);

        if (!isGrounded || rb.velocity.y > 0.1f)
        {
            animator.SetBool("isJumping", true);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ground detection only from above
        if (collision.gameObject.CompareTag("Platform"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    isGrounded = true;
                    transform.parent = collision.transform; // Stick to moving platform
                    break;
                }
            }
        }

        // Trigger game over if touching PlayerBorder
        if (collision.gameObject.CompareTag("PlayerBorder"))
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // When leaving the platform, unparent and mark as airborne
        if (collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = false;

            if (transform.parent == collision.transform && gameObject.activeInHierarchy)
            {
                transform.parent = null;
            }
        }
    }
}
