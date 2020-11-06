using System.Collections;
using System.Collections.Generic;
using BubbleFramework.Bubble_UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameHomeView : UI_Base<UI_GameHomeContent>
{
    [Bubble_Name("开始游戏按钮",Describe = "这个是开始游戏按钮")]
    public Button BeginGameBtn;
    
    public override void Init()
    {
        base.Init();
        UiType = UIType.Normal;
        
        //点击开始游戏
        BeginGameBtn.onClick.AddListener(() =>
        {
            SM_GameManager.Instance.GameState = EGameState.Playing;
        });
    }
    

    public override void SetContent(UI_BaseContent content)
    {
        base.SetContent(content);
    }
    
}
public class UI_GameHomeContent :UI_BaseContent
{
    
}

