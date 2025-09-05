using UnityEngine;

public class EnchaneJumpPerformance : MonoBehaviour
{
    public float gravityMutiplier = 2f;
    public float lowGravityMutiplier = 7f;
    public float maxFallSpeed = 25f;
    public Rigidbody2D rigi;

    void Start()
    {
        rigi = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rigi.linearVelocity.y < 0)
        {
            rigi.linearVelocity += Vector2.up * Physics2D.gravity.y * gravityMutiplier * Time.deltaTime;
        }
        else if (rigi.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            rigi.linearVelocity += Vector2.up * Physics2D.gravity.y * lowGravityMutiplier * Time.deltaTime;

        }
        if (rigi.linearVelocity.y < -maxFallSpeed)
        {
            rigi.linearVelocity = new Vector2(rigi.linearVelocity.x, -maxFallSpeed);
        }
    }
}
