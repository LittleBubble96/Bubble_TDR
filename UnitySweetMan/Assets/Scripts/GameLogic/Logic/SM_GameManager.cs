using System;
using System.Collections;
using System.Collections.Generic;
using BubbleFramework.Bubble_UI;
using UnityEngine;

public class SM_GameManager : Bubble_MonoSingle<SM_GameManager>
{
    /// <summary>
    /// 每帧得时间
    /// </summary>
    private float _dt;

    private EGameState _eGameState;



    /// <summary>
    /// 统一管理初始化
    /// </summary>
    private void Awake()
    {
        SM_SceneManager.Instance.Init();
        
        _dt = Time.deltaTime;
        GameState = EGameState.GameMain;
    }

    /// <summary>
    /// 统一管理Update
    /// </summary>
    private void Update()
    {
        if (Instance.GameState==EGameState.Playing)
        {
            SM_SceneManager.Instance.DoUpdate(_dt);
        }
    }
    
    /// <summary>
    /// 管理游戏状态
    /// </summary>
    public EGameState GameState
    {
        get => _eGameState;
        set
        {
            if (_eGameState!=value)
            {
                _eGameState = value;
                switch (value)
                {
                    case EGameState.GameMain:
                        //关闭其他页面 显示主页面
                        BubbleFrameEntry.GetModel<UI_Manager>().HideView(UIType.Normal);
                        BubbleFrameEntry.GetModel<UI_Manager>().Show(UI_Name.UI_GameHomeView,new UI_GameHomeContent());
                        //创建关卡
                        SM_SceneManager.Instance.CreateLevel();
                        Cursor.visible = true;
                        break;
                    case EGameState.Playing:
                        //关闭其他页面 显示游戏页
                        BubbleFrameEntry.GetModel<UI_Manager>().HideView(UIType.Normal);
                        BubbleFrameEntry.GetModel<UI_Manager>().Show(UI_Name.UI_GameView,new UI_GameContent());
                        //设置鼠标不可见
                        Cursor.visible = false;
                        SM_SceneManager.Instance.CurLevelData.ELevelState = ELevelState.WaitPlay;

                        break;
                    case EGameState.Settling:
                        BubbleFrameEntry.GetModel<UI_Manager>().HideView(UIType.Normal);
                        BubbleFrameEntry.GetModel<UI_Manager>().Show(UI_Name.UI_GameSettleView,new UI_GameSettleContent(SM_SceneManager.Instance.CurLevelData.CurCharacter.Order));
                        
                        Cursor.visible = true;
                        break;
                }
            }
        }
    }
}
