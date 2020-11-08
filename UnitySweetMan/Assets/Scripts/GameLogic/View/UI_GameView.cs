using System;
using System.Collections;
using System.Collections.Generic;
using BubbleFramework.Bubble_Event;
using BubbleFramework.Bubble_UI;
using UnityEngine;
using UnityEngine.UI;
using EventType = BubbleFramework.Bubble_Event.EventType;


public class UI_GameView : UI_Base<UI_GameContent>
{
    [Bubble_Name("通关人数",Describe = "这个是通关人数")]
    public Text CrossCount;

    [Bubble_Name("倒计时Text")]
    public Text CountDownText;

    [Bubble_Name("渐变Canvas")] 
    public CanvasGroup Alpha;
    public override void Init()
    {
        base.Init();
        UiType = UIType.Normal;
        
        //通关人数
        CrossCount.text = SM_SceneManager.Instance.CurLevelData.GetGameString();
    }

    private void OnEnable()
    {
        BubbleFrameEntry.GetModel<AppEventDispatcher>().AddEventListener<EventType>(EventName.EVENT_CHANGECROSSCOUNT,OnChangeCrossCount);
        BubbleFrameEntry.GetModel<AppEventDispatcher>().AddEventListener<EventType>(EventName.EVENT_COUNTDOWN,OnCountDown);
    }

    private void OnCountDown(EventType obj)
    {
        if (obj is MultiEvent<int> item) 
            StartCoroutine(OnCountDown(item.Value));
    }    
    
    /// <summary>
    /// 修改Game视图得内容
    /// </summary>
    /// <param name="obj"></param>
    private void OnChangeCrossCount(EventType obj)
    {
        CrossCount.text = SM_SceneManager.Instance.CurLevelData.GetGameString();
    }

    private void OnDisable()
    {
        BubbleFrameEntry.GetModel<AppEventDispatcher>().RemoveEventListener<EventType>(EventName.EVENT_CHANGECROSSCOUNT,OnChangeCrossCount);
        BubbleFrameEntry.GetModel<AppEventDispatcher>().RemoveEventListener<EventType>(EventName.EVENT_COUNTDOWN,OnCountDown);

    }

    public override void SetContent(UI_BaseContent content)
    {
        base.SetContent(content);
    }

    /// <summary>
    /// 开启协程
    /// </summary>
    /// <param name="times"></param>
    /// <returns></returns>
    IEnumerator OnCountDown(int times)
    {
        for (int i = times; i > 0; i--)
        {
            CountDownText.text = i + "";
            float tempTime = 1f;
            while (tempTime>0)
            {
                tempTime -= Time.deltaTime;
                var t = tempTime / times;
                Alpha.alpha = t;
                CountDownText.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.6f, 1 - t);
                yield return null;
            }
        }
    }

}
public class UI_GameContent :UI_BaseContent
{
    
}