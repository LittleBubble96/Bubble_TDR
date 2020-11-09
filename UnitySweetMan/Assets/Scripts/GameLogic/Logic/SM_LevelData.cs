using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BubbleFramework;
using BubbleFramework.Bubble_Event;
using UnityEditor;
using UnityEngine;

public class SM_LevelData : MonoBehaviour
{
    [Bubble_Name("AI数量")] 
    public int AiCount;


    [Bubble_Name("等待开始时间")] 
    public float WaitBeginTime = 3f;
    
    [Bubble_Name("等待结算时间")]
    public float WaitTime = 3f;

    
    private ELevelState _eLevelState;
    
    [HideInInspector]
    public Transform _birthPoint;
    [HideInInspector]
    public Transform _deathPoint;
    private float _waitTime;
    private float _waitBeginTime;

    /// <summary>
    /// 当前人物
    /// </summary>
    [HideInInspector] 
    public CharacterControls CurCharacter;

    /// <summary>
    /// AI集合
    /// </summary>
    [HideInInspector] 
    public List<SM_CharacterAIBase> CharacterAIs;

    /// <summary>
    /// 障碍集合
    /// </summary>
    [HideInInspector] 
    public List<PropBase> Props = new List<PropBase>();
    
    [Bubble_Name("目标")] 
    public Transform Target;
   
    public virtual void Init()
    {
        _birthPoint = transform.Find("BirthPoint");
        _deathPoint= transform.Find("DeathPoint");
        
        Props = GetComponentsInChildren<PropBase>().ToList();
            
       

        CreateAIList();
    }

    public virtual void DoUpdate(float dt)
    {
        if (_waitBeginTime>0)
        {
            _waitBeginTime -= dt;
            if (_waitBeginTime<=0)
            {
                ELevelState = ELevelState.Playing;
            }
        }
        //等待结算
        if (_waitTime > 0)
        {
            _waitTime -= dt;
            if (_waitTime<=0)
            {
                ELevelState = ELevelState.Settle;
            }
        }
       
       
    }

    /// <summary>
    /// 设置人物出生
    /// </summary>
    public void SetCharacterBirth()
    {
        CurCharacter.transform.position = _birthPoint.position + Vector3.up * SM_SceneManager.Instance.BirthHeight;
    }

    /// <summary>
    /// 创建Ai集合
    /// </summary>
    public void CreateAIList()
    {
        for (int i = 0; i < AiCount; i++)
        {
            int randomIndex = Utility.Random.GetRandom(SM_SceneManager.Instance.CharacterAIBases.Count - 1);
            var ai = Instantiate(SM_SceneManager.Instance.CharacterAIBases[randomIndex],transform);
            ai.Init();
            ai.transform.position = _birthPoint.position + Vector3.right * 2;
            ai.Target = Target;
            CharacterAIs.Add(ai);
        }
    }

    /// <summary>
    /// 获取游戏内文本
    /// </summary>
    /// <returns></returns>
    public virtual string GetGameString()
    {
        return "";
    }

    /// <summary>
    /// 获取结算页文本
    /// </summary>
    /// <returns></returns>
    public virtual string GetGameSettleString()
    {
        return "";
    }

    /// <summary>
    /// 关卡状态
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public ELevelState ELevelState
    {
        get => _eLevelState;
        set
        {
            if (_eLevelState!=value)
            {
                _eLevelState = value;
                switch (value)
                {
                    case ELevelState.None:
                        break;
                    case ELevelState.WaitPlay:
                        //创建人物
                        CurCharacter = Instantiate(SM_SceneManager.Instance.CharacterControlses[SM_SceneManager.Instance.SelectCharacterIndex], transform);
                        CurCharacter.Init();
                        CurCharacter.transform.position = _birthPoint.position;
                        SM_SceneManager.Instance.CurCamera.SetTarget(CurCharacter.transform);

                        _waitBeginTime = WaitBeginTime;
                        BubbleFrameEntry.GetModel<AppEventDispatcher>().BroadcastListener(EventName.EVENT_COUNTDOWN,3);
                        BubbleFrameEntry.GetModel<AppEventDispatcher>().BroadcastListener(EventName.EVENT_CHANGECROSSCOUNT);
                        break;
                    case ELevelState.Playing:
                        break;
                    case ELevelState.WaitSettle:
                        _waitTime = WaitTime;
                        break;
                    case ELevelState.Settle:
                        if (CurCharacter.characterAnimator.AnimatorState.Success)
                        {
                            SM_GameManager.Instance.SuccessLevel();
                        }
                        else
                        {
                            SM_GameManager.Instance.FailedLevel();
                        }

                        SM_GameManager.Instance.GameState = EGameState.Settling;
                        break;
                }
            }
        }
    }
}
