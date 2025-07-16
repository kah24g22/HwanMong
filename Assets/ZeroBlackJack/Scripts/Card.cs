using UnityEngine;

public enum Suit
{
    Spade,
    Heart,
    Diamond,
    Club
}

public enum Rank
{
    Ace = 1,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King
}

public class Card
{
    public Suit suit { get; private set; }
    public Rank rank { get; private set; }

    public Card(int suit, int rank)
    {
        this.suit = (Suit)suit;
        this.rank = (Rank)rank;
    }
}
