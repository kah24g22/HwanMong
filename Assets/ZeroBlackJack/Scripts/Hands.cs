using System;
using TMPro;
using UnityEngine;

public class Hands : MonoBehaviour
{
    private const int MAX_HANDS = 5;
    private const int BLACKJACK = 21;

    [SerializeField] private Deck deck;
    [SerializeField] private TurnChecker turnChecker;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject[] handsPosition = new GameObject[MAX_HANDS];
    [SerializeField] private TMP_Text valueText;

    public Card[] hands = new Card[MAX_HANDS];
    public bool isBlackjack;
    public bool isBust;

    private GameObject[] cardObjects = new GameObject[MAX_HANDS];

    void Start()
    {
        HandsInit();
    }

    public void ClearHandsButton()
    {
        ClearHands();
    }

    public void KillCard(GameObject hand)
    {
        hands[Array.IndexOf(handsPosition, hand)] = null;
        foreach (Transform child in hand.transform)
        {
            Destroy(child.gameObject);
        }

        ScoreUpdate();
    }

    public void HandsInit()
    {
        isBlackjack = false;
        isBust = false;
    }

    public void ScoreUpdate()
    {
        int currentScore = CalculateHandsValue();

        valueText.text = CalculateHandsValue().ToString();
    }

    public void HitButton()
    {
        if (!IsHandsFull())
        {
            Hit();
        }
    }

    public void Hit()
    {
        for (int i = 0; i < hands.Length; i++)
        {
            if (hands[i] == null)
            {
                hands[i] = deck.DrawCard();
                CardOnPlace(hands[i], i);
                ScoreUpdate();
                ScoreCheck();
                break;
            }
        }
    }

    private void ClearHands()
    {
        foreach (GameObject hand in handsPosition)
        {
            KillCard(hand);
        }
    }

    private void ScoreCheck()
    {
        int score = CalculateHandsValue();

        if (score == BLACKJACK)
        {
            isBlackjack = true;
}
        else if (score >= BLACKJACK)
        {
            isBust = true;
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

    private int CalculateHandsValue()
    {
        int value = 0;

        foreach (Card hand in hands)
        {
            if (hand == null) value += 0;
            else
            {
                int rank = (int)hand.rank;
                if (rank > 10)
                {
                    value += 10;
                }
                else if (rank == 1)
                {
                    value += (value + rank) > BLACKJACK ? 1 : 11;
                }
                else
                {
                    value += rank;
                }
            }
        }

        return value;
    }

    private void CardOnPlace(Card card, int index)
    {
        GameObject instance = Instantiate(cardPrefab, handsPosition[index].transform);

        CardSpriteRender render = instance.GetComponent<CardSpriteRender>();
        render.SetData(card);
        render.SetSprite();
    }
}
