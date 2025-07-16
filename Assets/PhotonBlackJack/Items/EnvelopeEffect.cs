using UnityEngine;

/// <summary>
/// 다음 히트 카드를 상대방에게 보이지 않게 하는 '편지봉투' 아이템 효과입니다.
/// </summary>
public class EnvelopeEffect : IItemEffect
{
    public void Execute(PhotonBlackJackLogic gameLogic, BlackJackPlayer user)
    {
        gameLogic.IsNextCardHidden = true; // 다음 히트 카드를 숨김
        Debug.Log($"Player {user.NickName} used Envelope item. Next hit card will be hidden.");
    }
}
