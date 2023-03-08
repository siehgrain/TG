using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float slopeCheckDistance = 1f;
    [SerializeField] private float groundCheckDistance = 1f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private PhysicsMaterial2D friccao;
    [SerializeField] private PhysicsMaterial2D noFriccao;
    [SerializeField] private float accelerationTime = 0.1f;
    [SerializeField] private float decelerationTime = 0.05f;
    [SerializeField] private float dashDistance = 20f;    // The distance the player will dash
    [SerializeField] private float dashSpeed = 20f;       // The speed at which the player will dash
    [SerializeField] private float dashDuration = 0.02f;
    [SerializeField] private float dashCooldown = 3.0f;// The duration of the dash animation
    private bool canDash = true;
    private float lastDashTime;
    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 perpendicularSpeed;
    private bool isSloped = false;
    private bool isGrounded = false;
    private bool isDashing = false;
    private float gravity;
    private float coyoteTimeCounter;
    private float jumpVelocity;
    
    private float jumpBufferCounter;
    private float SlopeAngle;
    public float jumpHeight = 2f;  // Altura m�xima do salto
    public float timeToJumpApex = 0.4f;  // Tempo para alcan�ar o ponto mais alto do salto
    private bool isJumping;
    private bool isLanding = false;
    private bool isShooting = false;
    private bool dano = false;
    private bool isDead = false;
    private bool IsCharging = false;
    private bool WasOnGround;
    private float chargeTime = 0.0f;
    private float currentAccelerationTime = 0f;
    private float currentDecelerationTime = 0f;
    private float targetSpeed = 0f;



    private float horizontalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        // Calcula a gravidade e a velocidade do salto
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

    }

    private void FixedUpdate()
    {
        
        // Mover o jogador horizontalmente
        if (!isDashing)
        {
            float targetVelocityX = horizontalInput * moveSpeed;
            if (targetVelocityX != 0f)
            {
                targetSpeed = targetVelocityX;
                currentAccelerationTime = accelerationTime;
                currentDecelerationTime = 0f;
            }
            else
            {
                targetSpeed = 0f;
                currentAccelerationTime = 0f;
                currentDecelerationTime = decelerationTime;
            }

            float currentSpeed = Mathf.SmoothStep(rb.velocity.x, targetSpeed, currentAccelerationTime > 0f ? currentAccelerationTime : currentDecelerationTime);
            float maxSpeed = horizontalInput != 0f ? moveSpeed : moveSpeed / 2f;
            rb.velocity = new Vector2(Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed), rb.velocity.y);
            if (currentAccelerationTime > 0f)
            {
                currentAccelerationTime -= Time.deltaTime;
            }
            else if (currentDecelerationTime > 0f)
            {
                currentDecelerationTime -= Time.deltaTime;
            }
        }



        // Adiciona for�a de queda
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        // Aplica a gravidade
        rb.velocity += Vector2.up * gravity * Time.deltaTime;
    }

    private void Update()
    {
        animator.SetBool("IsShooting", isShooting);
        animator.SetBool("IsDead", isDead);
        animator.SetBool("IsCharging", IsCharging);
        animator.SetBool("IsLanding", isLanding);
        
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, LayerMask.GetMask("Ground"));
        DetectGround();
        DetectSlopes();
        bool isAboutToLand = Physics2D.Raycast(transform.position, Vector2.down, 1f, LayerMask.GetMask("Ground"));

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            if (isJumping)
            {
                isJumping = false;
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsLanding", true);
            }
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        //Pulo
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f && !isDashing)
        {
            Jump();
            jumpBufferCounter =0f;
            
        }
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x,rb.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }

        //Dash
        if (Input.GetButtonDown("Dash") && !isDashing)
        {
            StartCoroutine(Dash());
        }

        //verifica se o personagem está no chão
        

        //Carrega o tiro
        if (Input.GetButtonDown("Fire1") && isGrounded && !isDashing)
        {
            IsCharging = true;
        }

        //Movimentação
        if (!IsCharging)
        {
            HorizontalInput();
        }
        
        if (Input.GetButtonUp("Fire1") && IsCharging && isGrounded && !isDashing)
        {
            Shoot();
        }
    }
    private void DetectSlopes()
    {
        RaycastHit2D raycastHitSlope = Physics2D.Raycast(transform.position, Vector2.down, slopeCheckDistance, LayerMask.GetMask("Ground"));
        if (raycastHitSlope)
        {
            perpendicularSpeed = Vector2.Perpendicular(raycastHitSlope.normal).normalized;
            SlopeAngle = Vector2.Angle(raycastHitSlope.normal, Vector2.up);
            isSloped = SlopeAngle != 0;
        }
        if (isSloped && horizontalInput == 0)
        {
            rb.sharedMaterial = friccao;
        }
        else
        {
            rb.sharedMaterial = noFriccao;
        }
    }

    private void Shoot()
    {
        IsCharging = false;
        animator.SetBool("IsCharging", IsCharging);
        // Realizar a��o quando o bot�o foi mantido pressionado por tempo suficiente
        isShooting = true;
        animator.SetBool("IsShooting", isShooting);
        Invoke("ResetShoot", 0.5f);
    }
    private void Jump()
    {
        isJumping = true;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        if (isSloped && !isJumping)
        {
            // Se o jogador estiver em um slope, projete a velocidade de pulo na direção perpendicular ao slope
            Vector2 jumpDirection = Vector2.Perpendicular(perpendicularSpeed).normalized;
            float jumpSpeed = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(gravity));
            rb.velocity = jumpDirection * jumpSpeed;
        }
    }
    private void HandleLanding()
    {
        isJumping = false;
        animator.SetBool("IsLanding", true);
    }
    private void DetectGround(){
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, LayerMask.GetMask("Ground"));
        if(isGrounded && !WasOnGround) HandleLanding();
        WasOnGround = isGrounded;
    }
    private void HorizontalInput()
    {
        // Detectar entrada de movimento horizontal
        horizontalInput = Input.GetAxisRaw("Horizontal");

        //// Atualizar a anima��o
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));

        // Girar o jogador na dire��o correta
        if (horizontalInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1, 1);
        }
    }

   
private IEnumerator Dash()
{
    // Check if the player can dash
    if (canDash)
    {
        isDashing = true;
        animator.SetBool("IsDashing", isDashing);
        // Determine the direction of the dash based on the player's local scale
        int dashDirection = (int)Mathf.Sign(transform.localScale.x);

        // Calculate the endpoint of the raycast based on the dash direction and distance
        Vector2 endPoint = transform.position + new Vector3(dashDirection, 0, 0) * dashDistance;

        // Draw a debug ray to visualize the dash direction and distance
        Debug.DrawRay(transform.position, new Vector2(dashDirection, 0) * dashDistance, Color.blue, 0.1f);

        // Check for collisions with the ground layer in the dash path
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(dashDirection, 0), dashDistance, layerMask);

        // If the ray hits something, adjust the endpoint to stop at the collision point
        if (hit.collider != null)
        {
            endPoint = hit.point;
        }

        // Apply the dash force in the correct direction
        Vector2 velocity = (endPoint - (Vector2)transform.position).normalized * dashSpeed;
        rb.velocity = velocity;

        

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        animator.SetBool("IsDashing", isDashing);

        rb.velocity = Vector2.zero;

        // Reset the dash cooldown
        canDash = false;
        lastDashTime = Time.time;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}










    private void ResetShoot()
    {
        isShooting = false;
    }

}
