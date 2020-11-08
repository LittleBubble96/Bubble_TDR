using System;
using System.Collections;
using System.Collections.Generic;
using BubbleFramework.Bubble_Event;
using BubbleFramework.Bubble_UI;
using UnityEngine.UI;


public class UI_GameView : UI_Base<UI_GameContent>
{
    [Bubble_Name("通关人数",Describe = "这个是通关人数")]
    public Text CrossCount;
    
    public override void Init()
    {
        base.Init();
        UiType = UIType.Normal;
        
        //通关人数
        CrossCount.text = SM_SceneManager.Instance.CurLevelData.CurCrossLevelCount +
                          " / "+ 
                          SM_SceneManager.Instance.CurLevelData.CrossLevelCount;
    }

    private void OnEnable()
    {
        BubbleFrameEntry.GetModel<AppEventDispatcher>().AddEventListener<EventType>(EventName.EVENT_CHANGECROSSCOUNT,OnChangeCrossCount);
    }

    private void OnChangeCrossCount(EventType obj)
    {
        CrossCount.text = SM_SceneManager.Instance.CurLevelData.CurCrossLevelCount +
                          " / "+ 
                          SM_SceneManager.Instance.CurLevelData.CrossLevelCount;
    }

    private void OnDisable()
    {
        BubbleFrameEntry.GetModel<AppEventDispatcher>().RemoveEventListener<EventType>(EventName.EVENT_CHANGECROSSCOUNT,OnChangeCrossCount);
    }

    public override void SetContent(UI_BaseContent content)
    {
        base.SetContent(content);
    }
    
}
public class UI_GameContent :UI_BaseContent
{
    
}