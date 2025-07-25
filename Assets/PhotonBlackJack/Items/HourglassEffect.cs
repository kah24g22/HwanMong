using UnityEngine;

/// <summary>
/// 즉시 새 라운드를 시작하는 '모래시계' 아이템 효과입니다.
/// </summary>
public class HourglassEffect : IItemEffect
{
    public void Execute(SoloBlackJackLogic gameLogic, BlackJackPlayer user)
    {
        Debug.Log($"Player {user.name} used Hourglass item. Starting new round immediately.");
        gameLogic.StartGame(); // 새 라운드 시작
    }
}
