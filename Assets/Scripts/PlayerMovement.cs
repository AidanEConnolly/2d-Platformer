
using UnityEngine;
using System.Collections;
//The purpose of this script is to get the ;layer moving
public class PlayerMovement : MonoBehaviour
{
    // 
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Rigidbody2D body;
    private Animator anim;
    private float wallJumpCooldown;
    private float horizontalInput;
    public float dashSpeed;
    public float dashTime;
    private float dashCooldown;
    public float resetDashCooldown;




    private BoxCollider2D boxCollider;

    private void Awake()
    {
        //adds reference to character that allows for movement
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

        private void Update()
    {
        //get input for dash
        if (Input.GetButtonDown("Fire1"))
        {
            if (dashCooldown <= 0)
            {
                StartCoroutine(Dash());
            }
        }

        IEnumerator Dash()
        {
            float startTime = Time.time;
            float localScaleX = transform.localScale.x;

            while (Time.time < startTime + dashTime)
            {
                float movementSpeed = dashSpeed * Time.deltaTime;
                if (Mathf.Sign(localScaleX) == 1)
                {
                    transform.Translate(movementSpeed, 0, 0);
                }
                else
                {
                    transform.Translate(-movementSpeed, 0, 0);
                }
                dashCooldown = resetDashCooldown;
                yield return null;
            }

        }
        //creates variable for horizontal movement
        horizontalInput = Input.GetAxis("Horizontal");

        //dash
        dashCooldown -= Time.deltaTime;
        if (dashCooldown < 0)
        {
            dashCooldown = -1;
        }
        else
        {
            dashCooldown -= Time.deltaTime;
        }



        //checks speed in directions to flip character
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        //allows character to jump
      

        //Set Animator Parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

        if (wallJumpCooldown > 0.2f)
        {
           

            //allows player to move left and right
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
            if (onWall() && !isGrounded())
            {
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }
            else
                body.gravityScale = 3;

            //allows character to jump
            if (Input.GetKey(KeyCode.Space))
                Jump();

        }
        else
            wallJumpCooldown += Time.deltaTime;

    }


    //Set Jump 
    private void Jump()
    {
        if (isGrounded())
        {

            body.velocity = new Vector2(body.velocity.x, jumpPower);
            anim.SetTrigger("jump");

        }
        else if (onWall() && !isGrounded())
        {
            if (horizontalInput == 0)
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            //-mathf gets value of -1 or one, and making it - causes momentum to be opposite, perfect for a wall jump
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);

            wallJumpCooldown = 0;
        }
    }
 
    private bool isGrounded()
    {
        //Raycast is a system of rays to check the state of the player, and a box cast is a variant to help with making it look natural
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
        // returns null when player is not grounded
    }
    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }
}
