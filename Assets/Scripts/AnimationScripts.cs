using UnityEngine;

public class AnimationScripts : MonoBehaviour
{
    private Animator animator;
    private CollisionTracker colliTracker;

    public SpriteRenderer spr;

    void Start()
    {
        animator = GetComponent<Animator>();
        colliTracker = GetComponent<CollisionTracker>();
        spr = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        animator.SetBool("onGround", colliTracker.onGround);
    }
    public void SetAxisParamater(float x, float y,float vy)
    {
        animator.SetFloat("AxisX", x);
        animator.SetFloat("AxisY", y);
        animator.SetFloat("VerticalAxis", vy);
    }
    public void AnimatorTrigger(string name)
    {
        animator.SetTrigger(name);
    }

    public void Flip(int side)
    {
        bool state = (side == 1) ? false : true;
        spr.flipX = state;
    }

   
}
