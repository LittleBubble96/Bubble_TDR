using System.Collections;
using System.Collections.Generic;
using BubbleFramework.Bubble_Event;
using UnityEngine;

public class SM_Level3 : SM_LevelData
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

    public override void Init()
    {
        base.Init();
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
            CurCharacter.characterAnimator.AnimatorState.Success = true;
            ELevelState = ELevelState.WaitSettle;
        }

        if (CurCharacter.characterAnimator.AnimatorState.Failed)
        {
            ELevelState = ELevelState.WaitSettle;
        }

        //人物死亡
        if (CurCharacter.transform.position.y<_deathPoint.transform.position.y)
        {
            CurCharacter.characterAnimator.AnimatorState.Failed = true;
        }
        //AI行为
        foreach (var ai in CharacterAIs)
        {
            ai.DoUpdate(dt);
        }
    }

    public override string GetGameSettleString()
    {
        string settleString = CurCharacter.characterAnimator.AnimatorState.Success
            ? $"恭喜通关第{SM_SceneManager.Instance.CrossLevelCount}关!!!"
            : $"可惜止步{SM_SceneManager.Instance.CrossLevelCount}关!!!";
        return settleString;
    }

    public override string GetGameString()
    {
        int m = (int)CurGameTime / 60;
        int s = (int) CurGameTime % 60;
        return m +" : "+ s;
    }
}
