using UnityEngine;

public class EnchaneJumpPerformance : MonoBehaviour
{
    public float gravityMutiplier = 2f;
    public float lowGravityMutiplier = 7f;
    public float maxFallSpeed = 25f;
    private Rigidbody2D rigi;
    private PlayerController player;
    public PlayerData playerData;

    void Start()
    {
        rigi = GetComponent<Rigidbody2D>();
        player = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && (player.wallJumped ||playerData.isGrappling))
            return;
        if (rigi.linearVelocity.y < 0)
        {
            rigi.linearVelocity += Vector2.up * Physics2D.gravity.y * gravityMutiplier * Time.deltaTime;
        }
        else if (rigi.linearVelocity.y > 0 && !Input.GetButton("Vertical"))
        {
            rigi.linearVelocity += Vector2.up * Physics2D.gravity.y * lowGravityMutiplier * Time.deltaTime;

        }
        if (rigi.linearVelocity.y < -maxFallSpeed)
        {
            rigi.linearVelocity = new Vector2(rigi.linearVelocity.x, -maxFallSpeed);
        }
    }
}
