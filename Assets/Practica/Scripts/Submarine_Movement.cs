using UnityEngine;

public class SubmarineMovement : MonoBehaviour
{
    public float moveSpeed = 5f; 
    public float tiltAngle = 10f; 
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        
        Vector2 movement = new Vector2(moveX, moveY).normalized * moveSpeed;
        rb.linearVelocity = movement;

        if (moveX > 0)
        {
            spriteRenderer.flipX = true;
            transform.rotation = Quaternion.Euler(0, 0, tiltAngle);
        }
        else if (moveX < 0)
        {
            spriteRenderer.flipX = false;
            transform.rotation = Quaternion.Euler(0, 0, -tiltAngle);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
