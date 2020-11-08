using System.Collections;
using System.Collections.Generic;
using BubbleFramework.Bubble_Event;
using UnityEngine;

public class SM_Level2 : SM_LevelData
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
    }

    public override string GetGameString()
    {
        int m = (int)CurGameTime / 60;
        int s = (int) CurGameTime % 60;
        return m +" : "+ s;
    }
}
