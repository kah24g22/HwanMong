using System.Collections;
using System.Collections.Generic;

public class Card
{
    public string suit; // 모양 (Spade, Heart, Diamond, Club)
    public string rank; // 숫자 (A, 2, 3, ..., K)
    public int value;  // 실제 값 (A=11 또는 1, J,Q,K=10)
    public bool IsFaceUp { get; set; } // 카드가 앞면인지 뒷면인지 상태 저장

    public Card(string suit, string rank)
    {
        this.suit = suit;
        this.rank = rank;

        if (rank == "A") this.value = 11;
        else if (rank == "J" || rank == "Q" || rank == "K") this.value = 10;
        else this.value = int.Parse(rank);

        this.IsFaceUp = true; // 기본적으로 카드는 앞면으로 생성 알아서 지지고 볶아봐라 
    }
}
