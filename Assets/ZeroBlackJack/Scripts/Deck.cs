using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<Card> cards;

    private const int MAX_SUIT = 4;
    private const int MAX_RANK = 13;

    void Awake()
    {
        cards = DeckInit();
        Suffle();
    }

    public Card DrawCard()
    {
        Card card = cards.Last();
        cards.RemoveAt(cards.Count - 1);
        return card;
    }

    private List<Card> DeckInit()
    {
        List<Card> cards = new List<Card>();

        for (int i = 0; i < MAX_SUIT; i++)
        {
            for (int j = 0; j < MAX_RANK; j++)
            {
                cards.Add(new Card(i, j + 1));
            }
        }

        return cards;
    }

    private void Suffle()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int rnd = Random.Range(0, cards.Count);
            Card temp = cards[i];
            cards[i] = cards[rnd];
            cards[rnd] = temp;
        }
    }
}
