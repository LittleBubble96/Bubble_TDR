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

    [Bubble_Name("文字")] 
    public Text BtnText;

    public override void Init()
    {
        base.Init();
        UiType = UIType.Normal;
        
        NextBtn.onClick.AddListener(() =>
        {
            if (SM_SceneManager.Instance.CurLevelData.CurCharacter.characterAnimator.AnimatorState.Success)
            {
                SM_GameManager.Instance.GameContinue();
            }
            else
            {
                SM_GameManager.Instance.GameRestart();
            }
        });
       
    }


    public override void SetContent(UI_BaseContent content)
    {
        base.SetContent(content);
        //通关人数
        if (SM_SceneManager.Instance.CurLevelData.CurCharacter.characterAnimator.AnimatorState.Success)
        {
            BtnText.text = SM_SceneManager.Instance.CheckCrossAll() ? "重新挑战" : "下一关";
        }
        else
        {
            BtnText.text = "重新开始";
        }
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
