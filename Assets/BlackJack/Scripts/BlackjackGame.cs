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

        // �ʱ� ī�� �й�
        DealCard(Player1Hand, true); // �÷��̾�1 ù ī��
        DealCard(Player2Hand, true); // �÷��̾�2 ù ī��
        DealCard(Player1Hand, true); // �÷��̾�1 �� ��° ī��
        DealCard(Player2Hand, true); // �÷��̾�2 �� ��° ī��

        CurrentPlayer = PlayerTurn.Player1; // �÷��̾�1���� ����
        OnTurnChanged?.Invoke(CurrentPlayer); // �� ���� �̺�Ʈ ȣ��
        CheckForBlackjack(); // �ʱ� ���� Ȯ�� (�� �÷��̾� ���)
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

            // ����Ʈ �߻� �� ���� ���� ���� Ȯ��
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
            // 21���� �ƴϰ� ����Ʈ�� �ƴϸ� �ϸ� �ѱ�ϴ�.
            SwitchTurnOrEndGame();
        }
    }

    public void Stand()
    {
        // ���ĵ� �� ���� ���� �޽��� ����. �ϸ� �ѱ�ϴ�.
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
            // ���� ���� �� ���� ������ �Ѿ�� ����
        }
        else if (player2Blackjack)
        {
            EndGame("Player2 has Blackjack! Player2 Wins.");
            // ���� ���� �� ���� ������ �Ѿ�� ����
        }
    }

    private void SwitchTurnOrEndGame()
    {
        if (CurrentPlayer == PlayerTurn.Player1)
        {
            CurrentPlayer = PlayerTurn.Player2;
            OnTurnChanged?.Invoke(CurrentPlayer); // �� ���� �̺�Ʈ ȣ��
        }
        else // CurrentPlayer == PlayerTurn.Player2
        {
            CurrentPlayer = PlayerTurn.Player1;
        }
        OnTurnChanged?.Invoke(CurrentPlayer); // �� ���� �̺�Ʈ ȣ��
    }

    private void EndGame(string message)
    {
        OnGameEnded?.Invoke(message);
    }
}
