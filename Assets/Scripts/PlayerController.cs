using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public PlayerData playerData;
    private AnimationScripts animationScripts;
    private Rigidbody2D rb;
    private EnchaneJumpPerformance enchaneJumpPerformance;
    private CollisionTracker colliTracker;
    // private InputSystem_Actions playerInputSystem;
    public ParticleSystem jumpParticale;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TrailRenderer trailRenderer;


    private int side = 1;
    public bool hasDashed;
    public bool groundTouched=false;
    public bool wallJumped;
    public bool isDashing;




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animationScripts = GetComponent<AnimationScripts>();
        colliTracker = GetComponent<CollisionTracker>();
        
    }
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);
        // Time.timeScale = 0.5f;
        animationScripts.SetAxisParamater(x, y, rb.linearVelocity.y);
        Walk(dir);

        if (colliTracker.onGround && !isDashing)
        {
        
            
        }
         if (Input.GetButtonDown("Dash") && !hasDashed)
        {
            if (xRaw != 0 || yRaw != 0)
            {
                Dash(xRaw, yRaw);
                return;
            }
        }
        if (Input.GetButtonDown("Jump"))
        {
            if (colliTracker.onGround)
            {
                Debug.Log("jump");
                Jump(Vector2.up);
            }
        }
       

        if (colliTracker.onGround && !groundTouched)
        {
            GroundTouch();
            groundTouched = true;
        }
        if (!colliTracker.onGround && groundTouched)
        {
            groundTouched = false;
        }
        if (x > 0)
        {
            side = 1;
            animationScripts.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            animationScripts.Flip(side);
        }
    }

    public void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;
    }

    private void Walk(Vector2 dir)
    {
        if (!wallJumped)
        {
            rb.linearVelocity = new Vector2(dir.x * playerData.moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, (new Vector2(dir.x * playerData.moveSpeed, rb.linearVelocity.y)), 10 * Time.deltaTime);
        }

    }
    private void Jump(Vector2 dir)
    {
        if (isDashing || hasDashed) return; 
        animationScripts.AnimatorTrigger("Jump");
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.linearVelocity += dir * playerData.jumpForce;


    }
   private void Dash(float x, float y)
{
    if (colliTracker.onGround && Input.GetButton("Jump")) return;

    Vector2 dir = new Vector2(x, y).normalized;

    hasDashed = true;
    isDashing = true;
    wallJumped = true;

    animationScripts.AnimatorTrigger("dashing");

    rb.gravityScale = 0; // turn off gravity
    rb.linearVelocity = dir * playerData.dashForce;

    StartCoroutine(DashWait());
}

IEnumerator DashWait()
{
        StartCoroutine(GroundDash());
    yield return new WaitForSeconds(playerData.dashTime);

    rb.gravityScale = 3;
    rb.linearVelocity = Vector2.zero;
    isDashing = false;
    wallJumped = false;
}
    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(0.15f);
        if(colliTracker.onGround)
            hasDashed = false;    
    }
}
