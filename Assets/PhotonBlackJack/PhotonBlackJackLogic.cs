using Photon.Realtime;
using UnityEngine;

public enum PlayerTurn { Player1, Player2 }
public enum Result { Win, Lose, Draw, None, }

public class PhotonBlackJackLogic
{
    private BlackJackPlayer m_player1;
    private BlackJackPlayer m_player2;

    private Deck m_deck;

    private PlayerTurn m_turn;

    public BlackJackPlayer Player1 { get { return m_player1; } }
    public BlackJackPlayer Player2 { get { return m_player2; } }
    public PlayerTurn Turn { get { return m_turn;} set { m_turn = value; } }



    public delegate void CardDealtEventHandler(Player pPlayer, Card card, bool isFaceUp);
    public event CardDealtEventHandler OnCardDealt;

    public delegate void GameStateChangedEventHandler(string message);
    public event GameStateChangedEventHandler OnGameEnded;

    public delegate void TurnChangedEventHandler(PlayerTurn currentPlayer);
    public event TurnChangedEventHandler OnTurnChanged;

    public PhotonBlackJackLogic(BlackJackPlayer player1, BlackJackPlayer player2)
    {
        m_player1 = player1;
        m_player2 = player2;
        m_deck = new Deck();
    }

    
    public void StartGame(BlackJackPlayer pPlayer)
    {
        m_deck.CreateDeck();
        m_deck.Shuffle();

        m_player1.PlayerReset();
        m_player2.PlayerReset();

        m_turn = PlayerTurn.Player1;
        OnTurnChanged?.Invoke(m_turn);

        m_player1.BlackJackCheck();
        m_player2.BlackJackCheck();

        if (m_player1.IsBlackJack ||  m_player2.IsBlackJack) WinnerCheck();
    }

    public void DealCard(BlackJackPlayer pPlayer, bool isFaceUp)
    {
        Card newCard = m_deck.Deal();

        if (newCard != null)
        {
            pPlayer.Hand.AddCard(newCard);
            OnCardDealt?.Invoke(pPlayer.Player, newCard, isFaceUp);
        }
    }

    public void Hit()
    {
        BlackJackPlayer currentPlayer = (m_turn == PlayerTurn.Player1) ? m_player1 : m_player2;
        
        DealCard(currentPlayer, true);
        SwitchTurnOrEndGame();

        currentPlayer.BlackJackCheck();
        currentPlayer.BustCheck();

        if (currentPlayer.IsBlackJack || currentPlayer.IsBust)
        {
            WinnerCheck();
            return;
        }
    }

    public void Stand()
    {
        BlackJackPlayer currentPlayer = (m_turn == PlayerTurn.Player1) ? m_player1 : m_player2;
        currentPlayer.IsTurnOn = false;
        SwitchTurnOrEndGame();
    }

    private void SwitchTurnOrEndGame()
    {
        if (!m_player1.IsTurnOn && !m_player2.IsTurnOn)
        {
            WinnerCheck();
            return;
        }

        else if (!m_player1.IsTurnOn)
        {
            Debug.LogError("player1 IsTurnOn OFF");
            m_turn = PlayerTurn.Player2;
            OnTurnChanged?.Invoke(m_turn);
            return;
        }

        else if (!m_player2.IsTurnOn)
        {
            Debug.LogError("player2 IsTurnOn OFF");
            m_turn = PlayerTurn.Player1;
            OnTurnChanged?.Invoke(m_turn);
            return;
        }

        if (m_turn == PlayerTurn.Player1)
        {
            Debug.Log("player 1 -> player 2");
            m_turn = PlayerTurn.Player2;
            OnTurnChanged?.Invoke(m_turn);
            return;
        }
        else if (m_turn == PlayerTurn.Player2)
        {
            Debug.Log("player 2 -> player 1");
            m_turn = PlayerTurn.Player1;
            OnTurnChanged?.Invoke(m_turn);
            return;
        }
    }

    private void EndGame(string message)
    {
        OnGameEnded?.Invoke(message);
    }

    public Result CalculrateWinner()
    {
        Debug.LogError($"{m_player1}  {m_player2}  {m_player1.Hand.CalculateValue()}   {m_player2.Hand.CalculateValue()}");

        if (m_player1.IsBust && m_player2.IsBust) return Result.Draw;
        if (m_player1.Hand.CalculateValue() == m_player2.Hand.CalculateValue()) return Result.Draw;

        if (m_player1.IsBust) return Result.Lose;
        if (m_player2.IsBust) return Result.Win;
        if (m_player1.Hand.CalculateValue() > m_player2.Hand.CalculateValue()) return Result.Win;
        if (m_player1.Hand.CalculateValue() < m_player2.Hand.CalculateValue()) return Result.Lose;

        return Result.None;
    }
    public void WinnerCheck()
    {
        Result result = CalculrateWinner();
        switch (result)
        {
            case Result.Win:
                {
                    EndGame("Player1 Wins.");
                    break;
                }
            case Result.Lose:
                {
                    EndGame("Player2 Wins.");
                    break;
                }
            case Result.Draw:
                {
                    EndGame("Draw");
                    break;
                }
        }
    }
}

