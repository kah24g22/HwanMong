using UnityEngine;

/// <summary>
/// '수정 구슬' 아이템의 효과를 구현한 클래스입니다.
/// 덱의 다음 카드를 미리 보여줍니다.
/// </summary>
public class CrystalBallEffect : IItemEffect
{
    public void Execute(SoloBlackJackLogic gameLogic, BlackJackPlayer user)
    {
        Card nextCard = gameLogic.Deck.Peek(); // Deck의 Peek() 메서드 사용

        if (nextCard != null)
        {
            Debug.Log($"{user.name}님이 수정 구슬을 사용하여 다음 카드를 미리 보았습니다: {nextCard.suit} {nextCard.rank}");
            // (추가) 여기에 사용자에게만 다음 카드를 보여주는 UI 로직을 연결해야 합니다.
            // 이 정보는 다른 플레이어에게는 보여지지 않아야 합니다.
        }
        else
        {
            Debug.Log("수정 구슬을 사용했지만 덱에 카드가 없습니다.");
        }

        // 이후 이 변경사항을 모든 클라이언트에게 동기화하는 네트워크 로직(RPC 호출 등)이 필요합니다.
        // 단, 이 아이템의 경우 '미리보기' 정보는 사용한 플레이어에게만 전달되어야 합니다.
    }
}
