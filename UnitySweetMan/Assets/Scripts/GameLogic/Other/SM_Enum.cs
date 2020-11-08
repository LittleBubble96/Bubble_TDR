
// enum 类
public enum EGameState
{
    None,
    GameMain,//初始化场景 人物
    Playing,//正在玩
    Settling,//结算
}

public enum ELevelState
{
    None,
    WaitPlay,//等待开始
    Playing,//正在玩
    WaitSettle,//等待结算
    Settle//结算
}
