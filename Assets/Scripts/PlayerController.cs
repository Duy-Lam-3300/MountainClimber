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
    public bool groundTouched;
    public bool wallJumped;
    public bool isDashing;




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animationScripts = GetComponent<AnimationScripts>();
        colliTracker = GetComponent<CollisionTracker>();
        enchaneJumpPerformance = GetComponent<EnchaneJumpPerformance>();
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


        // enchaneJumpPerformance.enabled = true;
        if (Input.GetButtonDown("Jump") && colliTracker.onGround)
        {
            Jump(Vector2.up);
        }
        if (Input.GetButtonDown("Dash") && !hasDashed)
        {
            if (xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }

        if (colliTracker.onGround && !groundTouched)
        {
            GroundTouch();
            groundTouched = false;
        }
        if (!colliTracker.onGround && groundTouched)
        {
            groundTouched = true;
        }
        if (x > 0)
        {
            side = 1;
            animationScripts.Flip(side);
        }
        else if (x < 0)
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
        // rb.linearVelocity = new Vector2(dir.x * playerData.moveSpeed, rb.linearVelocity.y);

    }
    private void Jump(Vector2 dir)
    {
        animationScripts.AnimatorTrigger("Jump");
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.linearVelocity += dir * playerData.jumpForce;


    }
    private void Dash(float x, float y)
    {
        hasDashed = true;
        rb.gravityScale = 0;

        rb.linearVelocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);
        rb.linearVelocity += dir.normalized * playerData.dashForce;
        StartCoroutine(DashWait());
    }
    IEnumerator DashWait()
    {
        StartCoroutine(GroundDash());
        isDashing = true;
        wallJumped = true;
        rb.gravityScale = 0;
        enchaneJumpPerformance.enabled = false;
        yield return new WaitForSeconds(playerData.dashTime);
        rb.gravityScale = 1;
        isDashing = false;
        wallJumped = false;
        enchaneJumpPerformance.enabled = true;

    }
    IEnumerator GroundDash()
    {
        Debug.Log(playerData.dashTime / 2);
        yield return new WaitForSeconds(0.15f);
        if(colliTracker.onGround)
            hasDashed = false;    
    }
}
