using UnityEngine;

public class Hands : MonoBehaviour
{
    private const int MAX_HANDS = 5;

    [SerializeField] private Deck deck;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject[] handsPosition = new GameObject[MAX_HANDS];

    public Card[] hands = new Card[MAX_HANDS];

    private GameObject[] cardObjects = new GameObject[MAX_HANDS];

    public void HitButton()
    {
        if (!IsHandsFull())
        {
            Hit();
        }
    }

    private void CardOnPlace(Card card, int index)
    {
        GameObject instance = Instantiate(cardPrefab, handsPosition[index].transform);

        CardSpriteRender render = instance.GetComponent<CardSpriteRender>();
        render.SetData(card);
        render.SetSprite();
    }

    private void Hit()
    {
        for (int i = 0; i < hands.Length; i++)
        {
            if (hands[i] == null)
            {
                hands[i] = deck.DrawCard();
                CardOnPlace(hands[i], i);
                break;
            }
        }
    }

    private bool IsHandsFull()
    {
        foreach (Card hand in hands)
        {
            if (hand == null)
            {
                return false;
            }
        }
        return true;
    }
}
