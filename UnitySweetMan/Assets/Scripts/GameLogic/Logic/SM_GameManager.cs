using System;
using System.Collections;
using System.Collections.Generic;
using BubbleFramework.Bubble_Event;
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
        BubbleFrameEntry.Awake();
        SM_SceneManager.Instance.Init();
        SM_AudioManager.Instance.Init();
        
        _dt = Time.deltaTime;
        GameState = EGameState.GameMain;
    }

    /// <summary>
    /// 统一管理Update
    /// </summary>
    private void Update()
    {
        BubbleFrameEntry.Update();

        if (Instance.GameState==EGameState.Playing)
        {
            SM_SceneManager.Instance.DoUpdate(Time.deltaTime);
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
                        SM_AudioManager.Instance.PlayBGM(SM_SceneManager.Instance.uiAudio);
                        break;
                    case EGameState.Playing:
                        //关闭其他页面 显示游戏页
                        BubbleFrameEntry.GetModel<UI_Manager>().HideView(UIType.Normal);
                        BubbleFrameEntry.GetModel<UI_Manager>().Show(UI_Name.UI_GameView,new UI_GameContent());
                        SM_AudioManager.Instance.PlayBGM(SM_SceneManager.Instance.CurLevelData.levelBGM);
                        //设置鼠标不可见
                        Cursor.visible = false;
                        SM_SceneManager.Instance.CurLevelData.ELevelState = ELevelState.WaitPlay;

                        break;
                    case EGameState.Settling:
                        BubbleFrameEntry.GetModel<UI_Manager>().HideView(UIType.Normal);
                        BubbleFrameEntry.GetModel<UI_Manager>().Show(UI_Name.UI_GameSettleView,new UI_GameSettleContent(SM_SceneManager.Instance.CurLevelData.GetGameSettleString()));
                        
                        Cursor.visible = true;
                        break;
                }
            }
        }
    }
    
    /// <summary>
    /// 通关
    /// </summary>
    public void SuccessLevel()
    {
        SM_SceneManager.Instance.CrossLevelCount++;
        GameState = EGameState.Settling;
    }

    /// <summary>
    /// 失败
    /// </summary>
    public void FailedLevel()
    {
        GameState = EGameState.Settling;
    }

    /// <summary>
    /// 游戏继续
    /// </summary>
    public void GameContinue()
    {
        SM_SceneManager.Instance.CreateLevel();
        if (SM_SceneManager.Instance.CheckCrossAll())
        {
            GameState = EGameState.GameMain;
            SM_SceneManager.Instance.CrossLevelCount = 0;
        }
        else
        {
            GameState = EGameState.Playing;
        }
    }

    /// <summary>
    /// 重新开始
    /// </summary>
    public void GameRestart()
    {
        GameState = EGameState.Playing;
    }
    
    
}
