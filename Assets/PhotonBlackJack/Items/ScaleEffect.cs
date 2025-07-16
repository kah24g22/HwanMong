using UnityEngine;

/// <summary>
/// 현재 라이프를 1 증가시키는 '저울' 아이템 효과입니다.
/// 라이프는 최대 라이프를 넘어서 회복되지 않습니다.
/// </summary>
public class ScaleEffect : IItemEffect
{
    public void Execute(PhotonBlackJackLogic gameLogic, BlackJackPlayer user)
    {
        if (user.Life < user.MaxLife)
        {
            user.IncreaseLife(); // 라이프 증가
            Debug.Log($"Player {user.Player.NickName} used Scale item. Life increased by 1. Current life: {user.Life}/{user.MaxLife}");
        }
        else
        {
            Debug.Log($"Player {user.Player.NickName} used Scale item, but life is already at maximum ({user.MaxLife}).");
        }
    }
}
