using System.Collections;
using System.Collections.Generic;
using BubbleFramework;
using UnityEngine;

public class SM_SceneManager : Bubble_MonoSingle<SM_SceneManager>
{
    [Bubble_Name("关卡")]
    public List<SM_LevelData> LevelDatas = new List<SM_LevelData>();

    /// <summary>
    /// 当前关卡
    /// </summary>
    [HideInInspector]
    public SM_LevelData CurLevelData;
    public void Init()
    {
        
    }

    public void DoUpdate(float dt)
    {
        
    }

    /// <summary>
    /// 创建新得关卡
    /// </summary>
    /// <exception cref="GameException"></exception>
    public void CreateLevel()
    {
        ClearLevel();
        if (LevelDatas.Count <= 0)
        {
            throw new GameException("关卡数为0");
            return;
        }
        //随机下标
        int r = Utility.Random.GetRandom(LevelDatas.Count - 1);
        r %= LevelDatas.Count;
        //创建新关卡
        SM_LevelData level = LevelDatas[r];
        CurLevelData = Instantiate(level);
        CurLevelData.Init();
    }

    /// <summary>
    /// 清除当前关卡
    /// </summary>
    public void ClearLevel()
    {
        if (CurLevelData!=null)
        {
            DestroyImmediate(CurLevelData);
        }
    }
}
