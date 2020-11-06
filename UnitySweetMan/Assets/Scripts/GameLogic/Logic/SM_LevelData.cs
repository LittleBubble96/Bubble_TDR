using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_LevelData : MonoBehaviour
{
    private Transform _birthPoint;
    
    /// <summary>
    /// 当前人物
    /// </summary>
    [HideInInspector] 
    public CharacterControls CurCharacter;
    
    public void Init()
    {
        _birthPoint = transform.Find("BirthPoint");
        
        //创建人物
        CurCharacter = Instantiate(SM_SceneManager.Instance.CharacterControlses[0], transform);
        CurCharacter.Init();
        SetCharacterBirth();

    }

    public void DoUpdate(float dt)
    {
        
    }

    /// <summary>
    /// 设置人物出生
    /// </summary>
    public void SetCharacterBirth()
    {
        CurCharacter.transform.position = _birthPoint.position + Vector3.up * SM_SceneManager.Instance.BirthHeight;
    }
}
