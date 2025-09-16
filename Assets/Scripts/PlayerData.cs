using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Move Data")]
    public float moveSpeed;
    public float lastsDir;

    [Space(20)]
    [Header("Jump Data")]
    public float jumpForce;
    public float jumpTime;


    [Space(20)]
    [Header("Dash Data")]
    public float dashForce;
    public float dashTime;
    public float dashCoolTime;

    [Space(20)]
    [Header("Data checker")]
    public bool canMoveAble;
    public bool isGrappling;




}
