using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class MatchEffect : IItemEffect
{
    public void Execute(PhotonBlackJackLogic gameLogic, BlackJackPlayer user)
    {
        // 1. 아이템을 사용한 플레이어를 기준으로 상대방을 찾습니다.
        BlackJackPlayer opponent = (gameLogic.Player1 == user) ? gameLogic.Player2 : gameLogic.Player1;
        if (opponent == null) return;

        // 2. 상대방의 손패에서 뒤집힌(Face-down) 카드만 골라 리스트를 만듭니다.
        //    (※ 전제: Card 클래스에 IsFaceUp 이라는 bool 변수가 있어야 합니다.)
        List<Card> faceDownCards = opponent.Hand.Cards.Where(card => !card.IsFaceUp).ToList();

        // 3. 만약 뒤집힌 카드가 없다면 아이템 효과가 발동하지 않고 종료됩니다.
        if (faceDownCards.Count == 0)
        {
            Debug.Log("성냥 아이템을 사용했지만, 상대방의 패에 뒤집힌 카드가 없습니다.");
            return;
        }

        // 4. 뒤집힌 카드 중에서 무작위로 하나를 선택합니다.
        int randomIndex = Random.Range(0, faceDownCards.Count);
        Card cardToRemove = faceDownCards[randomIndex];

        // 5. 상대방의 손패에서 해당 카드를 제거합니다.
        //    (※ 전제: Hand 클래스에 RemoveCard 라는 메서드가 있어야 합니다.)
        opponent.Hand.RemoveCard(cardToRemove);
    }
}
