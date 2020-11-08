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

    [Bubble_Name("过关数")] 
    public int CrossLevelCount;
    
    [Bubble_Name("目标")] 
    public Transform Target;

    [Bubble_Name("等待结算时间")]
    public float WaitTime = 3f;

    private Transform EndTriggerRect;
    private ELevelState _eLevelState;
    
    private Transform _birthPoint;
    private Transform _deathPoint;
    private float _waitTime;
    private int crossLevelCount;

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
    
    /// <summary>
    /// 当前过关数
    /// </summary>
    public int CurCrossLevelCount
    {
        get => crossLevelCount;
        private set
        {
            if (crossLevelCount!=value)
            {
                crossLevelCount = value;
                BubbleFrameEntry.GetModel<AppEventDispatcher>().BroadcastListener(EventName.EVENT_CHANGECROSSCOUNT);
            }
        }
    }

    public void Init()
    {
        _birthPoint = transform.Find("BirthPoint");
        _deathPoint= transform.Find("DeathPoint");
        EndTriggerRect = transform.Find("EndTriggerRect");
        
        Props = GetComponentsInChildren<PropBase>().ToList();
            
        //创建人物
        CurCharacter = Instantiate(SM_SceneManager.Instance.CharacterControlses[0], transform);
        CurCharacter.Init();
        CurCharacter.transform.position = _birthPoint.position;

        CreateAIList();
        ELevelState = ELevelState.Playing;
    }

    public void DoUpdate(float dt)
    {
        //等待结算
        if (_waitTime > 0)
        {
            _waitTime -= dt;
            if (_waitTime<=0)
            {
                ELevelState = ELevelState.Settle;
            }
        }
        if (ELevelState!=ELevelState.Playing)
        {
            return;
        }
        //判断结束
        if (CurCrossLevelCount >= CrossLevelCount)
        {
            ELevelState = ELevelState.WaitSettle;
        }
        //主角是否过关
        if (!CurCharacter.characterAnimator.AnimatorState.Success && IsContainEndTrigger(CurCharacter.transform))
        {
            DDebug.Log("主角过关");
            CurCharacter.characterAnimator.AnimatorState.Success = true;
            CurCrossLevelCount++;
            CurCharacter.Order = CurCrossLevelCount;
        }

        //AI是否过关
        foreach (var ai in CharacterAIs)
        {
            if (!ai.CharacterAnimator.AnimatorState.Success && IsContainEndTrigger(ai.transform))
            {
                DDebug.Log("AI过关");
                ai.CharacterAnimator.AnimatorState.Success = true;
                CurCrossLevelCount++;
                ai.Order = CurCrossLevelCount;
            }
        }

        
        
        //人物死亡
        if (CurCharacter.transform.position.y<_deathPoint.transform.position.y)
        {
            SetCharacterBirth();
        }
        //AI行为
        foreach (var ai in CharacterAIs)
        {
            ai.DoUpdate(dt);
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
    /// 是否在重点
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public bool IsContainEndTrigger(Transform character)
    {
        var localScale = EndTriggerRect.localScale;
        float xLength = localScale.x;
        float yLength = localScale.y;
        float zLength = localScale.z;
        var endPosition = EndTriggerRect.position;
        var characterPosition = character.position;
        bool contain =
            characterPosition.x > endPosition.x - xLength * 0.5f &&
            characterPosition.x < endPosition.x + xLength * 0.5f &&
            characterPosition.y > endPosition.y - yLength * 0.5f &&
            characterPosition.y < endPosition.y + yLength * 0.5f &&
            characterPosition.z > endPosition.z - zLength * 0.5f &&
            characterPosition.z < endPosition.z + zLength * 0.5f;
        return contain;
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
                    case ELevelState.Playing:
                        break;
                    case ELevelState.WaitSettle:
                        _waitTime = WaitTime;
                        break;
                    case ELevelState.Settle:
                        SM_GameManager.Instance.GameState = EGameState.Settling;
                        break;
                }
            }
        }
    }
}
