using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playercontroller : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 5.0f;
    [SerializeField] private float playerAirSpeed = 4.0f;
    [SerializeField] private float walljumpPower = 5.0f;
    [SerializeField] private float jumpPower = 5.0f;
    [SerializeField] private float playerHeight = 0.1f;
    [SerializeField] private TrailRenderer TR;

    private Rigidbody2D _playerRigidbody;
    public LayerMask groundLayer;
    public Animator animator;
    public bool isGrounded = true;
    public bool isWalled = true;
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 12f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;
    private bool isFacingRight = true;
    private float horizontal;
    public int WallJumps;


    void Start()
    {
        _playerRigidbody = GetComponent<Rigidbody2D>();
        if (_playerRigidbody == null)
        {
            Debug.LogError("Player is missing a Rigidbody2D component");
        } 
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        movementcontroller();

        //animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
       // animator.SetFloat("Speed", _playerRigidbody.velocity.magnitude / playerSpeed);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }


        if (Input.GetKeyDown(KeyCode.Space) && isWalled)
        {
            WallJump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        if (isGrounded)
        {
            WallJumps = 4;
        }

        Flip();


    }

    private void FixedUpdate()
    {

        isRunning();
        onLand();

        isGrounded = onGround();
        if (isDashing)
        {
            return;
        }

        isWalled = onWall();
    }

    private void movementcontroller ()
    {
        var horizontalInput = Input.GetAxisRaw("Horizontal");
        if (isGrounded)
        {
            _playerRigidbody.velocity = new Vector2(horizontalInput * playerSpeed, _playerRigidbody.velocity.y);
        }
        else
            _playerRigidbody.velocity = new Vector2(horizontalInput * playerAirSpeed, _playerRigidbody.velocity.y);
    }

private void Jump()
{
    if(Input.GetKeyDown(KeyCode.Space))
    {
        Debug.Log("Space is pressed");
        _playerRigidbody.velocity = new Vector2( 0, jumpPower);
        animator.SetBool("isJumping", true);
    }
}

private void WallJump()
{
    Vector2 localScaleHorizont = isFacingRight ? Vector2.right : Vector2.left;
    if (isGrounded)
    {
        WallJumps = 2;
    }
    if(Input.GetKeyDown(KeyCode.Space) && WallJumps != 0 && isGrounded == false)
    {
        Debug.Log("Space is pressed");
        if(isFacingRight)
        {
            _playerRigidbody.velocity = new Vector2(walljumpPower * -1 , jumpPower);
        }
        else
        _playerRigidbody.velocity = new Vector2(walljumpPower , jumpPower);
        WallJumps -- ; 
    }
}

    private bool onGround() 
    {
    Vector2 position = transform.position;
    Vector2 direction = Vector2.down;
    float distance = playerHeight;

    Debug.DrawRay(position, direction * distance, Color.green);
    RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
    if (hit.collider != null) 
    {
        //Debug.Log("On ground!!!");
        return true;
    }
    //Debug.Log("XYECOC");
    return false;
}

private bool onWall() 
    {
    Vector2 position = transform.position;
    Vector2 localScaleHorizont = isFacingRight ? Vector2.right : Vector2.left;
    float distance = 0.7f;

    Debug.DrawRay(position, localScaleHorizont * distance, Color.green);
    RaycastHit2D wallhit = Physics2D.Raycast(position, localScaleHorizont, distance, groundLayer);
    if (wallhit.collider != null) 
    {
        //Debug.Log("On wall!!!");
        return true;
    }
    //Debug.Log("walina");
    return false;
}

private IEnumerator Dash()
{
    canDash = false;
    isDashing = true;
    float originalGravity = _playerRigidbody.gravityScale;
    _playerRigidbody.gravityScale = 0f;
    _playerRigidbody.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
    TR.emitting = true;
    yield return new WaitForSeconds(dashingTime);
    TR.emitting = false;
    _playerRigidbody.gravityScale = originalGravity;
    isDashing = false;
    yield return new WaitForSeconds(dashingCooldown);
    canDash = true; 
}

private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

public void onLand()
{
    Vector2 position = transform.position;
    Vector2 direction = Vector2.down;
    float distance = playerHeight;

    RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
    if (hit.collider != null) 
    {
        //Debug.Log("On ground!!!");
        animator.SetBool("isJumping", false);
    }
    else
        animator.SetBool("isJumping", true);
        return;
}

public void isRunning()
{
    animator.SetFloat("Speed", _playerRigidbody.velocity.magnitude / playerSpeed);
}

}
/*public void isAttacking()
{
    animator.SetFloat("Attack", ) //Переменная аниматора для анимации
}

public void isJumping()
{
    animator.SetFloat("Jump", ) //Переменная аниматора для анимации
}

public void isDashing()
{
    animator.SetFloat("Dash", ) //Переменная аниматора для анимации
}

public void isTakingDamage()
{
    animator.SetFloat("Hit", ) //Переменная аниматора для анимации
}

public void */

