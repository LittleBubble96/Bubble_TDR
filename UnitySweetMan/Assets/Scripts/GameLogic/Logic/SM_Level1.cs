using System.Collections;
using System.Collections.Generic;
using BubbleFramework.Bubble_Event;
using UnityEngine;

public class SM_Level1 : SM_LevelData
{
    [Bubble_Name("过关数")] 
    public int CrossLevelCount;
    
   
    
    private Transform EndTriggerRect;
    
    private int crossLevelCount;
    
    /// <summary>
    /// 当前过关数
    /// </summary>
    public int CurCrossLevelCount
    {
        get => crossLevelCount;
        private set
        {
            if (crossLevelCount!=value)
            {
                crossLevelCount = value;
                BubbleFrameEntry.GetModel<AppEventDispatcher>().BroadcastListener(EventName.EVENT_CHANGECROSSCOUNT);
            }
        }
    }

    public override void Init()
    {
        base.Init();
        EndTriggerRect = transform.Find("EndTriggerRect");

    }

    public override void DoUpdate(float dt)
    {
        base.DoUpdate(dt);
        if (ELevelState!=ELevelState.Playing)
        {
            return;
        }
        //判断结束
        if (CurCrossLevelCount >= CrossLevelCount)
        {
            ELevelState = ELevelState.WaitSettle;
        }
        //主角是否过关
        if (!CurCharacter.characterAnimator.AnimatorState.Success && IsContainEndTrigger(CurCharacter.transform))
        {
            DDebug.Log("主角过关");
            CurCharacter.characterAnimator.AnimatorState.Success = true;
            CurCrossLevelCount++;
            CurCharacter.Order = CurCrossLevelCount;
        }

        //AI是否过关
        foreach (var ai in CharacterAIs)
        {
            if (!ai.CharacterAnimator.AnimatorState.Success && IsContainEndTrigger(ai.transform))
            {
                DDebug.Log("AI过关");
                ai.CharacterAnimator.AnimatorState.Success = true;
                CurCrossLevelCount++;
                ai.Order = CurCrossLevelCount;
            }
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
        return CurCrossLevelCount + " / " + CrossLevelCount;
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
