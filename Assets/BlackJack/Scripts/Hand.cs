using System.Collections.Generic;

public class Hand
{
    public List<Card> Cards { get; private set; }

    public Hand()
    {
        Cards = new List<Card>();
    }

    public void AddCard(Card card)
    {
        Cards.Add(card);
    }

    public void Clear()
    {
        Cards.Clear();
    }

    // 특정 카드를 손패에서 제거하는 메서드 추가
    public void RemoveCard(Card card)
    {
        if (Cards.Contains(card))
        {
            Cards.Remove(card);
        }
    }

    public int CalculateValue()
    {
        int value = 0;
        int aceCount = 0;
        foreach (Card card in Cards)
        {
            value += card.value;
            if (card.rank == "A")
            {
                aceCount++;
            }
        }

        while (value > 21 && aceCount > 0)
        {
            value -= 10;
            aceCount--;
        }
        return value;
    }

    public bool IsBlackJack()
    {
        if(CalculateValue() == 21) return true;
        else return false;
    }
    public bool IsBust()
    {
        if(CalculateValue() > 21) return true;
        else return false;
    }
}
