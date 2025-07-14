using UnityEngine;
using UnityEngine.UI; // UI 요소를 사용하기 위해 추가
using System.Collections.Generic;
using TMPro; // TextMeshPro를 사용하기 위해 추가

public class GameManager : MonoBehaviour
{
    public Deck deck;
    public List<Card> playerHand;
    public List<Card> dealerHand;

    // UI 요소 연결
    [SerializeField] private TextMeshProUGUI playerScoreText;
    [SerializeField] private TextMeshProUGUI dealerScoreText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button hitButton;
    [SerializeField] private Button standButton;
    [SerializeField] private Button restartButton;

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        deck = new Deck();
        deck.CreateDeck();
        deck.Shuffle();

        playerHand = new List<Card>();
        dealerHand = new List<Card>();

        // 초기 카드 2장씩 분배
        playerHand.Add(deck.Deal());
        dealerHand.Add(deck.Deal());
        playerHand.Add(deck.Deal());
        dealerHand.Add(deck.Deal());

        UpdateScores();
        resultText.text = "";
        hitButton.interactable = true;
        standButton.interactable = true;
        restartButton.gameObject.SetActive(false);

        // TODO: 화면에 카드 오브젝트를 생성하고 표시하는 로직 추가
    }

    public void Hit()
    {
        playerHand.Add(deck.Deal());
        UpdateScores();

        if (CalculateHandValue(playerHand) > 21)
        {
            EndGame("Player Busts! Dealer Wins.");
        }
    }

    public void Stand()
    {
        hitButton.interactable = false;
        standButton.interactable = false;

        // 딜러 턴
        while (CalculateHandValue(dealerHand) < 17)
        {
            dealerHand.Add(deck.Deal());
        }
        UpdateScores();
        DetermineWinner();
    }

    void DetermineWinner()
    {
        int playerScore = CalculateHandValue(playerHand);
        int dealerScore = CalculateHandValue(dealerHand);

        if (dealerScore > 21 || playerScore > dealerScore)
        {
            EndGame("Player Wins!");
        }
        else if (playerScore < dealerScore)
        {
            EndGame("Dealer Wins!");
        }
        else
        {
            EndGame("Push! (Draw)");
        }
    }

    void EndGame(string message)
    {
        resultText.text = message;
        hitButton.interactable = false;
        standButton.interactable = false;
        restartButton.gameObject.SetActive(true);
    }

    void UpdateScores()
    {
        playerScoreText.text = "Player: " + CalculateHandValue(playerHand);
        dealerScoreText.text = "Dealer: " + CalculateHandValue(dealerHand);
    }

    int CalculateHandValue(List<Card> hand)
    {
        int value = 0;
        int aceCount = 0;
        foreach (Card card in hand)
        {
            value += card.value;
            if (card.rank == "A")
            {
                aceCount++;
            }
        }

        // Ace 처리 (11 또는 1)
        while (value > 21 && aceCount > 0)
        {
            value -= 10;
            aceCount--;
        }
        return value;
    }
}
