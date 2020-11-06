using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SM_CharacterAnimator : MonoBehaviour
{
    private Animator _animator;

    public AniState AnimatorState = new AniState();

    [Bubble_Name("跳跃中间中断时间")]
    public float JumpBreakTime = 0.4f;

    private float tempJumpBreakTime;
    #region 属性参数

    private const string HorizontalParameter = "horizontal";
    private const string VerticalParameter = "vertical";
    
    private float tempHorParameterTime;
    private float horParameterTime = 0.2f;
    private float lastHorParameter;
    private float endHorParameter;
    
    private float tempVerParameterTime;
    private float verParameterTime = 0.2f;
    private float lastVerParameter;
    private float endVerParameter;
    
    #endregion
    

    /// <summary>
    /// 初始化 
    /// </summary>
    public void Init()
    {
        _animator = GetComponent<Animator>();
    }

    public void DoUpdate(float dt)
    {
        #region Parameter

        //HorizontalParameter fade
        if (tempHorParameterTime > 0)
        {
            tempHorParameterTime -= Time.deltaTime;
            var t = 1 - tempHorParameterTime / horParameterTime;
            _animator.SetFloat(HorizontalParameter, Mathf.Lerp(lastHorParameter, endHorParameter, t));
        }
        
        //VerticalParameter fade
        if (tempVerParameterTime > 0)
        {
            tempVerParameterTime -= Time.deltaTime;
            var t = 1 - tempVerParameterTime / verParameterTime;
            _animator.SetFloat(VerticalParameter, Mathf.Lerp(lastVerParameter, endVerParameter, t));
        }

        #endregion

        if (tempJumpBreakTime>0&&AnimatorState.Jumped)
        {
            tempJumpBreakTime -= dt;
            if (tempJumpBreakTime<=0)
            {
                SetAnimatorSpeed(0);
            }
        }
    }
    
    /// <summary>
    /// 设置HorizontalParameter参数
    /// </summary>
    /// <param name="value"></param>
    private void SetHorizontalParameter(float value)
    {
        endHorParameter = value;
        tempHorParameterTime = horParameterTime;
        lastHorParameter = _animator.GetFloat(HorizontalParameter);
    }

    /// <summary>
    /// 设置VerticalParameter参数
    /// </summary>
    /// <param name="value"></param>
    private void SetVerticalParameter(float value)
    {
        endVerParameter = value;
        tempVerParameterTime = verParameterTime;
        lastVerParameter = _animator.GetFloat(VerticalParameter);
    }

    #region 接口

    /// <summary>
    /// 设置跑步
    /// </summary>
    /// <param name="run"></param>
    public void SetRun(bool run)
    {
        AnimatorState.Running = run;
        if (!run) return;
        SetHorizontalParameter(-1);
        SetVerticalParameter(0);
    }

    /// <summary>
    /// 设置idle
    /// </summary>
    /// <param name="value"></param>
    public void SetIdle(bool value)
    {
        AnimatorState.Idle = value;
        if (!value) return;
        SetHorizontalParameter(0);
        SetVerticalParameter(0);
    }
    
    /// <summary>
    /// 设置Jump
    /// </summary>
    /// <param name="value"></param>
    public void SetJumped(bool value)
    {
        AnimatorState.Jumped = value;
        if (value)
        {
            SetHorizontalParameter(0);
            SetVerticalParameter(1);
            tempJumpBreakTime = JumpBreakTime;
        }
        else
        {
            SetAnimatorSpeed(1);
        }

    }
    #endregion

    /// <summary>
    /// 设置动画速度
    /// </summary>
    /// <param name="speed"></param>
    private void SetAnimatorSpeed(float speed)
    {
        _animator.speed = speed;
    }

}

public struct AniState
{
    /// <summary>
    /// 是否在idle
    /// </summary>
    public bool Idle;
    
    /// <summary>
    /// 是否跳跃
    /// </summary>
    public bool Jumped;

    /// <summary>
    /// 是否正在跑步
    /// </summary>
    public bool Running;
    
}


