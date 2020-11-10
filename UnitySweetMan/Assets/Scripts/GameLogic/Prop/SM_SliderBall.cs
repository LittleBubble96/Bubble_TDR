using System.Collections;
using System.Collections.Generic;
using BubbleFramework;
using UnityEngine;

public class SM_SliderBall : PropBase
{
    [Bubble_Name("吐球间隔")] 
    public float OutBallTime = 3f;

    [Bubble_Name("最大尺寸")] 
    public float MaxScale = 6;
    
    /// <summary>
    /// 球得预制
    /// </summary>
    private GameObject ballPrefab;

    /// <summary>
    /// 缓存球得预制
    /// </summary>
    private Queue<GameObject> ballQueue = new Queue<GameObject>();

    /// <summary>
    /// 临时时间
    /// </summary>
    private float tempoutBallTime;

    /// <summary>
    /// 当前正在吐得球
    /// </summary>
    private GameObject curOuttingBall;

    /// <summary>
    /// 当前正在吐得球的刚体
    /// </summary>
    private Rigidbody curOuttingBallRigi;
    /// <summary>
    /// 球得集合
    /// </summary>
    private List<GameObject> balls = new List<GameObject>();
    
    void Awake()
    {
        ballPrefab = transform.Find("Ball").gameObject;
        curOuttingBall = OutNewBall();
        tempoutBallTime = OutBallTime;
    }

    void Update()
    {
        if (tempoutBallTime > 0)
        {
            //变大
            tempoutBallTime -= Time.deltaTime;
            var t = 1 - tempoutBallTime / OutBallTime;
            //计算球得变大
            curOuttingBall.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * MaxScale, t);
            //计算球得位移
            curOuttingBall.transform.position = ballPrefab.transform.position - Vector3.up * (Mathf.Lerp(0, MaxScale, t) * 0.5f);
            if (tempoutBallTime<=0)
            {
                //吐球
                int dir = Utility.Random.GetRandom(-1, 1);
                float x = dir * (float) Utility.Random.GetRandomDouble();
                float y = (-1) * (float) Utility.Random.GetRandomDouble();
                float z = (-1) * (float) Utility.Random.GetRandomDouble();
                Vector3 randomDir = new Vector3(x, y, z);
                curOuttingBallRigi.isKinematic = false;
                curOuttingBallRigi.velocity = randomDir * 10f;
                curOuttingBall = OutNewBall();
                tempoutBallTime = OutBallTime;
            }
        }

        //检测球是否销毁进入缓存
        for (int i = balls.Count-1; i >=0; i--)
        {
            if (balls[i].transform.position.y<SM_SceneManager.Instance.CurLevelData._deathPoint.transform.position.y)
            {
                balls[i].SetActive(false);
                ballQueue.Enqueue(balls[i]);
                balls.RemoveAt(i);
            }
        }
        
    }

    /// <summary>
    /// 获得一个新得球
    /// </summary>
    /// <returns></returns>
    GameObject OutNewBall()
    {
        GameObject ballObj = ballQueue.Count > 0 ? ballQueue.Dequeue() : Instantiate(ballPrefab,transform);
        ballObj.transform.position = ballPrefab.transform.position;
        ballObj.transform.localScale =Vector3.zero;
        ballObj.SetActive(true);
        curOuttingBall = ballObj;
        balls.Add(ballObj);
        curOuttingBallRigi = curOuttingBall.GetComponent<Rigidbody>();
        curOuttingBallRigi.isKinematic = true;
        return ballObj;
    }
}
