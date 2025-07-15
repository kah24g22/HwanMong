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
}
