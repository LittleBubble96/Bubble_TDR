using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SM_CharacterAI : SM_CharacterAIBase
{

    [Bubble_Name("跳的高度")] 
    public float jumpHeight;

    public const string CollideTag = "JumpCollider";
    public const string CollideOnceTag = "JumpOnceCollider";

    private Collider lastOnceCollider;
    public bool NeedJump;
    
    public override void Init()
    {
        rb = GetComponent<Rigidbody>();
        Agent = GetComponent<NavMeshAgent>();
        DistToGround = GetComponent<Collider>().bounds.extents.y;
        CharacterAnimator = GetComponentInChildren<SM_CharacterAnimator>();
        CharacterAnimator.Init();
    }

    public override void DoUpdate(float dt)
    {
        
        //成功
        if (CharacterAnimator.AnimatorState.Success)
        {
            CharacterAnimator.SetSuccess();
            return;
        }
        //失败
        if (CharacterAnimator.AnimatorState.Failed)
        {
            CharacterAnimator.SetFailed();
            return;
        }
        CharacterAnimator.DoUpdate(dt);

        if (IsGrounded()&&!NeedJump)
        {
            if (Target!=null)
            {
                Agent.destination = Target.position;
            }
            
            var isMove = Agent.velocity.x != 0 && Agent.velocity.z != 0;
            CharacterAnimator.SetRun(isMove);

            var isIdle = Agent.velocity == Vector3.zero;
            CharacterAnimator.SetIdle(isIdle);
        }
    }
    
    /// <summary>
    /// 是否在地上
    /// </summary>
    /// <returns></returns>
    public override bool IsGrounded ()
    {
        var isGrounded = Physics.Raycast(transform.position, -Vector3.up, DistToGround + 0.01f);
        if (!isGrounded && !CharacterAnimator.AnimatorState.Jumped)
        {
            CharacterAnimator.SetJumped(true);
        }
        var isGroundedDown = Physics.Raycast(transform.position, -Vector3.up, DistToGround + 0.1f);
        if (CharacterAnimator.AnimatorState.Jumped && isGroundedDown)
        {
            CharacterAnimator.SetJumped(false);

        }
        return isGrounded;
    }

    /// <summary>
    /// 重写弹飞方法
    /// </summary>
    /// <param name="velocityF"></param>
    /// <param name="time"></param>
    public override void HitPlayer(Vector3 velocityF, float time)
    {
        rb.AddForce(velocityF);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(CollideTag))
        {
            if (IsGrounded() && !NeedJump)
            {
                Vector3 velocity = Agent.velocity;
                rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
                Agent.enabled = false;
                NeedJump = true;
                StartCoroutine(OnJumping());
                lastOnceCollider = other;
            }
        }
        else if (other.CompareTag(CollideOnceTag))
        {
            if (IsGrounded() && !NeedJump &&lastOnceCollider!=other)
            {
                Vector3 velocity = Agent.velocity;
                rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
                Agent.enabled = false;
                NeedJump = true;
                StartCoroutine(OnJumping());
                lastOnceCollider = other;
            }
        }
        
    }

    IEnumerator OnJumping()
    {
        yield return  new WaitForSeconds(Mathf.Sqrt(2 * jumpHeight / gravity)*2);
        Agent.enabled = true;
        NeedJump = false;
        //DDebug.Break();
    }

    /// <summary>
    /// 计算跳得高度
    /// </summary>
    /// <returns></returns>
    float CalculateJumpVerticalSpeed () {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }
}
