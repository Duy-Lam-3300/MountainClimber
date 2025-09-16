using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingGun : MonoBehaviour
{
    [Header("Scripts:")]
    public GrapplingRope grappleRope;
    public PlayerData playerData;


    [Header("Layer Settings:")]
    [SerializeField] private bool grappleToAll = false;
    [SerializeField] private LayerMask grappleMask; // Set this in Inspector (e.g. Wall, Ground, etc.)

    [Header("Main Camera")]
    public Camera m_camera;

    [Header("Transform Refrences:")]
    public Transform gunHolder;
    public Transform gunPivot;
    public Transform firePoint;

    [Header("Rotation:")]
    [SerializeField] private bool rotateOverTime = true;
    [Range(0, 80)][SerializeField] private float rotationSpeed = 4;

    [Header("Distance:")]
    [SerializeField] private bool hasMaxDistance = true;
    [SerializeField] private float maxDistance = 4;

    [Header("Max distance range:")]
    public int segments = 100;         // smoothness of circle
    public Color circleColor = new Color(1, 1, 1, 0.2f); // faint white with transparency


    [Header("Launching")]
    [SerializeField] private bool launchToPoint = true;
    [SerializeField] private LaunchType Launch_Type = LaunchType.Transform_Launch;
    [Range(0, 5)][SerializeField] private float launchSpeed = 5;

    [Header("No Launch To Point")]
    [SerializeField] private bool autoCongifureDistance = false;
    [SerializeField] private float targetDistance = 3;
    [SerializeField] private float targetFrequency = 3;




    [Header("Component Refrences:")]
    public SpringJoint2D m_springJoint2D;

    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 DistanceVector;
    Vector2 Mouse_FirePoint_DistanceVector;

    public Rigidbody2D playerRigi;

    private enum LaunchType
    {
        Transform_Launch,
        Physics_Launch,
    }
    private LineRenderer lineRenderer;



    private void Start()
    {
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;
        playerRigi.gravityScale = 1;


        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.loop = true;           // close the circle
        lineRenderer.useWorldSpace = false; // relative to object
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.positionCount = segments;

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = circleColor;
        lineRenderer.endColor = circleColor;

        DrawCircle();
    }

    private void Update()
    {
        Mouse_FirePoint_DistanceVector = m_camera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            SetGrapplePoint();
        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {

            playerData.canMoveAble = false;
            playerData.isGrappling = true;



            if (grappleRope.enabled)
            {
                RotateGun(grapplePoint, false);
            }
            else
            {
                RotateGun(m_camera.ScreenToWorldPoint(Input.mousePosition), false);
            }
            float x = Input.GetAxis("Horizontal");
            playerRigi.AddForce(new Vector2(x * 1f, 0)*Time.deltaTime);

            if (launchToPoint && grappleRope.isGrappling)
            {
                if (Launch_Type == LaunchType.Transform_Launch)
                {
                    gunHolder.position = Vector3.Lerp(gunHolder.position, grapplePoint, Time.deltaTime * launchSpeed);
                }
            }

        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            playerData.canMoveAble = true;

            grappleRope.enabled = false;
            m_springJoint2D.enabled = false;
            playerData.isGrappling = false;

            playerRigi.gravityScale = 1;

        }
        else
        {
            RotateGun(m_camera.ScreenToWorldPoint(Input.mousePosition), true);
        }
    }
    void LateUpdate()
{
    if (m_springJoint2D.enabled)
    {
        Vector2 toPlayer = playerRigi.position - grapplePoint;
        float angle = Vector2.SignedAngle(Vector2.down, toPlayer);

            // Restrict swing between -90° and +90° (half circle under anchor)
            if (angle < -100f || angle > 100f)
            {
                
                playerRigi.linearVelocity = Vector2.zero;
                playerRigi.gravityScale = 8f;
            }
            else
            {
                playerRigi.gravityScale = 1f;
                
            }
    }
}

    void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
    {
        Vector3 distanceVector = lookPoint - gunPivot.position;

        float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        if (rotateOverTime && allowRotationOverTime)
        {
            Quaternion startRotation = gunPivot.rotation;
            gunPivot.rotation = Quaternion.Lerp(startRotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotationSpeed);
        }
        else
            gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }

    void SetGrapplePoint()
    {

        RaycastHit2D _hit = Physics2D.Raycast(
               firePoint.position,
               Mouse_FirePoint_DistanceVector.normalized,
               hasMaxDistance ? maxDistance : Mathf.Infinity,
               grappleMask // only hit layers included in this mask
           );
        if (_hit.collider != null)
        {

            grapplePoint = _hit.point;
            DistanceVector = grapplePoint - (Vector2)gunPivot.position;
            grappleRope.enabled = true;
        }

    }

    public void Grapple()
    {

        if (!launchToPoint && !autoCongifureDistance)
        {
            m_springJoint2D.distance = targetDistance;
            m_springJoint2D.frequency = targetFrequency;
        }

        if (!launchToPoint)
        {
            if (autoCongifureDistance)
            {
                m_springJoint2D.autoConfigureDistance = true;
                m_springJoint2D.frequency = 0;
            }
            m_springJoint2D.connectedAnchor = grapplePoint;
            m_springJoint2D.enabled = true;
        }

        else
        {
            if (Launch_Type == LaunchType.Transform_Launch)
            {
                playerRigi.gravityScale = 0;
                playerRigi.linearVelocity = Vector2.zero;
            }
            if (Launch_Type == LaunchType.Physics_Launch)
            {
                m_springJoint2D.connectedAnchor = grapplePoint;
                m_springJoint2D.distance = 0;
                m_springJoint2D.frequency = launchSpeed;
                m_springJoint2D.enabled = true;
            }
        }
    }

    void DrawCircle()
    {
        lineRenderer.positionCount = segments;
        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.Deg2Rad * angleStep * i;
            float x = Mathf.Cos(angle) * maxDistance;
            float y = Mathf.Sin(angle) * maxDistance;

            lineRenderer.SetPosition(i, firePoint.position);
        }
    }

    private void OnDrawGizmos()
    {
        if (hasMaxDistance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistance);
        }
    }
}
