using Unity.VisualScripting;
using UnityEngine;

public class CollisionTracker : MonoBehaviour
{
    [Header("Require")]
    public LayerMask groundLayer;
    public Vector2 bottomOffset;
    public float circleRadius;

    [Header("Status")]
    public bool onGround;
    void Start()
    {

    }

    void Update()
    {
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, circleRadius, groundLayer);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, circleRadius);

    }
}
