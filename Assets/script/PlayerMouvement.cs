using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouvement : MonoBehaviour
{


    public AudioSource src;
    public AudioClip sfx1, sfx2;


    private float mvtInputDirection;
    private Rigidbody2D rb;
    public float mvtVitesse = 10.0f;
    private bool isFacingRight = true;
    public float jumpForce = 16.0f;
    private Animator anim;
    private bool isWalking;
    public Transform groundCheck;
    public Transform wallCheck;
    private bool isGrounded;
    private bool canNormalJump;
    private bool canWallJump;
    public float groundCheckRadius;
    public LayerMask whatIsGround;
    public int nb2jump = 1;
    private int nb2jumpRestant;
    private bool isTouchingWall;
    public float wallCheckDistance;
    private bool isWallSlide;
    public float WallSlideSpeed;
    public float mvtForceAir;
    public float airDragMult = 0.95f;
    public float variableJumpHeightMult = 0.1f;
    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;
    public float wallHopForce;
    public float wallJumpForce;
    private int facingDirection = 1;
    private float jumpTimer;
    public float jumpTimerSet = 0.15f;
    private bool isAttemptingToJump;
    private bool checkJumpMult;
    private bool canMove;
    private bool canFlip;
    private float turnTimer;
    public float turnTimerSet = 0.1f;
    private float wallJumpTimer;
    public float wallJumpTimerSet = 0.5f;
    private bool hasWallJumped;
    private int lastWallJumpDirection;
    private Transform ledgeCheck;
    private bool isTouchingLedge;
    private bool canClimbLedge = false;
    private bool ledgeDetected;
    private Vector2 ledgePosBot;
    private Vector2 ledgePos1;
    private Vector2 ledgePos2;
    public float ledgeClimbXoffset1 = 0f;
    public float ledgeClimbYoffset1 = 0f;
    public float ledgeClimbXoffset2 = 0f;
    public float ledgeClimbYoffset2 = 0f;
    private bool isDashing;
    public float dashTime;
    public float dashSpeed;
    public float distanceEntreImages;
    public float dashCoolDown;
    private float dashTimeLeft;
    private float lastImagesXpos;
    private float lastDash = -100f;
    private bool knockback;
    private float knockbackStartTime;
    [SerializeField]
    private float knockbackDuration;

    [SerializeField]
    private Vector2 knockbackSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        nb2jumpRestant = nb2jump;
        //vector = 1
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }


    void Update()
    {
        CheckInput();
        CheckMvtDirection();
        UpdateAnim();
        CheckIfCanJump();
        CheckWallSlide();
        CheckJump();
        //CheckLedgeClimb();
        CheckDash();
        CheckKnockback();
    }
    
    private void FixedUpdate()
    {
        ApplyMvt();
        CheckSurroundings();
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
        //isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, whatIsGround);

        if(isTouchingWall && !isTouchingLedge && !ledgeDetected)
        {
            ledgeDetected = true;
            ledgePosBot = wallCheck.position;
        }
    }

    private void CheckLedgeClimb()
    {
        if(ledgeDetected && !canClimbLedge)
        {
            canClimbLedge = true;

            if (isFacingRight)
            {
                ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) - ledgeClimbXoffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYoffset1);
                ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) + ledgeClimbXoffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYoffset2);
            }
            else
            {
                ledgePos1 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) + ledgeClimbXoffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYoffset1);
                ledgePos2 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) - ledgeClimbXoffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYoffset2);
            }
            canMove = false;
            canFlip = false;

            anim.SetBool("canClimbLedge", canClimbLedge);
        }
        if (canClimbLedge)
        {
            transform.position = ledgePos1;
        }
    }

    private void CheckKnockback(){
        if(Time.time >= knockbackStartTime + knockbackDuration && knockback){
            knockback = false;
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
        }
    }

    public void Knockback(int direction){
        knockback = true;
        knockbackStartTime = Time.time;

        rb.velocity = new Vector2(knockbackSpeed.x * direction, knockbackSpeed.y);
    }
    public bool GetDashStatus(){
        return isDashing;
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y < 0.01f)
        {
            nb2jumpRestant = nb2jump;
        }

        if (isTouchingWall)
        {
            canWallJump = true;
        }

        if(nb2jumpRestant <= 0)
        {
            canNormalJump = false;
        }
        else
        {
            canNormalJump = true;
        }
    }

    private void UpdateAnim()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelo", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSlide);
    }

    private void CheckInput()
    {
        mvtInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            if(isGrounded || (nb2jumpRestant > 0 && !isTouchingWall))
            {
                NormalJump();

                src.clip = sfx2;
                src.Play();
            }
            else
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
        }
        if(Input.GetButtonDown("Horizontal") && isTouchingWall)
        {
            if(!isGrounded && mvtInputDirection != facingDirection)
            {
                canMove = false;
                canFlip = false;

                turnTimer = turnTimerSet;
            }
        }

        if (turnTimer >= 0)
        {
            turnTimer -= Time.deltaTime;

            if(turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }

        if (checkJumpMult && !Input.GetButton("Jump"))
        {
            checkJumpMult = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMult);
        }
