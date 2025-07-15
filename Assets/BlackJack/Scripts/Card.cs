using System.Collections;
using System.Collections.Generic;

public class Card
{
    public string suit; // 모양 (Spade, Heart, Diamond, Club)
    public string rank; // 숫자 (A, 2, 3, ..., K)
    public int value;  // 실제 값 (A=11 또는 1, J,Q,K=10)

    public Card(string suit, string rank, int value)
    {
        this.suit = suit;
        this.rank = rank;
        this.value = value;
    }
}
