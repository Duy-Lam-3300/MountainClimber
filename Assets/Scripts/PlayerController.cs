using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public PlayerData playerData;
    private AnimationScripts animationScripts;
    private Rigidbody2D rb;
    private CollisionTracker colliTracker;
    // private InputSystem_Actions playerInputSystem;
    public ParticleSystem jumpParticale;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TrailRenderer trailRenderer;


    private int side = 1;
    public bool groundTouched = false;
    public bool wallJumped;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animationScripts = GetComponent<AnimationScripts>();
        colliTracker = GetComponent<CollisionTracker>();
        playerData.canMoveAble = true;
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
        if (Mathf.Abs(x) > 0)
            Walk(dir);

        if (colliTracker.onGround)
        {


        }

        // if (Input.GetButtonDown("Jump"))
        if (Input.GetButtonDown("Vertical"))
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

    }

    private void Walk(Vector2 dir)
    {
        if (playerData.isGrappling) return;
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

        animationScripts.AnimatorTrigger("Jump");
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.linearVelocity += dir * playerData.jumpForce;


    }

}
