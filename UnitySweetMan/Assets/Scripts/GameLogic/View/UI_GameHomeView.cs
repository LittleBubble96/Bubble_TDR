using System;
using System.Collections;
using System.Collections.Generic;
using BubbleFramework.Bubble_UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameHomeView : UI_Base<UI_GameHomeContent>
{
    [Bubble_Name("开始游戏按钮",Describe = "这个是开始游戏按钮")]
    public Button BeginGameBtn;

    [Bubble_Name("选择右边按钮")]
    public Button SelectRightBtn;
    
    [Bubble_Name("选择左边按钮")]
    public Button SelectLeftBtn;
    
    [Bubble_Name("初始位置")] 
    public Transform startPoint;
    
    [Bubble_Name("人物父物体")] 
    public Transform parentPoint;

    [Bubble_Name("过度时间")]
    public float ChangeTime = 1f;
    
    /// <summary>
    /// 人物间间隔距离
    /// </summary>
    private float duration = 3.4f;

    /// <summary>
    /// 临时时间
    /// </summary>
    private float tempChangeTime;

    /// <summary>
    /// 初始位置
    /// </summary>
    private Vector3 startPos;

    /// <summary>
    /// 结束位置
    /// </summary>
    private Vector3 endPos;

    private List<GameObject> uiCharacters = new List<GameObject>();
    public override void Init()
    {
        base.Init();
        UiType = UIType.Normal;
        
        
        //点击开始游戏
        BeginGameBtn.onClick.AddListener(() =>
        {
            SM_GameManager.Instance.GameState = EGameState.Playing;
        });
        
        SelectRightBtn.onClick.AddListener(() =>
        {
            if (SM_SceneManager.Instance.SelectCharacterIndex<SM_SceneManager.Instance.CharacterUIs.Count-1)
            {
                SM_SceneManager.Instance.SelectCharacterIndex++;
                CheckCharacterIndex();
                SetChangeTime(SM_SceneManager.Instance.SelectCharacterIndex);
            }
        });

        SelectLeftBtn.onClick.AddListener(() =>
        {
            if (SM_SceneManager.Instance.SelectCharacterIndex>0)
            {
                SM_SceneManager.Instance.SelectCharacterIndex--;
                CheckCharacterIndex();
                SetChangeTime(SM_SceneManager.Instance.SelectCharacterIndex);
            }
        });
        //实例化人物
        int i = 0;
        foreach (var uiPrefab in SM_SceneManager.Instance.CharacterUIs)
        {
            GameObject character = Instantiate(uiPrefab, parentPoint);
            character.transform.localPosition = startPoint.localPosition + Vector3.right * (duration * i);
            uiCharacters.Add(character);
            i++;
        }
    }

    public override void DoUpdate(float dt)
    {
        base.DoUpdate(dt);
        if (tempChangeTime>0)
        {
            tempChangeTime -= dt;
            var t = 1 - tempChangeTime / ChangeTime;
            parentPoint.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
        }
    }

    public override void SetContent(UI_BaseContent content)
    {
        base.SetContent(content);
        CheckCharacterIndex();
    }

    /// <summary>
    /// 设置该变量
    /// </summary>
    /// <param name="index"></param>
    public void SetChangeTime(int index)
    {
        startPos = parentPoint.transform.localPosition;
        endPos = new Vector3(-1 * index * duration, 0, startPos.z);
        tempChangeTime = ChangeTime;
    }

    private void CheckCharacterIndex()
    {
        SelectLeftBtn.gameObject.SetActive(SM_SceneManager.Instance.SelectCharacterIndex != 0);
        SelectRightBtn.gameObject.SetActive(SM_SceneManager.Instance.SelectCharacterIndex!=SM_SceneManager.Instance.CharacterUIs.Count-1);
    }
}
public class UI_GameHomeContent :UI_BaseContent
{
    
}

