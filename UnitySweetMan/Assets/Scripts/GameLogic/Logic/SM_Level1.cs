using System.Collections;
using System.Collections.Generic;
using BubbleFramework.Bubble_Event;
using UnityEngine;

public class SM_Level1 : SM_LevelData
{
    [Bubble_Name("游戏计时")] 
    public float GameTime;

    private float curGameTime;
    public float CurGameTime
    {
        get => curGameTime;
        set
        {
            if (curGameTime!=value)
            {
                curGameTime = value;
                BubbleFrameEntry.GetModel<AppEventDispatcher>().BroadcastListener(EventName.EVENT_CHANGECROSSCOUNT);
            }
        }
    }
   
    
    private Transform EndTriggerRect;
    
    

    public override void Init()
    {
        base.Init();
        EndTriggerRect = transform.Find("EndTriggerRect");
        CurGameTime = GameTime;

    }

    public override void DoUpdate(float dt)
    {
        base.DoUpdate(dt);
        if (ELevelState!=ELevelState.Playing)
        {
            return;
        }
      
        CurGameTime -= dt;
       
        //判断结束
        if (CurGameTime <= 0 )
        {
            CurCharacter.characterAnimator.AnimatorState.Failed = true;
            ELevelState = ELevelState.WaitSettle;
        }
        
        
        //主角是否过关
        if (!CurCharacter.characterAnimator.AnimatorState.Success && IsContainEndTrigger(CurCharacter.transform))
        {
            DDebug.Log("主角过关");
            CurCharacter.characterAnimator.AnimatorState.Success = true;
            ELevelState = ELevelState.WaitSettle;
        }



        //人物死亡
        if (CurCharacter.transform.position.y<_deathPoint.transform.position.y)
        {
            SetCharacterBirth();
        }
        //AI行为
        foreach (var ai in CharacterAIs)
        {
            ai.DoUpdate(dt);
        }
    }

    public override string GetGameString()
    {
        int m = (int)CurGameTime / 60;
        int s = (int) CurGameTime % 60;
        return m +" : "+ s;
    }

    public override string GetGameSettleString()
    {
        string settleString = CurCharacter.characterAnimator.AnimatorState.Success
            ? $"恭喜通关第{SM_SceneManager.Instance.CrossLevelCount}关!!!"
            : $"可惜止步{SM_SceneManager.Instance.CrossLevelCount}关!!!";
        return settleString;
    }

    /// <summary>
    /// 是否在重点
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public bool IsContainEndTrigger(Transform character)
    {
        var localScale = EndTriggerRect.localScale;
        float xLength = localScale.x;
        float yLength = localScale.y;
        float zLength = localScale.z;
        var endPosition = EndTriggerRect.position;
        var characterPosition = character.position;
        bool contain =
            characterPosition.x > endPosition.x - xLength * 0.5f &&
            characterPosition.x < endPosition.x + xLength * 0.5f &&
            characterPosition.y > endPosition.y - yLength * 0.5f &&
            characterPosition.y < endPosition.y + yLength * 0.5f &&
            characterPosition.z > endPosition.z - zLength * 0.5f &&
            characterPosition.z < endPosition.z + zLength * 0.5f;
        return contain;
    }
}
