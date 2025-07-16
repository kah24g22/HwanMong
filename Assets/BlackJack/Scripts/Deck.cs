using System.Collections;
using System.Collections.Generic;
using System.Linq; // Shuffle을 위해 추가

public class Deck
{
    public List<Card> cards;

    public void CreateDeck()
    {
        cards = new List<Card>();
        string[] suits = { "Spade", "Heart", "Diamond", "Club" };
        string[] ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

        foreach (string suit in suits)
        {
            foreach (string rank in ranks)
            {
                int value = 0;
                if (rank == "A") value = 11;
                else if (rank == "J" || rank == "Q" || rank == "K") value = 10;
                else value = int.Parse(rank);

                cards.Add(new Card(suit, rank));
            }
        }
    }

    public void Shuffle()
    {
        // 간단한 셔플 (Fisher-Yates 알고리즘)
        System.Random rng = new System.Random();
        cards = cards.OrderBy(c => rng.Next()).ToList();
    }

    public Card Deal()
    {
        if (cards.Count == 0) return null;

        Card cardToDeal = cards[0];
        cards.RemoveAt(0);
        return cardToDeal;
    }

    // 덱에서 카드를 제거하지 않고 맨 위의 카드를 확인하는 메서드
    public Card Peek()
    {
        if (cards.Count == 0) return null;
        return cards[0];
    }
}
