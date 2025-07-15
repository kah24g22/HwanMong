using System.Collections.Generic;

public class BlackjackGame
{
    private Deck deck;
    public Hand Player1Hand { get; private set; }
    public Hand Player2Hand { get; private set; }

    public enum PlayerTurn { Player1, Player2 }
    public PlayerTurn CurrentPlayer { get; private set; }
    public bool isPlayer1Busted { get; private set; }
    public bool isPlayer2Busted { get; private set; }

    public delegate void CardDealtEventHandler(Card card, Hand hand, bool isFaceUp);
    public event CardDealtEventHandler OnCardDealt;

    public delegate void GameStateChangedEventHandler(string message);
    public event GameStateChangedEventHandler OnGameEnded;

    public delegate void TurnChangedEventHandler(PlayerTurn currentPlayer);
    public event TurnChangedEventHandler OnTurnChanged;

    public BlackjackGame()
    {
        deck = new Deck();
        Player1Hand = new Hand();
        Player2Hand = new Hand();
    }

    public void StartNewGame()
    {
        deck.CreateDeck();
        deck.Shuffle();

        Player1Hand.Clear();
        Player2Hand.Clear();

        isPlayer1Busted = false;
        isPlayer2Busted = false;

        // 초기 카드 분배
        DealCard(Player1Hand, true); // 플레이어1 첫 카드
        DealCard(Player2Hand, true); // 플레이어2 첫 카드
        DealCard(Player1Hand, true); // 플레이어1 두 번째 카드
        DealCard(Player2Hand, true); // 플레이어2 두 번째 카드

        CurrentPlayer = PlayerTurn.Player1; // 플레이어1부터 시작
        OnTurnChanged?.Invoke(CurrentPlayer); // 턴 변경 이벤트 호출
        CheckForBlackjack(); // 초기 블랙잭 확인 (두 플레이어 모두)
    }


    private void DealCard(Hand hand, bool isFaceUp)
    {
        Card newCard = deck.Deal();
        if (newCard != null)
        {
            hand.AddCard(newCard);
            OnCardDealt?.Invoke(newCard, hand, isFaceUp);
        }
    }

    public void Hit()
    {
        Hand currentHand = (CurrentPlayer == PlayerTurn.Player1) ? Player1Hand : Player2Hand;
        DealCard(currentHand, true);

        if (currentHand.CalculateValue() == 21)
        {
            EndGame($"{CurrentPlayer} has 21! {CurrentPlayer} Wins.");
        }
        else if (currentHand.CalculateValue() > 21)
        {
            if (CurrentPlayer == PlayerTurn.Player1)
            {
                isPlayer1Busted = true;
            }
            else
            {
                isPlayer2Busted = true;
            }

            // 버스트 발생 시 게임 종료 조건 확인
            if (isPlayer1Busted && isPlayer2Busted)
            {
                EndGame("Both players busted! It's a Push.");
            }
            else if (isPlayer1Busted)
            {
                EndGame("Player1 busted! Player2 Wins.");
            }
            else // isPlayer2Busted
            {
                EndGame("Player2 busted! Player1 Wins.");
            }
        }
        else
        {
            // 21점도 아니고 버스트도 아니면 턴만 넘깁니다.
            SwitchTurnOrEndGame();
        }
    }

    public void Stand()
    {
        // 스탠드 시 게임 종료 메시지 제거. 턴만 넘깁니다.
        SwitchTurnOrEndGame();
    }

    private void CheckForBlackjack()
    {
        bool player1Blackjack = Player1Hand.Cards.Count == 2 && Player1Hand.CalculateValue() == 21;
        bool player2Blackjack = Player2Hand.Cards.Count == 2 && Player2Hand.CalculateValue() == 21;

        if (player1Blackjack && player2Blackjack)
        {
            EndGame("Both players have Blackjack! It's a Push.");
        }
        else if (player1Blackjack)
        {
            EndGame("Player1 has Blackjack! Player1 Wins.");
            // 게임 종료 후 다음 턴으로 넘어가지 않음
        }
        else if (player2Blackjack)
        {
            EndGame("Player2 has Blackjack! Player2 Wins.");
            // 게임 종료 후 다음 턴으로 넘어가지 않음
        }
    }

    private void SwitchTurnOrEndGame()
    {
        if (CurrentPlayer == PlayerTurn.Player1)
        {
            CurrentPlayer = PlayerTurn.Player2;
            OnTurnChanged?.Invoke(CurrentPlayer); // 턴 변경 이벤트 호출
        }
        else // CurrentPlayer == PlayerTurn.Player2
        {
            CurrentPlayer = PlayerTurn.Player1;
        }
        OnTurnChanged?.Invoke(CurrentPlayer); // 턴 변경 이벤트 호출
    }

    private void EndGame(string message)
    {
        OnGameEnded?.Invoke(message);
    }
}
