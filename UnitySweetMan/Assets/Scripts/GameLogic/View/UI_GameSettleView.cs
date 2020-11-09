using System.Collections;
using System.Collections.Generic;
using BubbleFramework.Bubble_UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameSettleView : UI_Base<UI_GameSettleContent>
{
    [Bubble_Name("名次",Describe = "这个是名次")]
    public Text OrderText;
    
    [Bubble_Name("下一关")]
    public Button NextBtn;
    public override void Init()
    {
        base.Init();
        UiType = UIType.Normal;
        NextBtn.onClick.AddListener(() =>
        {
            SM_GameManager.Instance.GameState = EGameState.GameMain;
        });
       
    }


    public override void SetContent(UI_BaseContent content)
    {
        base.SetContent(content);
        //通关人数
        OrderText.text = UiBaseContent.SettleString + "";
    }
}
public class UI_GameSettleContent :UI_BaseContent
{
    public string SettleString;

    public UI_GameSettleContent(string settleString)
    {
        SettleString = settleString;
    }
}