//dash
        if(Input.GetButtonDown("Dash")){
            if(Time.time >= (lastDash + dashCoolDown))
            AttemptToDash();
        }
    

    }
    private void AttemptToDash(){
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        AfterImagePool.Instance.GetFromPool();
        lastImagesXpos = transform.position.x;
    }
    private void CheckDash(){
        if(isDashing){
            if(dashTimeLeft > 0){
                canMove = false;
                canFlip = false;
                rb.velocity = new Vector2(dashSpeed * facingDirection, 0);
                dashTimeLeft -= Time.deltaTime;

                src.clip = sfx1;
                src.Play();

                if(Mathf.Abs(transform.position.x - lastImagesXpos) > distanceEntreImages)
                {
                    AfterImagePool.Instance.GetFromPool();
                    lastImagesXpos = transform.position.x;
                }
            }
            if(dashTimeLeft <= 0 || isTouchingWall){
                isDashing = false;
                canMove = true;
                canFlip = true;
            }
        }
    }
    private void CheckMvtDirection()
    {
        if(isFacingRight && mvtInputDirection < 0)
        {
            Flip();
        }
        else if(!isFacingRight && mvtInputDirection > 0)
        {
            Flip();
        }

        if(Mathf.Abs(rb.velocity.x)>= 0.01f)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    private void CheckJump()
    {
        
        if(jumpTimer > 0)
        {
            //wallJump
            if (!isGrounded && isTouchingWall && mvtInputDirection != 0 && mvtInputDirection != facingDirection)
            {
                WallJump();
            }else if(isGrounded){
                NormalJump();
            }
        }
        if(isAttemptingToJump)
        {
            jumpTimer -= Time.deltaTime;
        }

        if(wallJumpTimer > 0)
        {
            if(hasWallJumped && mvtInputDirection == -lastWallJumpDirection)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                hasWallJumped = false;
            }else if(wallJumpTimer <= 0)
            {
                hasWallJumped = false;
            }
            else
            {
                wallJumpTimer -= Time.deltaTime;
            }
        }

    }

    private void NormalJump()
    {
        if (canNormalJump && !isWallSlide)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            nb2jumpRestant--;
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMult = true;
        }
    }

    private void WallJump()
    {
        if (canWallJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0.0f);
            isWallSlide = false;
            nb2jumpRestant = nb2jump;
            nb2jumpRestant--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * mvtInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMult = true;
            turnTimer = 0;
            canMove = true;
            canFlip = true;
            hasWallJumped = true;
            wallJumpTimer = wallJumpTimerSet;
            lastWallJumpDirection = -facingDirection;
        }
    }

    private void ApplyMvt()
    {
        //ICI CHELOU
         if (!isGrounded && !isWallSlide && mvtInputDirection == 0 && !knockback)
         {
            rb.velocity = new Vector2(rb.velocity.x * airDragMult, rb.velocity.y);
         }
         else if(canMove && !knockback)
         {
            rb.velocity = new Vector2(mvtVitesse * mvtInputDirection, rb.velocity.y);
         }
     
        

        if (isWallSlide)
        {
            if(rb.velocity.y < -WallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -WallSlideSpeed);
            }
        }
    }

    public void DisableFlip()
    {
        canFlip = false;
    }

    public void EnableFlip()
    {
        canFlip = true;
    }

    private void Flip()
    {
        if (!isWallSlide && canFlip && !knockback)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
        
    } 

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));

        Gizmos.DrawLine(ledgePos1, ledgePos2);
    }

    private void CheckWallSlide()
    {
        if(isTouchingWall && mvtInputDirection == facingDirection && rb.velocity.y < 0 && !canClimbLedge)
        {
            isWallSlide = true;
        }
        else
        {
            isWallSlide = false;
        }
    }

    public void FinishLedgeClimb()
    {
        canClimbLedge = false;
        transform.position = ledgePos2;
        canMove = true;
        canFlip = true;
        ledgeDetected = false;
        anim.SetBool("canClimbLedge", canClimbLedge);
    }

    public int GetFacingDirection()
    {
        return facingDirection;
    }

}

