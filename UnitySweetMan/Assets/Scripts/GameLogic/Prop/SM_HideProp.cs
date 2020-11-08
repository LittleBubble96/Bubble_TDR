using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SM_HideProp : MonoBehaviour
{
    [Bubble_Name("消失时间")]
    public float hideTime;
    private float _tempHideTime;

    private bool IsVisable;
    private void Update()
    {
        if (_tempHideTime>0)
        {
            _tempHideTime -= Time.deltaTime;
            if (_tempHideTime<=0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void SetHide()
    {
        if (SM_SceneManager.Instance.CurLevelData.ELevelState!=ELevelState.Playing&&!IsVisable)
        {
            return;
        }

        _tempHideTime = hideTime;
        IsVisable = true;
    }
}
